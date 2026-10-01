using System.Text.Json.Serialization;

namespace Ertis.Core.Models;

public interface IHasErrorModel
{
	[JsonPropertyName("error")]
	ErrorModel Error { get; }
}