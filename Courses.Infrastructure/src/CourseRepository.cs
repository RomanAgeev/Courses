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
            var cursor =  await _courses.FindAsync<CourseModel>(it => it.Title == courseTitle);
            var courseModel = await cursor.FirstOrDefaultAsync();
            if (courseModel != null)
                return new Course(courseModel.Title, courseModel.Teacher, courseModel.Capacity);
            return null;
        }
        public void SetCourse(Course course) {
        }
    }
}
