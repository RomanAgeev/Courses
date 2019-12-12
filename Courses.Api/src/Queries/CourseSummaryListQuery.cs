using System.Collections.Generic;
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
    public class CourseSummaryListQuery : IRequest<IEnumerable<CourseSummaryListModel>> {
        public class Validator : AbstractValidator<CourseSummaryListQuery> {
        }
    }

    public class CourseSummaryListQueryHandler : IRequestHandler<CourseSummaryListQuery, IEnumerable<CourseSummaryListModel>> {
        public CourseSummaryListQueryHandler(DbContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly DbContext _context;

        public async Task<IEnumerable<CourseSummaryListModel>> Handle(CourseSummaryListQuery query, CancellationToken ct) {
            var filter = Builders<BsonDocument>.Filter.Empty;

            var project = Builders<BsonDocument>.Projection
                .Include(Fields.CourseTitle)
                .Include(Fields.CourseCapacity)
                .Include(Fields.CourseSummary);

            var documents = await _context.Courses
                .Find(filter)
                .Project(project)
                .ToListAsync();

            return documents
                .Select(document => {
                    var summaryDoc = FromBson.GetSummaryDocument(document);

                    return new CourseSummaryListModel {
                        Title = FromBson.GetCourseTitle(document),
                        Capacity = FromBson.GetCourseCapacity(document),
                        StudentCount = FromBson.GetSummaryStudentCount(summaryDoc),
                        MinAge = FromBson.GetSummaryMin(summaryDoc),
                        MaxAge = FromBson.GetSummaryMax(summaryDoc),
                        AverageAge = FromBson.GetSummaryAvg(summaryDoc)
                    };
                })
                .ToList();
        }
    }
}