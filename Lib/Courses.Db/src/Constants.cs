namespace Courses.Db {
    public static class Collections {
        public const string
            Courses = "courses",
            Students = "students";
    }

    public static class Fields {
        public const string
            Id = "_id",
            Version = "version",
            CourseTitle = "title",
            CourseTeacher = "teacher",
            CourseCapacity = "capacity",
            CourseStudents = "students",
            CourseSummary = "summary",
            StudentName = "name",
            StudentEmail = "email",
            StudentAge = "age",
            AgeMin = "ageMin",
            AgeMax = "ageMax",
            AgeSum = "ageSum",
            AgeAvg = "ageAvg",
            StudentCount = "studentCount";
    }
}