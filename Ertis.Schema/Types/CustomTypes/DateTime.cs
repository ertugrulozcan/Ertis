using System.Globalization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class DateTime : StringFieldInfo
    {
        #region Constants

        private const string StringFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.datetime;

        #endregion

        #region Methods

        protected override void Validate(object obj)
        {
            base.Validate(obj);

            if (obj is not System.DateTime)
            {
                if (obj is string dateStr)
                {
                    if (!IsValidDateTime(dateStr))
                    {
                        throw new FieldValidationException($"Datetime is not valid. Datetime values must be '{StringFormat}' format.", this);
                    }
                }
            }
        }
        
        private static bool IsValidDateTime(string dateString)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(dateString))
                return false;

            return System.DateTime.TryParseExact(dateString, StringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        #endregion
    }
}