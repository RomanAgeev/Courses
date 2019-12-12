using System.Threading.Tasks;
using Courses.Domain;
using Guards;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Courses.Db {
    public class StudentRepository : IStudentRepository {
        public StudentRepository(DbContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly DbContext _context;

        public async Task<Student> GetStudentAsync(string email) {
            Guard.NotNullOrEmpty(email, nameof(email));

            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.StudentEmail, email);

            var document = await _context.Students
                .Find(filter)
                .SingleOrDefaultAsync();

            if (document == null)
                return null;

            var student = new Student(
                email:  FromBson.GetStudentEmail(document),
                name:   FromBson.GetStudentName(document),
                age:    FromBson.GetStudentAge(document)
            );

            student.InitId(FromBson.GetId(document));
            student.InitVersion(FromBson.GetVersion(document));

            return student;
        }

        public async Task InsertStudentAsync(Student student) {
            Guard.NotNull(student, nameof(student));

            var document = student.ToStudentBson();

            await _context.Students.InsertOneAsync(document);
        }
    }
}