using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class Uri : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.uri;

        #endregion

        #region Methods

        protected override void Validate(object obj)
        {
            base.Validate(obj);
            
            if (obj is string uri)
            {
                if (!IsValidUri(uri))
                {
                    throw new FieldValidationException($"Uri is not valid", this);
                }
            }
        }
        
        private static bool IsValidUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return false;

            return System.Uri.IsWellFormedUriString(uri, UriKind.Absolute) &&
                   System.Uri.TryCreate(uri, UriKind.Absolute, out _);
        }

        #endregion
    }
}