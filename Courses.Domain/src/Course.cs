using System;
using System.Collections.Generic;
using System.Linq;
using Guards;

namespace Courses.Domain {
    public class Course {
        public Course(string title, string teacher, int capacity, IEnumerable<string> students) {
            Guard.NotNullOrEmpty(title, nameof(title));
            Guard.NotNullOrEmpty(teacher, nameof(teacher));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));
            Guard.NotNull(students, nameof(students));

            _title = title;
            _teacher = teacher;
            _capacity = capacity;
            _students = new List<string>(students);
        }

        readonly string _title;
        readonly string _teacher;
        readonly int _capacity;
        readonly List<string> _students;

        public string Title => _title;
        public string Teacher => _teacher;
        public int Capacity => _capacity;
        public IReadOnlyCollection<string> Students => _students.AsReadOnly();

        public void AddStudent(string studentName) {
            Guard.NotNullOrEmpty(studentName, nameof(studentName));

            bool inCourse = _students.Any(it => it == studentName);
            if (inCourse)
                throw new Exception("TODO");

            _students.Add(studentName);
        }
    }
}
