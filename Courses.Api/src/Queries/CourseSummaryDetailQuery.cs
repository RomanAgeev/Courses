using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Infrastructure;
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

            var summaryDocument = document[Fields.CourseSummary].ToBsonDocument();

            int studentCount = summaryDocument[Fields.StudentCount].ToInt32();
            int ageSum = summaryDocument[Fields.AgeSum].ToInt32();

            var studentsId = document[Fields.CourseStudents]
                .AsBsonArray
                .Select(it => it.ToString())
                .Select(it => ObjectId.Parse(it))
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
                    Email = studentDoc[Fields.StudentEmail].ToString(),
                    Name = studentDoc[Fields.StudentName].ToString()
                };
            })
            .ToList();

            return new CourseSummaryDetailModel {
                Title = document[Fields.CourseTitle].ToString(),
                Teacher = document[Fields.CourseTeacher].ToString(),
                Capacity = document[Fields.CourseCapacity].ToInt32(),
                StudentCount = studentCount,
                MinAge = summaryDocument[Fields.AgeMin].ToInt32(),
                MaxAge = summaryDocument[Fields.AgeMax].ToInt32(),
                AverageAge = studentCount > 0 ? ageSum / studentCount : 0,
                Students = students
            };
        }
    }
}