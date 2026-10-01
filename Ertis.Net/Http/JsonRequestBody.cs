using System.Net.Http.Headers;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Net.Http;

public class JsonRequestBody : IRequestBody
{
	#region Properties
	
	public object? Payload { get; }
	
	public BodyTypes Type => BodyTypes.Json;
	
	/// <summary>
	/// The serialized payload; a string payload is the json itself
	/// </summary>
	public string? Json =>
		this.Payload switch
		{
			null => null,
			string json => json,
			_ => System.Text.Json.JsonSerializer.Serialize(this.Payload)
		};
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="payload"></param>
	public JsonRequestBody(object payload)
	{
		this.Payload = payload;
	}
	
	#endregion
	
	#region Methods
	
	public HttpContent GetHttpContent()
	{
		var json = this.Json;
		if (json != null)
		{
			var buffer = System.Text.Encoding.UTF8.GetBytes(json);
			var content = new ByteArrayContent(buffer);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return content;	
		}
		else
		{
			return new StringContent(string.Empty);
		}
	}
	
	#endregion
}