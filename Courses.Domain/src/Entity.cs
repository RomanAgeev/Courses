using Guards;

namespace Courses.Domain {
    public abstract class Entity {
        protected Entity(string id, int version) {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNegative(version, nameof(version));

            _id = id;
            _version = version;
        }

        readonly string _id;
        int _version;

        public string Id => _id;
        public int Version => _version;

        protected void IncrementVersion() {
            _version++;
        }
    }
}