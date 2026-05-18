using System.Text.Json.Serialization;

// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Schema.Types.Primitives;

public class BooleanFieldInfo : FieldInfo<bool?>, IPrimitiveType
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.boolean;
	
	[JsonPropertyName("isUnique")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsUnique { get; init; }
	
	#endregion
	
	#region Methods
	
	public override bool ValidateSchema(out Exception? exception)
	{
		base.ValidateSchema(out exception);
		return exception == null;
	}
	
	public override object Clone()
	{
		return new BooleanFieldInfo
		{
			Name = this.Name,
			Description = this.Description,
			DisplayName = this.DisplayName,
			Parent = this.Parent,
			IsRequired = this.IsRequired,
			IsUnique = this.IsUnique,
			IsVirtual = this.IsVirtual,
			IsHidden = this.IsHidden,
			IsReadonly = this.IsReadonly,
			DefaultValue = this.DefaultValue
		};
	}
	
	#endregion
}