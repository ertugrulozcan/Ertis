using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Types.CustomTypes
{
	public sealed class CodeFieldInfo : ObjectFieldInfoBase
	{
		#region Properties
        
        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.code;
        
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public CodeFieldInfo()
        {
            this.Properties = new[]
            {
                new StringFieldInfo
                {
                    Name = "code",
                    DisplayName = "Code",
                    Description = "Code",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "language",
                    DisplayName = "Language",
                    Description = "Programming or Script Language",
                    IsRequired = true
                }
            };
        }

        #endregion

        #region Methods

        public override object GetDefaultValue()
        {
            var defaultValue = base.GetDefaultValue();
            if (defaultValue is JObject jObject)
            {
                return DynamicObject.Load(jObject).ToDynamic();
            }
            else
            {
                return defaultValue;
            }
        }

        public override object Clone()
        {
            return new CodeFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
                IsReadonly = this.IsReadonly,
                DefaultValue = this.DefaultValue,
                Properties = this.Properties,
            };
        }

        #endregion
	}
}