using System.Text.Json;
using System.Text.Json.Serialization;

namespace NzCovidPass.Core.Models
{
    internal class ContextJsonConverter : JsonConverter<IReadOnlyList<string>>
    {
        public override IReadOnlyList<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.StartArray => JsonSerializer.Deserialize<List<string>>(ref reader, options),
                JsonTokenType.String => new List<string>() { reader.GetString() },
                _ => throw new JsonException("Unexpected JSON data for context."),
            };

        public override void Write(Utf8JsonWriter writer, IReadOnlyList<string> value, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }
}