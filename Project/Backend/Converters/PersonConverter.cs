using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class PersonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(IPerson));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        var role = jsonObject["role"]?.Value<string>();

        IPerson person;
        switch (role)
        {
            case "Employee":
                person = new Employee();
                break;
            case "Manager":
                person = new Manager();
                break;
            default:
                throw new Exception("Unknown role type");
        }

        serializer.Populate(jsonObject.CreateReader(), person);
        return person;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}