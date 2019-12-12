using System;
using Guards;

namespace Courses.Domain {
    public abstract class Entity {
        protected Entity() {
            Id = null;
            Version = 0;
        }
        protected Entity(string id, int version) {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNegative(version, nameof(version));

            Id = id;
            Version = version;
        }

        public string Id { get; }
        public int Version { get; private set; }

        protected void IncrementVersion() {
            Version++;
        }
    }
}