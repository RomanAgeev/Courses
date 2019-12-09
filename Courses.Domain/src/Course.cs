using System;
using System.Collections.Generic;
using Guards;

namespace Courses.Domain {
    public class Course : Entity {
        public Course(string id, int version, int capacity, IEnumerable<string> students)
            : base(id, version) {

            Guard.NotZeroOrNegative(capacity, nameof(capacity));
            Guard.NotNull(students, nameof(students));

            _capacity = capacity;
            _students = new List<string>(students);
        }

        readonly int _capacity;
        readonly List<string> _students;
        
        public int Capacity => _capacity;
        public IReadOnlyCollection<string> Students => _students.AsReadOnly();

        public void AddStudent(string studentName) {
            Guard.NotNullOrEmpty(studentName, nameof(studentName));

            if (_capacity == _students.Count) {
                throw new Exception("TODO");
            }

            if (_students.Contains(studentName))
                throw new Exception("TODO");

            _students.Add(studentName);

            IncrementVersion();
        }
    }
}
