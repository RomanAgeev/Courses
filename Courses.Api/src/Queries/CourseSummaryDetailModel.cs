using System.Collections.Generic;

namespace Courses.Api.Queries {
    public class CourseSummaryDetailModel {
        public string Title { get; set; }
        public string Teacher { get; set; }
        public int Capacity { get; set; }
        public int StudentCount { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public double AverageAge { get; set; }
        public IEnumerable<StudentSummaryModel> Students { get; set; }
    }
}