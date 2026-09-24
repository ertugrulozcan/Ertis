using System.Text.Json.Serialization;
using Ertis.Schema.Types;
using NewtonsoftJsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
using NewtonsoftJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Schema.Exceptions;

public class FieldValidationException : ErtisSchemaValidationException
{
    #region Properties
    
    [JsonIgnore]
    [NewtonsoftJsonIgnore]
    private IFieldInfo FieldInfo { get; }
    
    [JsonPropertyName("fieldName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [NewtonsoftJsonProperty("fieldName", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string FieldName => this.FieldInfo.Name;
    
    [JsonPropertyName("fieldPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [NewtonsoftJsonProperty("fieldPath", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string FieldPath => this.FieldInfo.Path;
    
    [JsonPropertyName("throwEvenOnCreate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [NewtonsoftJsonProperty("throwEvenOnCreate", DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool ThrowEvenOnCreate { get; init; }
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="fieldInfo"></param>
    public FieldValidationException(string message, IFieldInfo fieldInfo) : base(message)
    {
        this.FieldInfo = fieldInfo;
    }
    
    #endregion
}