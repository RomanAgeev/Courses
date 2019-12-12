using System;
using System.Collections.Generic;
using Guards;

namespace Courses.Domain {
    public class CourseEnrollment {
        public CourseEnrollment(string courseId, int courseVersion, string courseTitle, int capacity,
            IEnumerable<string> students, CourseSummary summary) {

            Guard.NotNullOrEmpty(courseId, nameof(courseId));
            Guard.NotNegative(courseVersion, nameof(courseVersion));
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));
            Guard.NotZeroOrNegative(capacity, nameof(capacity));
            Guard.NotNull(students, nameof(students));
            Guard.NotNull(summary, nameof(summary));

            CourseId = courseId;
            CourseVersion = courseVersion;
            CourseTitle = courseTitle;
            Capacity = capacity;
            Summary = summary;

            _students = new List<string>(students);
        }

        readonly List<string> _students;
        
        public string CourseId { get; }
        public int CourseVersion { get; private set;}
        public string CourseTitle { get; private set; }
        public int Capacity { get; private set; }
        public CourseSummary Summary { get; }

        public IReadOnlyCollection<string> Students => _students.AsReadOnly();

        public void AddStudent(Student student) {
            Guard.NotNull(student, nameof(student));

            if (Capacity == _students.Count) {
                throw new DomainException($"Course '{CourseTitle}' capacity {Capacity} is exceeded");
            }

            if (_students.Contains(student.Id))
                throw new DomainException($"Student '{student.Name}' is already in the '{CourseTitle}' coures");

            _students.Add(student.Id);

            Summary.AddStudent(student.Age);

            CourseVersion++;
        }
    }
}
