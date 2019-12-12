using Guards;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Courses.Db {
    public class DbContext {
        public DbContext(string connectionString) {
            Guard.NotNullOrEmpty(connectionString, nameof(connectionString));

            var client = new MongoClient(connectionString);

            string[] sections = connectionString.Split('/');
            
            string databaseName = sections[sections.Length - 1];

            var database = client.GetDatabase(databaseName);

            _students = database.GetCollection<BsonDocument>(Collections.Students);
            _courses = database.GetCollection<BsonDocument>(Collections.Courses);
        }

        readonly IMongoCollection<BsonDocument> _students;
        readonly IMongoCollection<BsonDocument> _courses;

        public IMongoCollection<BsonDocument> Students => _students;
        public IMongoCollection<BsonDocument> Courses => _courses;
    }
}