using System;
using System.Threading.Tasks;
using Courses.Domain;
using Guards;
using MongoDB.Driver;

namespace Courses.Infrastructure {
    public class CourseRepository : ICourseRepository {
        public CourseRepository(string connectionString, string databaseName) {
            Guard.NotNullOrEmpty(connectionString, nameof(connectionString));
            Guard.NotNullOrEmpty(databaseName, nameof(databaseName));

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _courses = database.GetCollection<CourseModel>("courses");
        }

        readonly IMongoCollection<CourseModel> _courses;

        public async Task<Course> GetCourseAsync(string courseTitle) {
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));

            var cursor =  await _courses.FindAsync<CourseModel>(it => it.Title == courseTitle);
            var courseModel = await cursor.FirstOrDefaultAsync();
            if (courseModel != null)
                return new Course(courseModel.Title, courseModel.Teacher, courseModel.Capacity, courseModel.Students);
            return null;
        }
        public async Task SetCourseAsync(Course course) {
            Guard.NotNull(course, nameof(course)); 

            var filter = Builders<CourseModel>.Filter.Eq("title", course.Title);
            var update = Builders<CourseModel>.Update.Set("students", course.Students);

            await _courses.UpdateOneAsync(filter, update);
        }
    }
}
