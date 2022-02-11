using System.Collections.Generic;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public sealed class Location : ObjectFieldInfoBase
    {
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.location;
        
        [JsonIgnore]
        public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Location()
        {
            this.Properties = new[]
            {
                new FloatFieldInfo
                {
                    Name = "latitude",
                    DisplayName = "Latitude",
                    Description = "Latitude",
                    Minimum = -90.0d,
                    Maximum = 90.0d,
                    IsRequired = true
                },
                new FloatFieldInfo
                {
                    Name = "longitude",
                    DisplayName = "Longitude",
                    Description = "Longitude",
                    Minimum = -180.0d,
                    Maximum = 180.0d,
                    IsRequired = true
                }
            };
        }

        public override object Clone()
        {
            return new Location
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                DefaultValue = this.DefaultValue,
                Properties = this.Properties
            };
        }

        #endregion
    }
}