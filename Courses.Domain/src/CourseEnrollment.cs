using System;
using System.Collections.Generic;
using Guards;

namespace Courses.Domain {
    public class CourseEnrollment {
        public CourseEnrollment(string courseId, int courseVersion, int capacity, IEnumerable<string> students) {
            Guard.NotNullOrEmpty(courseId, nameof(courseId));
            Guard.NotNegative(courseVersion, nameof(courseVersion));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));
            Guard.NotNull(students, nameof(students));

            _courseId = courseId;
            _courseVersion = courseVersion;
            _capacity = capacity;
            _students = new List<string>(students);
        }

        readonly string _courseId;
        int _courseVersion;
        readonly int _capacity;
        readonly List<string> _students;
        
        public string CourseId => _courseId;
        public int CourseVersion => _courseVersion;
        public int Capacity => _capacity;
        public IReadOnlyCollection<string> Students => _students.AsReadOnly();

        public void AddStudent(string studentId) {
            Guard.NotNullOrEmpty(studentId, nameof(studentId));

            if (_capacity == _students.Count) {
                throw new Exception("TODO");
            }

            if (_students.Contains(studentId))
                throw new Exception("TODO");

            _students.Add(studentId);

            _courseVersion++;
        }
    }
}
