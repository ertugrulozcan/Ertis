using System.Text.Json.Serialization;

namespace Ertis.Core.Models.Response;

public interface IHasErrorModel
{
	[JsonPropertyName("error")]
	[Newtonsoft.Json.JsonProperty("error")]
	ErrorModel Error { get; }
}