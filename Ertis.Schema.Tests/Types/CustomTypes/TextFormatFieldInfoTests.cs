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
	public void Validate_ValidUri_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new UriFieldInfo { Name = "website" }, $$"""{ "website": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("example.com")]
	[InlineData("/relative/path")]
	[InlineData("not a uri")]
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
	public void Validate_ValidHostName_Succeeds(string value)
	{
		var result = SchemaValidation.ValidateField(new HostNameFieldInfo { Name = "host" }, $$"""{ "host": "{{value}}" }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("localhost")]
	[InlineData("exa mple.com")]
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
	[InlineData("#a1b2c3")]
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
	public void Validate_InvalidColor_Fails(string value)
	{
		var result = SchemaValidation.ValidateField(new ColorFieldInfo { Name = "color" }, $$"""{ "color": "{{value}}" }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Color code is not valid"], result.Messages);
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
