using System.Threading.Tasks;

namespace Courses.Domain {
    public interface IStudentRepository {
        Task<Student> GetStudentAsync(string email);
        Task InsertStudentAsync(Student student);
    }
}