using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StrictStringConverter : JsonConverter<string>
{
    private readonly string _fieldName;

    public StrictStringConverter(string fieldName)
    {
        _fieldName = fieldName;
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"The field '{_fieldName}' must be a string.");
        }
        return reader.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
