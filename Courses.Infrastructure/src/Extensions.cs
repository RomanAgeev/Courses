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
        public static BsonDocument ToBson(this CourseSummary summary) =>
            new BsonDocument {
                { Fields.AgeMin, summary.AgeMin },
                { Fields.AgeMax, summary.AgeMax },
                { Fields.AgeSum, summary.AgeSum },
                { Fields.StudentCount, summary.StudentCount }
            };

        public static CourseSummary FromBson(BsonDocument document) =>
            new CourseSummary(
                ageMin: document[Fields.AgeMin].ToInt32(),
                ageMax: document[Fields.AgeMax].ToInt32(),
                ageSum: document[Fields.AgeSum].ToInt32(),
                studentCount: document[Fields.StudentCount].ToInt32()
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