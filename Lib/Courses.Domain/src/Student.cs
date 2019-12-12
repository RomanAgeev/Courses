using Guards;

namespace Courses.Domain {
    public class Student : Entity {
        public Student(string id, int version, string name, int age, string email)
            : base(id, version) {
            Initialize(name, age, email);
        }

        public Student(string name, int age, string email)
            : base() {
            Initialize(name, age, email);
        }

        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Email { get; private set; }

        void Initialize(string name, int age, string email) {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotZeroOrNegative(age, nameof(age));
            Guard.NotNullOrEmpty(email, nameof(email));

            Name = name;
            Age = age;
            Email = email;
        }
    }
}