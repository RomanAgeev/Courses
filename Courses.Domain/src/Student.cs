using Guards;

namespace Courses.Domain {
    public class Student : Entity {
        public Student(string id, int version, int age, string email)
            : base(id, version) {
            
            Guard.NotZeroOrNegative(age, nameof(age));
            Guard.NotNullOrEmpty(email, nameof(email));

            _age = age;
            _email = email;
        }

        readonly int _age;
        readonly string _email;

        public int Age => _age;
        public string Email => _email;
    }
}