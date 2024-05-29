using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Ertis.Schema.Types.Primitives
{
    public interface IPrimitiveType
    {
        [JsonProperty("isUnique", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("isUnique")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        bool IsUnique { get; }
    }
}