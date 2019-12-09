using System.Threading.Tasks;

namespace Courses.Domain {
    public interface ICourseRepository {
        Task<Course> GetCourseAsync(string courseTitle);
        void SetCourse(Course course);
    }
}