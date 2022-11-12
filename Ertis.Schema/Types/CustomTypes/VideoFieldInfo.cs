using System;
using System.Collections.Generic;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
	public sealed class VideoFieldInfo : ObjectFieldInfoBase
	{
		#region Fields

        private readonly int? maxSize;

        #endregion
        
		#region Properties
        
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.video;
        
        [JsonIgnore]
        public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }

        [JsonProperty("maxSize", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxSize
        {
            get => this.maxSize;
            init
            {
                this.maxSize = value;
                
                if (!this.ValidateMaxSize(out var exception))
                {
                    throw exception;
                }
            }
        }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public VideoFieldInfo()
        {
            this.Properties = new FieldInfo[]
            {
                new StringFieldInfo
                {
                    Name = "id",
                    DisplayName = "Id",
                    Description = "File Id",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "name",
                    DisplayName = "File Name",
                    Description = "File Name",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "path",
                    DisplayName = "File Path",
                    Description = "File Path",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "fullPath",
                    DisplayName = "Full Path",
                    Description = "File Full Path",
                    IsRequired = true
                },
                new StringFieldInfo
                {
                    Name = "mimeType",
                    DisplayName = "Mime Type",
                    Description = "File Mime Type",
                    IsRequired = true
                },
                new FloatFieldInfo
                {
                    Name = "size",
                    DisplayName = "File Size",
                    Description = "File Size (bytes)",
                    IsRequired = false
                },
                new StringFieldInfo
                {
                    Name = "url",
                    DisplayName = "Url",
                    Description = "Url",
                    IsRequired = true
                }
            };
        }

        #endregion

        #region Methods

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateMaxSize(out exception);

            return exception == null;
        }
        
        private bool ValidateMaxSize(out Exception exception)
        {
            if (this.MaxSize < 0)
            {
                exception = new FieldValidationException("MaxSize can not be less than zero", this);
                return false;
            }
            
            exception = null;
            return true;
        }
        
        public override object Clone()
        {
            return new ImageFieldInfo
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
                MaxSize = this.MaxSize,
            };
        }

        #endregion
	}
}