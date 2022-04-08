using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NzCovidPass.Core.Models
{
    /// <summary>
    /// A custom <see cref="JsonConverter{T}" /> for handling JSON-LD context fields.
    /// </summary>
    /// <remarks>
    /// The context fields can either be an array of strings or single string.
    /// </remarks>
    internal class ContextJsonConverter : JsonConverter<IReadOnlyList<string>>
    {
        public override IReadOnlyList<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    return JsonSerializer.Deserialize<List<string>>(ref reader, options);
                case JsonTokenType.String:
                    return new List<string>() { reader.GetString() };
                default:
                    throw new JsonException("Unexpected JSON data for context.");
            };
        }
        public override void Write(Utf8JsonWriter writer, IReadOnlyList<string> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
