using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class HostName : StringFieldInfo
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.hostname;

        #endregion

        #region Methods

        protected override void Validate(object obj)
        {
            base.Validate(obj);
            
            if (obj is string hostName)
            {
                if (!IsValidHostName(hostName))
                {
                    throw new FieldValidationException($"Hostname is not valid", this);
                }
            }
        }
        
        private static bool IsValidHostName(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                return false;

            try
            {
                var hostNameType = System.Uri.CheckHostName(hostName);
                return hostNameType != UriHostNameType.Unknown;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}