using System.Text.Json.Serialization;

namespace Ertis.Schema.Types;

public interface IHasDefault
{
    #region Methods
    
    object? GetDefaultValue();
    
    #endregion
}

public interface IHasDefault<out T>
{
    #region Properties
    
    [JsonPropertyName("defaultValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("defaultValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    T? DefaultValue { get; }
    
    #endregion
}