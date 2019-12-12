using Xunit;
using FluentAssertions;

namespace Courses.Domain.Tests {
    public class CourseEnrollmentTests {
        [Fact]
        public void AddStudent_Test() {
            var enrollment = new CourseEnrollment(
                courseId:       "Course_1",
                courseVersion:  0,
                courseTitle:    "Course 1",
                capacity:       2,
                students:       new string[0],
                summary:        new CourseSummary()
            );

            var student = new Student(
                id: "Student_1",
                version: 0,
                name: "Student 1",
                age: 30,
                email: "student1@test.com"
            );

            enrollment.AddStudent(student);

            enrollment.CourseId.Should().Be("Course_1");
            enrollment.CourseVersion.Should().Be(1);
            enrollment.CourseTitle.Should().Be("Course 1");
            enrollment.Capacity.Should().Be(2);
            enrollment.Students.Should().BeEquivalentTo(new[] { "Student_1" });
            enrollment.Summary.StudentCount.Should().Be(1);
        }

        [Fact]
        public void AddStudentNoCapacity_Test() {
            var enrollment = new CourseEnrollment(
                courseId:       "Course_1",
                courseVersion:  0,
                courseTitle:    "Course 1",
                capacity:       2,
                students:       new string[] { "Student_1", "Student_2" },
                summary:        new CourseSummary()
            );

            var student = new Student(
                id: "Student_3",
                version: 0,
                name: "Student 3",
                age: 30,
                email: "student3@test.com"
            );

            enrollment.Invoking(it => it.AddStudent(student))
                .Should().Throw<DomainException>();
        }

        [Fact]
        public void AddStudentDuplicated_Test() {
              var enrollment = new CourseEnrollment(
                courseId:       "Course_1",
                courseVersion:  0,
                courseTitle:    "Course 1",
                capacity:       2,
                students:       new string[] { "Student_1" },
                summary:        new CourseSummary()
            );

            var student = new Student(
                id: "Student_1",
                version: 0,
                name: "Student 1",
                age: 30,
                email: "student1@test.com"
            );

            enrollment.Invoking(it => it.AddStudent(student))
                .Should().Throw<DomainException>();
        }
    }
} 