using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class DateTimeFieldInfo : DateTimeFieldInfoBase<DateTimeFieldInfo>
    {
        #region Constants

        private const string STRING_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.datetime;

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        protected override string StringFormat => STRING_FORMAT;

        #endregion
    }
}