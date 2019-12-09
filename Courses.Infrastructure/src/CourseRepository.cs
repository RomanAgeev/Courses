using System.Linq;
using System.Threading.Tasks;
using Courses.Domain;
using Guards;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Courses.Infrastructure {
    public class CourseRepository : ICourseRepository {
        public CourseRepository(string connectionString, string databaseName) {
            Guard.NotNullOrEmpty(connectionString, nameof(connectionString));
            Guard.NotNullOrEmpty(databaseName, nameof(databaseName));

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _courses = database.GetCollection<BsonDocument>(Collections.Courses);
        }

        readonly IMongoCollection<BsonDocument> _courses;

        public async Task<Course> GetCourseAsync(string courseTitle) {
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));

            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.CouseTitle, courseTitle);

            var project = Builders<BsonDocument>.Projection
                .Include(Fields.Id)
                .Include(Fields.Version)
                .Include(Fields.CourseCapacity)
                .Include(Fields.CourseStudents);

            var document = await _courses.Find(filter).Project(project).SingleOrDefaultAsync();

            return document != null ?
                new Course(
                    document[Fields.Id].ToString(),
                    document[Fields.Version].ToInt32(),
                    document[Fields.CourseCapacity].ToInt32(),
                    document[Fields.CourseStudents].AsBsonArray.Select(it => it.ToString())) :
                null;
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

            UpdateResult result = await _courses.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
    }
}
