using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Types.CustomTypes;

public sealed class LocationFieldInfo : ObjectFieldInfoBase
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Newtonsoft.Json.JsonProperty("type")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public override FieldType Type => FieldType.location;
	
	[JsonIgnore]
	[Newtonsoft.Json.JsonIgnore]
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
	public LocationFieldInfo()
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
	
	#endregion
}