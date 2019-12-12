using Guards;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Courses.Infrastructure {
    public class DbContext {
        public DbContext(string connectionString, string databaseName) {
            Guard.NotNullOrEmpty(connectionString, nameof(connectionString));
            Guard.NotNullOrEmpty(databaseName, nameof(databaseName));

            var client = new MongoClient(connectionString);
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