using Guards;

namespace Courses.Domain {
    public class Course : Entity {
        public Course(string title, string teacher, int capacity) {
            Guard.NotNullOrEmpty(title, nameof(title));
            Guard.NotNullOrEmpty(teacher, nameof(teacher));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));

            _title = title;
            _teacher = teacher;
            _capacity = capacity;
        }

        readonly string _title;
        readonly string _teacher;
        readonly int _capacity;

        public string Title => _title;
        public string Teacher => _teacher;
        public int Capacity => _capacity;
    }
}