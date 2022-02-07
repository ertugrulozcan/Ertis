using Newtonsoft.Json;

namespace Ertis.Schema.Types
{
    public interface IHasDefault<T>
    {
        #region Properties
        
        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        T DefaultValue { get; }
        
        #endregion
    }
}