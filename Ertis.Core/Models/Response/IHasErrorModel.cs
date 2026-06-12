using System.Text.Json.Serialization;
using JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;

namespace Ertis.Core.Models.Response;

public interface IHasErrorModel
{
	[JsonProperty("error")]
	[JsonPropertyName("error")]
	ErrorModel Error { get; }
}