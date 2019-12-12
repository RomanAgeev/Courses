using System.Text;
using Newtonsoft.Json;

namespace Courses.Utils {
    public static class Helpers {
        public static byte[] SerializeObject(object obj) {
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T DeserializeObject<T>(byte[] bytes) {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}