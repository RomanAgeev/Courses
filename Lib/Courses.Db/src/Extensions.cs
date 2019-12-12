using System.Collections.Generic;
using System.Linq;
using Courses.Domain;
using MongoDB.Bson;

namespace Courses.Db {
    public static class ToBson {
        public static BsonDocument ToCourseSummaryBson(this CourseSummary summary) =>
            new BsonDocument {
                { Fields.AgeMin,        summary.AgeMin },
                { Fields.AgeMax,        summary.AgeMax },
                { Fields.AgeSum,        summary.AgeSum },
                { Fields.AgeAvg,        summary.AgeAvg },
                { Fields.StudentCount,  summary.StudentCount }
            };

        public static BsonDocument ToCourseBson(this Course course) =>
            new BsonDocument {
                { Fields.Version,           course.Version },
                { Fields.CourseTitle,       course.Title },
                { Fields.CourseTeacher,     course.Teacher },
                { Fields.CourseCapacity,    course.Capacity },
                { Fields.CourseSummary,     new CourseSummary().ToCourseSummaryBson() },
                { Fields.CourseStudents,    new BsonArray() }
            };

        public static BsonDocument ToStudentBson(this Student student) =>
            new BsonDocument {
                { Fields.Version,       student.Version },
                { Fields.StudentName,   student.Name },
                { Fields.StudentAge,    student.Age },
                { Fields.StudentEmail,  student.Email }
            };
    }

    public static class FromBson {
        public static string GetId(BsonDocument document) => document[Fields.Id].ToString();
        public static int GetVersion(BsonDocument document) => document[Fields.Version].ToInt32();
        public static string GetCourseTitle(BsonDocument document) => document[Fields.CourseTitle].ToString();
        public static string GetCourseTeacher(BsonDocument document) => document[Fields.CourseTeacher].ToString();
        public static int GetCourseCapacity(BsonDocument document) => document[Fields.CourseCapacity].ToInt32();
        public static IEnumerable<string> GetCourseStudents(BsonDocument document) =>
            document[Fields.CourseStudents].AsBsonArray.Select(it => it.ToString());

        public static BsonDocument GetSummaryDocument(BsonDocument document) => document[Fields.CourseSummary].ToBsonDocument();
        public static int GetSummaryMin(BsonDocument document) => document[Fields.AgeMin].ToInt32();
        public static int GetSummaryMax(BsonDocument document) => document[Fields.AgeMax].ToInt32();
        public static int GetSummarySum(BsonDocument document) => document[Fields.AgeSum].ToInt32();
        public static double GetSummaryAvg(BsonDocument document) => document[Fields.AgeAvg].ToDouble();
        public static int GetSummaryStudentCount(BsonDocument document) => document[Fields.StudentCount].ToInt32();
        public static string GetStudentName(BsonDocument document) => document[Fields.StudentName].ToString();
        public static int GetStudentAge(BsonDocument document) => document[Fields.StudentAge].ToInt32();
        public static string GetStudentEmail(BsonDocument document) => document[Fields.StudentEmail].ToString();
    }
}