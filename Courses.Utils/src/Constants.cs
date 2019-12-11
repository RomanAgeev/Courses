namespace Courses.Utils {
    public static class Queues {
        public const string
            LogIn = "QUEUE_LOGIN",
            Notify = "QUEUE_NOTIFY";
    }

    public static class LogInResults {
        public const string
            Succeed = "Succeed",
            NoCourseCapacity = "NoCourseCapacity",
            AlreadyInCourse = "AlreadyInCourse";
    }
}