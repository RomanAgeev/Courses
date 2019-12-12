using Courses.Domain;
using MongoDB.Bson;

namespace Courses.Infrastructure {
    public static class CourseSummaryMongoExtensions {
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
}