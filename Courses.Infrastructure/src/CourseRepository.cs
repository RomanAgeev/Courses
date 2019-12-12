using System.Linq;
using System.Threading.Tasks;
using Courses.Domain;
using Guards;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Courses.Infrastructure {
    public class CourseRepository : ICourseRepository {
        public CourseRepository(DbContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly DbContext _context;

        public async Task<Course> GetCourseAsync(string courseTitle) {
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));

            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.CourseTitle, courseTitle);

            var document = await _context.Courses
                .Find(filter)
                .SingleOrDefaultAsync();

            if (document == null) {
                return null;
            }

            var course = new Course(
                document[Fields.CourseTitle].ToString(),
                document[Fields.CourseTeacher].ToString(),
                document[Fields.CourseCapacity].ToInt32());

            course.InitId(document[Fields.Id].ToString());
            course.InitVersion(document[Fields.Version].ToInt32());

            return course;
        }

        public async Task InsertCourseAsync(Course course) {
            Guard.NotNull(course, nameof(course));

            var document = new BsonDocument {
                { Fields.Version, course.Version },
                { Fields.CourseTitle, course.Title },
                { Fields.CourseTeacher, course.Teacher },
                { Fields.CourseCapacity, course.Capacity },
                { Fields.CourseSummary, new CourseSummary().ToBson() },
                { Fields.CourseStudents, new BsonArray() }
            };

            await _context.Courses.InsertOneAsync(document); 
        }

        public async Task<CourseEnrollment> GetCourseEnrollmentAsync(string courseTitle) {
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));

            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.CourseTitle, courseTitle);

            var project = Builders<BsonDocument>.Projection
                .Include(Fields.Id)
                .Include(Fields.Version)
                .Include(Fields.CourseTitle)
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

            var enrollment = new CourseEnrollment(
                document[Fields.Id].ToString(),
                document[Fields.Version].ToInt32(),
                document[Fields.CourseTitle].ToString(),
                document[Fields.CourseCapacity].ToInt32(),
                document[Fields.CourseStudents].AsBsonArray.Select(it => it.ToString()),
                CourseSummaryMongoExtensions.FromBson(summaryDocument));

            return enrollment;
        }
        
        public async Task<bool> SetCourseEnrollmentAsync(int version, CourseEnrollment enrollment) {
            Guard.NotNull(enrollment, nameof(enrollment)); 

            var idFilter = Builders<BsonDocument>.Filter
                .Eq(Fields.Id, ObjectId.Parse(enrollment.CourseId));

            var versionFilter = Builders<BsonDocument>.Filter
                .Eq(Fields.Version, version);

            var filter = idFilter & versionFilter;

            var update = Builders<BsonDocument>.Update
                .Set(Fields.CourseStudents, enrollment.Students)
                .Set(Fields.Version, enrollment.CourseVersion)
                .Set(Fields.CourseSummary, enrollment.Summary.ToBson());

            UpdateResult result = await _context.Courses.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
    }
}
