using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class Color : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.color;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Color()
        {
            this.RegexPattern = "(?:#|0x)(?:[a-f0-9]{3}|[a-f0-9]{6})\\b|(?:rgb|hsl)a?\\([^\\)]*\\)";
        }

        #endregion

        #region Methods

        protected override void Validate(object obj)
        {
            try
            {
                base.Validate(obj);
            }
            catch (FieldValidationException ex)
            {
                if (ex.Message == $"String value is not valid by the regular expression rule. ('{this.RegexPattern}')")
                {
                    throw new FieldValidationException("Color code is not valid", this);
                }

                throw;
            }
        }

        #endregion
    }
}