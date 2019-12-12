using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using Courses.Utils;
using FluentValidation;
using Guards;
using MediatR;

namespace Courses.Worker.Commands {
    public class StudentLogInCommand : IRequest<bool> {
        public class Validator : AbstractValidator<StudentLogInCommand> {
            public Validator() {
                RuleFor(it => it.StudentEmail)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.CourseTitle)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public string StudentEmail { get; set; }
        public string CourseTitle { get; set; }
    }

    public class StudentLogInCommandHandler : IRequestHandler<StudentLogInCommand, bool> {
        public StudentLogInCommandHandler(ICourseRepository courses, IStudentRepository students, IMessageSender messageSender) {
            Guard.NotNull(courses, nameof(courses));
            Guard.NotNull(students, nameof(students));
            Guard.NotNull(messageSender, nameof(messageSender));

            _courses = courses;
            _students = students;
            _messageSender = messageSender;
        }

        readonly ICourseRepository _courses;
        readonly IStudentRepository _students;
        readonly IMessageSender _messageSender;

        public async Task<bool> Handle(StudentLogInCommand command, CancellationToken ct) {
            string error = null;

            try {
                await EnrollStudent(command);
            } catch (DomainException e) {
                error = e.Message;
            }
            
            _messageSender.SendMessage(new {
                CourseTitle = command.CourseTitle,
                StudentEmail = command.StudentEmail,
                Error = error
            });
            
            return true;
        }

        async Task EnrollStudent(StudentLogInCommand command) {
            Student student = await _students.GetStudentAsync(command.StudentEmail);
            if (student == null) {
                throw new DomainException($"Student email '{command.StudentEmail}' doens't exist");
            }

            bool suceed;
            do {
                CourseEnrollment enrollment = await _courses.GetCourseEnrollmentAsync(command.CourseTitle);
                if (enrollment == null) {
                    throw new DomainException($"Course {command.CourseTitle} doesn't exist");
                }

                int version = enrollment.CourseVersion;

                enrollment.AddStudent(student);

                suceed = await _courses.SetCourseEnrollmentAsync(version, enrollment);
            } while(!suceed);
        }
    }
}