using System.Threading.Tasks;

namespace Courses.Domain {
    public interface ICourseRepository {
        Task<Course> GetCourseAsync(string courseTitle);
        Task<bool> SetCourseAsync(int version, Course course);
    }
}