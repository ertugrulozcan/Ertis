using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class DateFieldInfo : DateTimeFieldInfoBase<DateFieldInfo>
    {
        #region Constants

        private const string STRING_FORMAT = "yyyy-MM-dd";

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.date;

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        protected override string StringFormat => STRING_FORMAT;

        #endregion
    }
}