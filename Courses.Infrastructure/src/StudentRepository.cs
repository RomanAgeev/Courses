using System.Threading.Tasks;
using Courses.Domain;
using Guards;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Courses.Infrastructure {
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
                document[Fields.StudentName].ToString(),
                document[Fields.StudentAge].ToInt32(),
                document[Fields.StudentEmail].ToString());

            student.InitId(document[Fields.Id].ToString());
            student.InitVersion(document[Fields.Version].ToInt32());

            return student;
        }

        public async Task InsertStudentAsync(Student student) {
            Guard.NotNull(student, nameof(student));

            var document = new BsonDocument {
                { Fields.Version, student.Version },
                { Fields.StudentName, student.Name },
                { Fields.StudentAge, student.Age },
                { Fields.StudentEmail, student.Email }
            };

            await _context.Students.InsertOneAsync(document);
        }
    }
}