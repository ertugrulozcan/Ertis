using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Ertis.Schema.Types
{
    public interface IHasDefault
    {
        #region Methods

        object GetDefaultValue();

        #endregion
    }
    
    public interface IHasDefault<out T>
    {
        #region Properties
        
        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("defaultValue")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        T DefaultValue { get; }
        
        #endregion
    }
}