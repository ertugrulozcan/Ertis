using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.Primitives;

public interface IPrimitiveType
{
	[JsonPropertyName("isUnique")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	bool IsUnique { get; }
}