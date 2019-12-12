using System.Linq;
using System.Threading.Tasks;
using Courses.Domain;
using Guards;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Courses.Db {
    public class CourseRepository : ICourseRepository {
        public CourseRepository(DbContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly DbContext _context;

        public async Task<Course> GetCourseAsync(string courseTitle) {
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));

            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.CourseTitle, courseTitle);

            var document = await _context.Courses
                .Find(filter)
                .SingleOrDefaultAsync();

            if (document == null) {
                return null;
            }

            return new Course(
                id:         FromBson.GetId(document),
                version:    FromBson.GetVersion(document),
                title:      FromBson.GetCourseTitle(document),
                teacher:    FromBson.GetCourseTeacher(document),
                capacity:   FromBson.GetCourseCapacity(document)
            );
        }

        public async Task InsertCourseAsync(Course course) {
            Guard.NotNull(course, nameof(course));

            var document = course.ToCourseBson();

            await _context.Courses.InsertOneAsync(document); 
        }

        public async Task<CourseEnrollment> GetCourseEnrollmentAsync(string courseTitle) {
            Guard.NotNullOrEmpty(courseTitle, nameof(courseTitle));

            var filter = Builders<BsonDocument>.Filter
                .Eq(Fields.CourseTitle, courseTitle);

            var document = await _context.Courses
                .Find(filter)
                .SingleOrDefaultAsync();

            if (document == null)
                return null;

            BsonDocument summaryDoc = FromBson.GetSummaryDocument(document);

            return new CourseEnrollment(
                courseId:       FromBson.GetId(document),
                courseVersion:  FromBson.GetVersion(document),
                courseTitle:    FromBson.GetCourseTitle(document),
                capacity:       FromBson.GetCourseCapacity(document),
                students:       FromBson.GetCourseStudents(document),
                summary:        new CourseSummary(
                    ageMin:         FromBson.GetSummaryMin(summaryDoc),
                    ageMax:         FromBson.GetSummaryMax(summaryDoc),
                    ageSum:         FromBson.GetSummarySum(summaryDoc),
                    ageAvg:         FromBson.GetSummaryAvg(summaryDoc),
                    studentCount:   FromBson.GetSummaryStudentCount(summaryDoc)
                )
            );
        }
        
        public async Task<bool> SetCourseEnrollmentAsync(int version, CourseEnrollment enrollment) {
            Guard.NotNull(enrollment, nameof(enrollment)); 

            var idFilter = Builders<BsonDocument>.Filter
                .Eq(Fields.Id, ObjectId.Parse(enrollment.CourseId));

            var versionFilter = Builders<BsonDocument>.Filter
                .Eq(Fields.Version, version);

            var filter = idFilter & versionFilter;

            var update = Builders<BsonDocument>.Update
                .Set(Fields.CourseStudents, enrollment.Students)
                .Set(Fields.Version, enrollment.CourseVersion)
                .Set(Fields.CourseSummary, enrollment.Summary.ToCourseSummaryBson());

            UpdateResult result = await _context.Courses.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
    }
}
