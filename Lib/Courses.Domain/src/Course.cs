using Guards;

namespace Courses.Domain {
    public class Course : Entity {
        public Course(string id, int version, string title, string teacher, int capacity)
            : base(id, version) {

            Guard.NotNullOrEmpty(title, nameof(title));
            Guard.NotNullOrEmpty(teacher, nameof(teacher));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));

            _title = title;
            _teacher = teacher;
            _capacity = capacity;
        }
        public Course(string title, string teacher, int capacity)
            : this(null, 0, title, teacher, capacity) {
        }

        readonly string _title;
        readonly string _teacher;
        readonly int _capacity;

        public string Title => _title;
        public string Teacher => _teacher;
        public int Capacity => _capacity;
    }
}