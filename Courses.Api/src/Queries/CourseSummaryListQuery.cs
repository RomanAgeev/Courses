using System.Collections.Generic;
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
                    var summaryDocument = document[Fields.CourseSummary].ToBsonDocument();

                    int studentCount = summaryDocument[Fields.StudentCount].ToInt32();
                    int ageSum = summaryDocument[Fields.AgeSum].ToInt32();

                    return new CourseSummaryListModel {
                        Title = document[Fields.CourseTitle].ToString(),
                        Capacity = document[Fields.CourseCapacity].ToInt32(),
                        StudentCount = studentCount,
                        MinAge = summaryDocument[Fields.AgeMin].ToInt32(),
                        MaxAge = summaryDocument[Fields.AgeMax].ToInt32(),
                        AverageAge = studentCount > 0 ? ageSum / studentCount : 0
                    };
                })
                .ToList();
        }
    }
}