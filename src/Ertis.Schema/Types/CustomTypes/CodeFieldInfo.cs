using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Types.CustomTypes;

public sealed class CodeFieldInfo : ObjectFieldInfoBase
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.code;
	
	[JsonIgnore]
	public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }
	
	/// <summary>
	/// The values of the predefined types may carry additional data (e.g. image metadata)
	/// </summary>
	protected override bool AcceptsAdditionalProperties => true;
	
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
}