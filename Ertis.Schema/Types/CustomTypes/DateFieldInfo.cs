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
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.date;

        [JsonIgnore] 
        protected override string StringFormat => STRING_FORMAT;

        #endregion
    }
}