using System.IO;
using System.Text;
using System.Text.Json;

namespace WebApplication.Utilities
{
    public static class Json
    {
        /// <summary>
        /// Serialize data as JSON to a stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeIndented">Write indented JSON.</param>
        /// <returns>The stream. Should be disposed.</returns>
        public static MemoryStream ToStream<T>(T data, bool writeIndented = false)
        {
            var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream))
            {
                JsonSerializer.Serialize(jsonWriter, data, typeof(T), new JsonSerializerOptions { WriteIndented = writeIndented });
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Deserialize JSON file content.
        /// </summary>
        public static T DeserializeFile<T>(string filename)
        {
            var content = File.ReadAllText(filename, Encoding.UTF8);
            return JsonSerializer.Deserialize<T>(content);
        }
    }
}
