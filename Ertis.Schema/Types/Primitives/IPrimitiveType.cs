using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.Primitives;

public interface IPrimitiveType
{
    [JsonPropertyName("isUnique")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isUnique", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    bool IsUnique { get; }
}