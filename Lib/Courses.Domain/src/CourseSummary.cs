using System;
using Guards;

namespace Courses.Domain {
    public class CourseSummary {
        public CourseSummary()
            : this(-1, -1, 0, 0, 0) {
        }
        public CourseSummary(int ageMin, int ageMax, int ageSum, double ageAvg, int studentCount) {
            _ageMin = ageMin;
            _ageMax = ageMax;
            _ageSum = ageSum;
            _ageAvg = ageAvg;
            _studentCount = studentCount;
        }

        int _ageMin;
        int _ageMax;
        int _ageSum;
        double _ageAvg;
        int _studentCount;

        public int AgeMin => _ageMin;
        public int AgeMax => _ageMax;
        public int AgeSum => _ageSum;
        public double AgeAvg => _ageAvg;
        public int StudentCount => _studentCount;

        public void AddStudent(int studentAge) {
            Guard.NotNegative(studentAge, nameof(studentAge));

            _ageMin = _ageMin < 0 ? studentAge : Math.Min(_ageMin, studentAge);
            _ageMax = _ageMax < 0 ? studentAge : Math.Max(_ageMax, studentAge);
            _ageSum += studentAge;
            _studentCount++;
            _ageAvg = (double)_ageSum / _studentCount;
        }
    }
}