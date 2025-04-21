using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArizaKayitApi.Converters
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>

    {
        private readonly string _dateFormat = "MMM d, yyyy h:mm:ss tt";  // Tarih formatı: 'Mar 7, 2025 8:42:24 AM'

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), _dateFormat, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateFormat));  // Tarihi belirtilen formatta yaz
        }
    }
}
