using Guards;

namespace Courses.Domain {
    public class Course : Entity {
        public Course(string id, int version, string title, string teacher, int capacity)
            : base(id, version) {
            Initialize(title, teacher, capacity);
        }
        public Course(string title, string teacher, int capacity)
            : base() {
            Initialize(title, teacher, capacity);
        }

        public string Title { get; private set; }
        public string Teacher { get; private set; }
        public int Capacity { get; private set; }

        void Initialize(string title, string teacher, int capacity) {
            Guard.NotNullOrEmpty(title, nameof(title));
            Guard.NotNullOrEmpty(teacher, nameof(teacher));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));

            Title = title;
            Teacher = teacher;
            Capacity = capacity;
        }
    }
}