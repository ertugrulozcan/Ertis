using System.Globalization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class Date : StringFieldInfo
    {
        #region Constants

        private const string StringFormat = "yyyy-MM-dd";

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.date;

        #endregion

        #region Methods

        protected override void Validate(object obj)
        {
            base.Validate(obj);

            if (obj is not System.DateTime)
            {
                if (obj is string dateStr)
                {
                    if (!IsValidDate(dateStr))
                    {
                        throw new FieldValidationException($"Date is not valid. Datetime values must be '{StringFormat}' format.", this);
                    }
                }
            }
        }
        
        private static bool IsValidDate(string dateString)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(dateString))
                return false;
            
            return System.DateTime.TryParseExact(dateString, StringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        #endregion
    }
}