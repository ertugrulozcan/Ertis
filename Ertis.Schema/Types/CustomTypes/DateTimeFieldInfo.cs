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
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.datetime;

        [JsonIgnore] 
        protected override string StringFormat => STRING_FORMAT;

        #endregion
    }
}