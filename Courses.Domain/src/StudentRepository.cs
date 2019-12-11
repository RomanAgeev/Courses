using System.Threading.Tasks;

namespace Courses.Domain {
    public interface IStudentRepository {
        Task<Student> GetStudentAsync(string email);
        Task<bool> SetStudentAsync(int version, Student student);
    }
}