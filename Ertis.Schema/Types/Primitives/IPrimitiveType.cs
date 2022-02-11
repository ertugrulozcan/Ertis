using Newtonsoft.Json;

namespace Ertis.Schema.Types.Primitives
{
    public interface IPrimitiveType
    {
        [JsonProperty("isUnique", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        bool IsUnique { get; }
    }
}