using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DueDateStrictDateConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString()!;
            if (DateTime.TryParse(dateString, out var date))
            {
                return date;
            }
            else
            {
                throw new JsonException("The field 'dueDate' must be a valid date.");
            }
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("The field 'dueDate' is required.");
        }
        else
        {
            throw new JsonException("The field 'dueDate' must be a valid date string.");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("o")); // ISO 8601 format
    }
}
