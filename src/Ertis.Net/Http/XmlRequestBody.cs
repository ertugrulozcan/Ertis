using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Net.Http;

public class XmlRequestBody : IRequestBody
{
	#region Properties
	
	public object? Payload { get; }
	
	public BodyTypes Type => BodyTypes.Xml;
	
	public string? Xml
	{
		get
		{
			if (this.Payload == null)
			{
				return null;
			}
			
			// The xml declares the encoding of the writer, the content is sent as UTF-8
			using var stringWriter = new Utf8StringWriter();
			using var xmlWriter = XmlWriter.Create(stringWriter);
			var xmlSerializer = new XmlSerializer(this.Payload.GetType());
			xmlSerializer.Serialize(xmlWriter, this.Payload);
			return stringWriter.ToString();
		}
	}
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="payload"></param>
	public XmlRequestBody(object payload)
	{
		this.Payload = payload;
	}
	
	#endregion
	
	#region Methods
	
	public HttpContent GetHttpContent()
	{
		var xml = this.Xml;
		if (xml != null)
		{
			var buffer = System.Text.Encoding.UTF8.GetBytes(xml);
			var content = new ByteArrayContent(buffer);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
			return content;
		}
		else
		{
			return new StringContent(string.Empty);
		}
	}
	
	#endregion
	
	#region Helper Types
	
	private sealed class Utf8StringWriter : StringWriter
	{
		public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
	}
	
	#endregion
}