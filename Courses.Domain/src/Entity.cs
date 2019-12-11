using System;
using Guards;

namespace Courses.Domain {
    public abstract class Entity {
        string _id;
        int? _version;

        public string Id => _id;
        public int Version => _version ?? 0;

        public void InitId(string id) {
            Guard.NotNullOrEmpty(id, nameof(id));

            if (_id != null) 
                throw new Exception("TODO");

            _id = id;
        }

        public void InitVersion(int version) {
            Guard.NotNegative(version, nameof(version));

            if(_version.HasValue) {
                throw new Exception("TODO");
            }

            _version = version;
        }

        protected void IncrementVersion() {
            _version++;
        }
    }
}