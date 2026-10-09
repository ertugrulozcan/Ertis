using System.Text.Json.Serialization;

// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Schema.Types;

public interface IHasDefault
{
	#region Methods
	
	object? GetDefaultValue();
	
	#endregion
}

public interface IHasDefault<out T>
{
	#region Properties
	
	[JsonPropertyName("defaultValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	T? DefaultValue { get; }
	
	#endregion
}