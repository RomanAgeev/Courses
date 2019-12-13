using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using Courses.Utils;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;

namespace Courses.Worker.Tests {
    public class StudentEnrollCommandTests {
        public StudentEnrollCommandTests() {
            _fakeCourses = A.Fake<ICourseRepository>();
            _fakeStudents = A.Fake<IStudentRepository>();
            _fakeSender = A.Fake<IMessageSender>();

            _commandHandler = new StudentEnrollCommandHandler(_fakeCourses,_fakeStudents,_fakeSender,
                A.Fake<ILogger<StudentEnrollCommandHandler>>());
        }

        ICourseRepository _fakeCourses;
        IStudentRepository _fakeStudents;
        IMessageSender _fakeSender;
        StudentEnrollCommandHandler _commandHandler;

        [Fact]
        public async Task EnrollStudent_Test() {
            const string studentId = "Student_1";
            const string studentEmail = "student1@test.com";
            const string courseTitle = "Math";
            const int courseVersion = 0;

            var student = new Student(
                id: studentId,
                version: 0,
                name: "Student 1",
                age: 30,
                email: studentEmail
            );

            var enrollment = new CourseEnrollment(
                courseId: "Course_Math",
                courseVersion,
                courseTitle,
                capacity: 10,
                students: new string[0],
                new CourseSummary()
            );
            
            A.CallTo(() => _fakeStudents.GetStudentAsync(studentEmail)).Returns(student);

            A.CallTo(() => _fakeCourses.GetCourseEnrollmentAsync(courseTitle)).Returns(enrollment);
            
            var setCourseEnrollment = A.CallTo(() => _fakeCourses.SetCourseEnrollmentAsync(courseVersion, enrollment));

            setCourseEnrollment.Returns(true);

            var sendMessage = A.CallTo(() => _fakeSender.SendMessage(A<StudentEnrollCommandHandler.MassagePayload>._))
                .WhenArgumentsMatch((StudentEnrollCommandHandler.MassagePayload payload) =>
                    payload.CourseTitle == courseTitle &&
                    payload.StudentEmail == studentEmail &&
                    payload.Error == null);

            var command = new StudentEnrollCommand {
                StudentEmail = studentEmail,
                CourseTitle = courseTitle
            };

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

            setCourseEnrollment.MustHaveHappenedOnceExactly();
            sendMessage.MustHaveHappenedOnceExactly();

            enrollment.Students.Should().BeEquivalentTo(new[] { studentId });
        }
    }
}
