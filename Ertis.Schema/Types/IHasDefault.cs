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
        T DefaultValue { get; }
        
        #endregion
    }
}