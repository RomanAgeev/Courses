using Guards;

namespace Courses.Domain {
    public class Student : Entity {
        public Student(string name, int age, string email) {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotZeroOrNegative(age, nameof(age));
            Guard.NotNullOrEmpty(email, nameof(email));

            _name = name;
            _age = age;
            _email = email;
        }

        readonly string _name;
        readonly int _age;
        readonly string _email;

        public string Name => _name;
        public int Age => _age;
        public string Email => _email;
    }
}