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
                .Eq(Fields.CouseTitle, courseTitle);

            var project = Builders<BsonDocument>.Projection
                .Include(Fields.Id)
                .Include(Fields.Version)
                .Include(Fields.CourseCapacity)
                .Include(Fields.CourseStudents);

            var document = await _context.Courses
                .Find(filter)
                .Project(project)
                .SingleOrDefaultAsync();

            if (document == null)
                return null;

            var course = new Course(
                document[Fields.CourseCapacity].ToInt32(),
                document[Fields.CourseStudents].AsBsonArray.Select(it => it.ToString()));

            course.InitId(document[Fields.Id].ToString());
            course.InitVersion(document[Fields.Version].ToInt32());

            return course;
        }
        
        public async Task<bool> SetCourseAsync(int version, Course course) {
            Guard.NotNull(course, nameof(course)); 

            var idFilter = Builders<BsonDocument>.Filter
                .Eq(Fields.Id, ObjectId.Parse(course.Id));

            var versionFilter = Builders<BsonDocument>.Filter
                .Eq(Fields.Version, version);

            var filter = idFilter & versionFilter;

            var update = Builders<BsonDocument>.Update
                .Set(Fields.CourseStudents, course.Students)
                .Set(Fields.Version, course.Version);

            UpdateResult result = await _context.Courses.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
    }
}
