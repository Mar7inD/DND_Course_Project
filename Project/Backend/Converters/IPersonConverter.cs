using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Converters
{
    public class IPersonConverter : JsonConverter<IPerson>
    {
        public override IPerson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
            var role = jsonObject.GetProperty("role").GetString();

            if (string.IsNullOrEmpty(role))
            {
                throw new NotSupportedException("Role is missing or empty");
            }

            IPerson? person = role switch
            {
                "Manager" => JsonSerializer.Deserialize<Manager>(jsonObject.GetRawText(), options),
                "Employee" => JsonSerializer.Deserialize<Employee>(jsonObject.GetRawText(), options),
                _ => throw new NotSupportedException($"Role '{role}' is not supported")
            };

             if (person == null)
            {
                throw new JsonException($"Deserialization of role '{role}' resulted in null");
            }

            return person;
        }

        public override void Write(Utf8JsonWriter writer, IPerson value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}