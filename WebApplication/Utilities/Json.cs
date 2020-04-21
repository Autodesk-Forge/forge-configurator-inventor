using System.IO;
using System.Text.Json;

namespace WebApplication.Utilities
{
    public static class Json
    {
        /// <summary>
        /// Serialize data as JSON to a stream.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The stream. Should be disposed.</returns>
        public static MemoryStream ToStream<T>(T data)
        {
            var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream))
            {
                JsonSerializer.Serialize(jsonWriter, data, typeof(T), new JsonSerializerOptions { WriteIndented = true });
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
