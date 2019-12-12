namespace Courses.Api.Queries {
    public class CourseSummaryListModel {
        public string Title { get; set; }
        public int Capacity { get; set; }
        public int StudentCount { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public double AverageAge { get; set; }
    }
}