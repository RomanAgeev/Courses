using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Courses.Infrastructure {
    public class CourseModel {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("teacher")]
        public string Teacher { get; set; }

        [BsonElement("capacity")]
        public int Capacity { get; set; }
        
        [BsonElement("students")]
        public IEnumerable<string> Students { get; set; }

        [BsonElement("version")]
        public int Version { get; set; }
    }
}