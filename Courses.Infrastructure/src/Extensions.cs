using System.Linq;
using Courses.Domain;
using MongoDB.Bson;

namespace Courses.Infrastructure {
    public static class StudentBson {
        public static BsonDocument ToBson(this Student student) =>
            new BsonDocument {
                { Fields.Version, student.Version },
                { Fields.StudentName, student.Name },
                { Fields.StudentAge, student.Age },
                { Fields.StudentEmail, student.Email }
            };

        public static Student FromBson(BsonDocument document) {
            var student = new Student(
                document[Fields.StudentName].ToString(),
                document[Fields.StudentAge].ToInt32(),
                document[Fields.StudentEmail].ToString());

            student.InitId(document[Fields.Id].ToString());
            student.InitVersion(document[Fields.Version].ToInt32());

            return student;
        }
    }

    public static class CourseBson {
        public static BsonDocument ToBson(this Course course) =>
            new BsonDocument {
                { Fields.Version, course.Version },
                { Fields.CourseTitle, course.Title },
                { Fields.CourseTeacher, course.Teacher },
                { Fields.CourseCapacity, course.Capacity },
                { Fields.CourseSummary, new CourseSummary().ToBson() },
                { Fields.CourseStudents, new BsonArray() }
            };

        public static Course FromBson(BsonDocument document) {
            var course = new Course(
                document[Fields.CourseTitle].ToString(),
                document[Fields.CourseTeacher].ToString(),
                document[Fields.CourseCapacity].ToInt32());

            course.InitId(document[Fields.Id].ToString());
            course.InitVersion(document[Fields.Version].ToInt32());

            return course;
        }
    }

    public static class CourseSummaryBson {
        const string FieldAgeMin = "ageMin";
        const string FieldAgeMax = "ageMax";
        const string FieldAgeSum = "ageSum";
        const string FieldStudentCount = "studentCount";

        public static BsonDocument ToBson(this CourseSummary summary) =>
            new BsonDocument {
                { FieldAgeMin, summary.AgeMin },
                { FieldAgeMax, summary.AgeMax },
                { FieldAgeSum, summary.AgeSum },
                { FieldStudentCount, summary.StudentCount }
            };

        public static CourseSummary FromBson(BsonDocument document) =>
            new CourseSummary(
                ageMin: document[FieldAgeMin].ToInt32(),
                ageMax: document[FieldAgeMax].ToInt32(),
                ageSum: document[FieldAgeSum].ToInt32(),
                studentCount: document[FieldStudentCount].ToInt32()
            );
    }

    public static class CourseEnrollmentBson {
        public static CourseEnrollment FromBson(BsonDocument document) =>
            new CourseEnrollment(
                document[Fields.Id].ToString(),
                document[Fields.Version].ToInt32(),
                document[Fields.CourseTitle].ToString(),
                document[Fields.CourseCapacity].ToInt32(),
                document[Fields.CourseStudents].AsBsonArray.Select(it => it.ToString()),
                CourseSummaryBson.FromBson(document[Fields.CourseSummary].ToBsonDocument()));
    }
}