using System;

namespace Courses.Domain {
    public class DomainException : Exception {
        public DomainException(string message)
            : base(message) {
        }
    }
}