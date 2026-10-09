using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.CustomTypes;

namespace Ertis.Schema.Tests.Types.CustomTypes;

/// <summary>
/// email, uri, hostname, color, longtext and richtext fields (string fields with a format rule)
/// </summary>
public class TextFormatFieldInfoTests
{
	#region Email Methods
	
	[Theory]
	[InlineData("jane@example.com")]
	[InlineData("jane.doe+tag@sub.example.co")]
	[InlineData("jane@münchen.de")]
	public void Validate_ValidEmailAddress_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new EmailAddressFieldInfo { Name = "email" }, $$"""{ "email": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("jane")]
	[InlineData("jane@")]
	[InlineData("jane@example")]
	[InlineData("jane doe@example.com")]
	public void Validate_InvalidEmailAddress_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new EmailAddressFieldInfo { Name = "email" }, $$"""{ "email": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Email address is not valid"], result.Messages);
	}
	
	#endregion
	
	#region Uri Methods
	
	[Theory]
	[InlineData("https://example.com")]
	[InlineData("https://example.com/path?q=1#top")]
	[InlineData("ftp://files.example.com/a.txt")]
	[InlineData("mailto:jane@example.com")]
	[InlineData("HTTPS://EXAMPLE.COM")]
	public void Validate_ValidUri_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new UriFieldInfo { Name = "website" }, $$"""{ "website": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("example.com")]
	[InlineData("/relative/path")]
	[InlineData("not a uri")]
	[InlineData("javascript:alert(1)")]
	[InlineData("JavaScript:alert(1)")]
	[InlineData("vbscript:msgbox(1)")]
	[InlineData("data:text/plain,hello")]
	[InlineData("file:///etc/passwd")]
	public void Validate_InvalidUri_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new UriFieldInfo { Name = "website" }, $$"""{ "website": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Uri is not valid"], result.Messages);
	}
	
	#endregion
	
	#region Hostname Methods
	
	[Theory]
	[InlineData("example.com")]
	[InlineData("api.example.com")]
	[InlineData("my-host.example.co.uk")]
	[InlineData("münchen.de")]
	[InlineData("xn--mnchen-3ya.de")]
	[InlineData("192.168.1.1")]
	public void Validate_ValidHostName_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new HostNameFieldInfo { Name = "host" }, $$"""{ "host": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("localhost")]
	[InlineData("exa mple.com")]
	[InlineData("example.com/path")]
	[InlineData("example.com:8080")]
	[InlineData("https://example.com")]
	[InlineData("exa_mple.com")]
	[InlineData("-bad-.com")]
	[InlineData("bad-.com")]
	[InlineData("example.com.")]
	[InlineData("a..b.com")]
	[InlineData("::1")]
	[InlineData("256.1.1.1")]
	[InlineData("1.2.3")]
	public void Validate_InvalidHostName_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new HostNameFieldInfo { Name = "host" }, $$"""{ "host": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Hostname is not valid"], result.Messages);
	}
	
	#endregion
	
	#region Color Methods
	
	[Theory]
	[InlineData("#fff")]
	[InlineData("#FFF")]
	[InlineData("#ffff")]
	[InlineData("#a1b2c3")]
	[InlineData("#A1B2C3")]
	[InlineData("#a1b2c380")]
	[InlineData("0xabc")]
	[InlineData("RGB(255, 0, 0)")]
	[InlineData("hsla(120, 100%, 50%, 0.3)")]
	[InlineData("0xa1b2c3")]
	[InlineData("rgb(0, 0, 0)")]
	[InlineData("rgba(0, 0, 0, 0.5)")]
	[InlineData("hsl(120, 100%, 50%)")]
	public void Validate_ValidColor_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new ColorFieldInfo { Name = "color" }, $$"""{ "color": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("red")]
	[InlineData("#ggg")]
	[InlineData("#fffff")]
	[InlineData("#fff garbage")]
	[InlineData("x#abc")]
	[InlineData("rgb(hello)")]
	[InlineData("rgb(1, 2)")]
	public void Validate_InvalidColor_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new ColorFieldInfo { Name = "color" }, $$"""{ "color": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Color code is not valid"], result.Messages);
	}
	
	[Fact]
	public void Validate_ColorWithAStoredRegexPattern_IgnoresThePattern()
	{
		var options = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web) { Converters = { new Ertis.Schema.Serialization.FieldInfoJsonConverter() } };
		const string storedField = """{ "name": "color", "type": "color", "regexPattern": "(?:#|0x)(?:[a-f0-9]{3}|[a-f0-9]{6})\\b|(?:rgb|hsl)a?\\([^\\)]*\\)" }""";
		var fieldInfo = System.Text.Json.JsonSerializer.Deserialize<Ertis.Schema.Types.IFieldInfo>(storedField, options)!;
		
		Assert.True(SchemaValidation.ValidateField(fieldInfo, """{ "color": "#FFF" }""").IsValid);
		Assert.False(SchemaValidation.ValidateField(fieldInfo, """{ "color": "#fff garbage" }""").IsValid);
	}
	
	[Fact]
	public void Serialize_Color_DoesNotWriteARegexPattern()
	{
		var options = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web) { Converters = { new Ertis.Schema.Serialization.FieldInfoJsonConverter() } };
		
		var json = System.Text.Json.JsonSerializer.Serialize<Ertis.Schema.Types.IFieldInfo>(new ColorFieldInfo { Name = "color" }, options);
		
		Assert.Equal("""{"type":"color","name":"color"}""", json);
	}
	
	#endregion
	
	#region Long Text Methods
	
	[Fact]
	public void Validate_LongTextWithMaxLength_ValidatesTheLength()
	{
		var fieldInfo = new LongTextFieldInfo { Name = "bio", MaxLength = 10 };
		
		Assert.True(SchemaValidation.ValidateField(fieldInfo, """{ "bio": "short" }""").IsValid);
		Assert.False(SchemaValidation.ValidateField(fieldInfo, """{ "bio": "this text is too long" }""").IsValid);
	}
	
	[Fact]
	public void Validate_RichText_AcceptsHtml()
	{
		var result = SchemaValidation.ValidateField(new RichTextFieldInfo { Name = "body" }, """{ "body": "<p>Hello <b>world</b></p>" }""");
		
		Assert.True(result.IsValid);
	}
	
	#endregion
}
