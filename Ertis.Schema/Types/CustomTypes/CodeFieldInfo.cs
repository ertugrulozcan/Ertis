using System.Collections.Generic;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
	public sealed class CodeFieldInfo : ObjectFieldInfoBase
	{
		#region Properties
        
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.code;
        
        [JsonIgnore]
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