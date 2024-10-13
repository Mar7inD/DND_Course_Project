using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Models;

namespace Backend.Converters;

public class IPersonDTOConverter : JsonConverter<IPersonDTO>
{
    public override IPersonDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            JsonElement root = doc.RootElement;
            string? role = root.GetProperty("role").GetString();

            if (role == "Employee")
            {
                return JsonSerializer.Deserialize<EmployeeDTO>(root.GetRawText(), options)!;
            }
            else if (role == "Manager")
            {
                return JsonSerializer.Deserialize<ManagerDTO>(root.GetRawText(), options)!;
            }
            else
            {
                throw new NotSupportedException($"Role '{role}' is not supported.");
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, IPersonDTO value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}