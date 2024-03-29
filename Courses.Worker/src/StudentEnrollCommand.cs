using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using Courses.Utils;
using FluentValidation;
using Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Courses.Worker {
    public class StudentEnrollCommand : IRequest<bool> {
        public class Validator : AbstractValidator<StudentEnrollCommand> {
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

    public class StudentEnrollCommandHandler : IRequestHandler<StudentEnrollCommand, bool> {
        public class MassagePayload {
            public string CourseTitle { get; set; }
            public string StudentEmail { get; set; }
            public string Error { get; set; } 
        }

        public StudentEnrollCommandHandler(ICourseRepository courses, IStudentRepository students,
            IMessageSender messageSender, ILogger<StudentEnrollCommandHandler> logger) {

            Guard.NotNull(courses, nameof(courses));
            Guard.NotNull(students, nameof(students));
            Guard.NotNull(messageSender, nameof(messageSender));
            Guard.NotNull(logger, nameof(logger));

            _courses = courses;
            _students = students;
            _messageSender = messageSender;
            _logger = logger;
        }

        readonly ICourseRepository _courses;
        readonly IStudentRepository _students;
        readonly IMessageSender _messageSender;
        readonly ILogger<StudentEnrollCommandHandler> _logger;

        public async Task<bool> Handle(StudentEnrollCommand command, CancellationToken ct) {
            string error = null;

            try {
                await EnrollStudent(command);
            } catch (DomainException e) {
                error = e.Message;
            }

            _logger.LogInformation($"Sending message to {_messageSender.QueueName}");
            
            _messageSender.SendMessage(new MassagePayload {
                CourseTitle = command.CourseTitle,
                StudentEmail = command.StudentEmail,
                Error = error
            });
            
            return true;
        }

        async Task EnrollStudent(StudentEnrollCommand command) {
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