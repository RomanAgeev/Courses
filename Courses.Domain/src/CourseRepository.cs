using System.Threading.Tasks;

namespace Courses.Domain {
    public interface ICourseRepository {
        Task<Course> GetCourseAsync(string courseTitle);
        Task InsertCourseAsync(Course course);
        Task<CourseEnrollment> GetCourseEnrollmentAsync(string courseTitle);
        Task<bool> SetCourseEnrollmentAsync(int version, CourseEnrollment enrollment);
    }
}