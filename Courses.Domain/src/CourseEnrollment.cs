using System;
using System.Collections.Generic;
using Guards;

namespace Courses.Domain {
    public class CourseEnrollment {
        public CourseEnrollment(string courseId, int courseVersion, string courseTitle, int capacity, IEnumerable<string> students) {
            Guard.NotNullOrEmpty(courseId, nameof(courseId));
            Guard.NotNegative(courseVersion, nameof(courseVersion));
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));
            Guard.NotNull(students, nameof(students));

            _courseId = courseId;
            _courseVersion = courseVersion;
            _courseTitle = courseTitle;
            _capacity = capacity;
            _students = new List<string>(students);
        }

        readonly string _courseId;
        int _courseVersion;
        readonly string _courseTitle;
        readonly int _capacity;
        readonly List<string> _students;
        
        public string CourseId => _courseId;
        public int CourseVersion => _courseVersion;
        public int Capacity => _capacity;
        public IReadOnlyCollection<string> Students => _students.AsReadOnly();

        public void AddStudent(Student student) {
            Guard.NotNull(student, nameof(student));

            if (_capacity == _students.Count) {
                throw new DomainException($"Course '{_courseTitle}' capacity {_capacity} is exceeded");
            }

            if (_students.Contains(student.Id))
                throw new DomainException($"Student '{student.Name}' is already in the '{_courseTitle}' coures");

            _students.Add(student.Id);

            _courseVersion++;
        }
    }
}
