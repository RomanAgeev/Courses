using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Db;
using FluentValidation;
using Guards;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Courses.Api.Queries {
    public class CourseSummaryDetailQuery : IRequest<CourseSummaryDetailModel> {
        public class Validator : AbstractValidator<CourseSummaryDetailQuery> {
            public Validator() {
                RuleFor(it => it.CourseTitle)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public string CourseTitle { get; set; }
    }

    public class CourseSummaryDetailQueryHandler : IRequestHandler<CourseSummaryDetailQuery, CourseSummaryDetailModel> {
        public CourseSummaryDetailQueryHandler(DbContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly DbContext _context;

        public async Task<CourseSummaryDetailModel> Handle(CourseSummaryDetailQuery query, CancellationToken ct) {
            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.CourseTitle, query.CourseTitle);

            var project = Builders<BsonDocument>.Projection
                .Include(Fields.CourseTitle)
                .Include(Fields.CourseTeacher)
                .Include(Fields.CourseCapacity)
                .Include(Fields.CourseStudents)
                .Include(Fields.CourseSummary);

            var document = await _context.Courses
                .Find(filter)
                .Project(project)
                .SingleOrDefaultAsync();

            if (document == null)
                return null;

            var studentsId = FromBson.GetCourseStudents(document)
                .Select(id => ObjectId.Parse(id))
                .ToList();
            
            var studentsFilter = Builders<BsonDocument>.Filter
                .In(Fields.Id, studentsId);

            var studentsProject = Builders<BsonDocument>.Projection
                .Include(Fields.StudentEmail)
                .Include(Fields.StudentName);

            var studentDocuments = await _context.Students
                .Find(studentsFilter)
                .Project(studentsProject)
                .ToListAsync();

            var students = studentDocuments.Select(studentDoc => {
                return new StudentSummaryModel {
                    Email = FromBson.GetStudentEmail(studentDoc),
                    Name = FromBson.GetStudentName(studentDoc)
                };
            })
            .ToList();

            var summaryDoc = FromBson.GetSummaryDocument(document);

            return new CourseSummaryDetailModel {
                Title = FromBson.GetCourseTitle(document),
                Teacher = document[Fields.CourseTeacher].ToString(),
                Capacity = FromBson.GetCourseCapacity(document),
                StudentCount = FromBson.GetSummaryStudentCount(summaryDoc),
                MinAge = FromBson.GetSummaryMin(summaryDoc),
                MaxAge = FromBson.GetSummaryMax(summaryDoc),
                AverageAge = FromBson.GetSummaryAvg(summaryDoc),
                Students = students
            };
        }
    }
}