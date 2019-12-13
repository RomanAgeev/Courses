namespace Courses.Domain.Tests {
    public static class TestHelpers {
        public static Student CreateStudent(int id, int age) =>
            new Student(
                id: $"Student_{id}",
                version: 0,
                name: $"Student {id}",
                age: 30,
                email: $"student{id}@test.com"
            );
    }
}