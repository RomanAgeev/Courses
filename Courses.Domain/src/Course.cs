using System;
using System.Collections.Generic;
using System.Linq;

namespace Courses.Domain {
    public class Course {
        public Course(string title, string teacher, int capacity) {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"{nameof(title)} cannot be null of empty", nameof(title));

            if (string.IsNullOrEmpty(teacher))
                throw new ArgumentException($"{nameof(teacher)} cannot be null of empty", nameof(teacher));

            if (capacity <= 0)
                throw new ArgumentException($"{nameof(capacity)} cannot be negative or zero", nameof(capacity));

            _title = title;
            _teacher = teacher;
            _capacity = capacity;
        }

        string _title;
        string _teacher;
        int _capacity;

        readonly List<string> _students = new List<string>();

        public void AddStudent(string studentName) {
            if (string.IsNullOrEmpty(studentName))
                throw new ArgumentException($"{nameof(studentName)} cannot be null of empty", nameof(studentName));

            bool exist = _students.Any(it => it == studentName);
            if (exist)
                throw new Exception("TODO");

            _students.Add(studentName);
        }
    }
}
