using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Exceptions;

// ReSharper disable UnusedMember.Global
namespace Ertis.Schema.Types.CustomTypes;

public sealed class VideoFieldInfo : ObjectFieldInfoBase
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.video;
	
	[JsonIgnore]
	public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }
	
	/// <summary>
	/// The values of the predefined types may carry additional data (e.g. image metadata)
	/// </summary>
	protected override bool AcceptsAdditionalProperties => true;
	
	[JsonPropertyName("maxSize")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? MaxSize
	{
		get;
		init
		{
			field = value;
			
			if (!this.ValidateMaxSize(out var exception) && exception != null)
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
				IsRequired = false
			},
			new StringFieldInfo
			{
				Name = "name",
				DisplayName = "File Name",
				Description = "File Name",
				IsRequired = false
			},
			new StringFieldInfo
			{
				Name = "path",
				DisplayName = "File Path",
				Description = "File Path",
				IsRequired = false
			},
			new StringFieldInfo
			{
				Name = "fullPath",
				DisplayName = "Full Path",
				Description = "File Full Path",
				IsRequired = false
			},
			new StringFieldInfo
			{
				Name = "mimeType",
				DisplayName = "Mime Type",
				Description = "File Mime Type",
				IsRequired = false
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
				IsRequired = false
			}
		};
	}
	
	#endregion
	
	#region Methods
	
	public override bool ValidateSchema(out Exception? exception)
	{
		return base.ValidateSchema(out exception) &&
			this.ValidateMaxSize(out exception);
	}
	
	private bool ValidateMaxSize(out Exception? exception)
	{
		if (this.MaxSize < 0)
		{
			exception = new FieldValidationException("MaxSize can not be less than zero", this);
			return false;
		}
		
		exception = null;
		return true;
	}
	
	#endregion
}