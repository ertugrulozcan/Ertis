using System.Text.Json.Serialization;

namespace Ertis.Core.Models;

public interface IHasErrorModel
{
	#region Properties
	
	[JsonPropertyName("error")]
	ErrorModel Error { get; }
	
	#endregion
}