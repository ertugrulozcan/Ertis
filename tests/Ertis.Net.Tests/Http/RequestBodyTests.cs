using System.Text;
using Ertis.Net.Http;

namespace Ertis.Net.Tests.Http;

public class RequestBodyTests
{
	#region Methods
	
	private static async Task<(string Content, string? ContentType, string? CharSet)> ReadAsync(IRequestBody body)
	{
		using var content = body.GetHttpContent();
		return (await content.ReadAsStringAsync(TestContext.Current.CancellationToken), content.Headers.ContentType?.MediaType, content.Headers.ContentType?.CharSet);
	}
	
	#endregion
	
	#region Json Methods
	
	[Fact]
	public async Task JsonRequestBody_SerializesThePayload()
	{
		var body = new JsonRequestBody(new { name = "Jane", tags = new[] { "a" } });
		
		var (content, contentType, _) = await ReadAsync(body);
		
		Assert.Equal("""{"name":"Jane","tags":["a"]}""", content);
		Assert.Equal(content, body.Json);
		Assert.Equal("application/json", contentType);
		Assert.Equal(BodyTypes.Json, body.Type);
	}
	
	[Fact]
	public async Task JsonRequestBody_SerializesTheComplexPayload()
	{
		var objectBody = new JsonRequestBody(new { name = "Jane" });
		var stringBody = new JsonRequestBody("""{ "a": 1 }""");
		var nullBody = new JsonRequestBody(null!);
		
		Assert.Equal("""{"name":"Jane"}""", (await ReadAsync(objectBody)).Content);
		Assert.Equal("""{ "a": 1 }""", (await ReadAsync(stringBody)).Content);
		Assert.Equal(string.Empty, (await ReadAsync(nullBody)).Content);
		Assert.Equal(BodyTypes.Json, objectBody.Type);
	}
	
	[Fact]
	public async Task JsonRequestBody_WithAStringPayload_SendsItAsItIs()
	{
		var (content, _, _) = await ReadAsync(new JsonRequestBody("""{ "where": { "a": 1 } }"""));
		
		Assert.Equal("""{ "where": { "a": 1 } }""", content);
	}
	
	[Fact]
	public async Task JsonRequestBody_WithANullPayload_SendsAnEmptyContent()
	{
		var body = new JsonRequestBody(null!);
		
		var (content, contentType, _) = await ReadAsync(body);
		
		Assert.Null(body.Json);
		Assert.Equal(string.Empty, content);
		Assert.Equal("text/plain", contentType);
	}
	
	#endregion
	
	#region Other Body Methods
	
	[Fact]
	public async Task TextRequestBody_SendsTheText()
	{
		Assert.Equal(("hello", "text/plain", null), await ReadAsync(new TextRequestBody("hello")));
		Assert.Equal(("<b/>", "text/html", null), await ReadAsync(new TextRequestBody("<b/>") { ContentType = "text/html" }));
		Assert.Equal(string.Empty, (await ReadAsync(new TextRequestBody(string.Empty))).Content);
		Assert.Equal("hello", new TextRequestBody("hello").Payload);
		Assert.Equal(BodyTypes.Text, new TextRequestBody("x").Type);
	}
	
	[Fact]
	public async Task RawRequestBody_SendsTheBytes()
	{
		// ReSharper disable once UseUtf8StringLiteral
		var body = new RawRequestBody(Encoding.UTF8.GetBytes("çay"), BodyTypes.Binary, "application/octet-stream", "utf-8");
		
		Assert.Equal(("çay", "application/octet-stream", "utf-8"), await ReadAsync(body));
		Assert.Equal(BodyTypes.Binary, body.Type);
		Assert.Equal(string.Empty, (await ReadAsync(new RawRequestBody(null!, BodyTypes.Binary, "application/octet-stream"))).Content);
	}
	
	[Fact]
	public async Task XFormUrlEncodedBody_EncodesTheFields()
	{
		var body = new XFormUrlEncodedBody(new Dictionary<string, string> { ["grant_type"] = "password", ["username"] = "a b&c" });
		
		var (content, contentType, _) = await ReadAsync(body);
		
		Assert.Equal("grant_type=password&username=a+b%26c", content);
		Assert.Equal("application/x-www-form-urlencoded", contentType);
		Assert.Equal(BodyTypes.UrlEncoded, body.Type);
		Assert.NotNull(body.Payload);
	}
	
	[Fact]
	public async Task XmlRequestBody_SerializesThePayload()
	{
		var body = new XmlRequestBody(new Item { Name = "çay" });
		
		var (content, contentType, _) = await ReadAsync(body);
		
		Assert.Contains("<Name>çay</Name>", content);
		Assert.Equal("application/xml", contentType);
		Assert.Equal(BodyTypes.Xml, body.Type);
		Assert.Null(new XmlRequestBody(null!).Xml);
		Assert.Equal(string.Empty, (await ReadAsync(new XmlRequestBody(null!))).Content);
	}
	
	[Fact]
	public async Task XmlRequestBody_DeclaresTheSentEncoding()
	{
		var (content, _, _) = await ReadAsync(new XmlRequestBody(new Item { Name = "çay" }));
		
		Assert.StartsWith("""<?xml version="1.0" encoding="utf-8"?>""", content);
	}
	
	#endregion
	
	#region Test Types
	
	// ReSharper disable once MemberCanBePrivate.Global
	public sealed class Item
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public string? Name { get; set; }
	}
	
	#endregion
}
