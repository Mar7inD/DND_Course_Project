using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Converters
{
    public class IPersonConverter : JsonConverter<IPerson>
    {
        public override IPerson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

            // If "role" is missing, we skip the check and assume the default "Employee"
            if (!jsonObject.TryGetProperty("role", out var roleProperty) || string.IsNullOrEmpty(roleProperty.GetString()))
            {
                return JsonSerializer.Deserialize<Employee>(jsonObject.GetRawText(), options) ?? throw new JsonException("Failed to deserialize as Employee");
            }

            var role = roleProperty.GetString();
            return role switch
            {
                "Manager" => JsonSerializer.Deserialize<Manager>(jsonObject.GetRawText(), options) ?? throw new JsonException("Failed to deserialize as Manager"),
                "Employee" => JsonSerializer.Deserialize<Employee>(jsonObject.GetRawText(), options) ?? throw new JsonException("Failed to deserialize as Employee"),
                _ => throw new NotSupportedException($"Role '{role}' is not supported")
            };
        }


        public override void Write(Utf8JsonWriter writer, IPerson value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}