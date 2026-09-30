using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.CustomTypes;

namespace Ertis.Schema.Tests.Types.CustomTypes;

/// <summary>
/// location, code, image, video and json fields (fields with a predefined or free structure)
/// </summary>
public class StructuredFieldInfoTests
{
	#region Location Methods
	
	[Fact]
	public void Validate_ValidLocation_Succeeds()
	{
		var result = SchemaValidation.ValidateField(new LocationFieldInfo { Name = "position" }, """{ "position": { "latitude": 41.01, "longitude": 28.97 } }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory(Skip = "Bug (finding #11): the predefined properties of location/code/image/video fields are never validated (ObjectFieldInfoBase has no content validation)")]
	[InlineData("{ \"latitude\": 91.5, \"longitude\": 28.97 }")]
	[InlineData("{ \"latitude\": 41.01 }")]
	[InlineData("{ \"latitude\": \"41\", \"longitude\": 28.97 }")]
	[InlineData("\"Istanbul\"")]
	public void Validate_InvalidLocation_Fails(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new LocationFieldInfo { Name = "position" }, $$"""{ "position": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
	}
	
	#endregion
	
	#region Code Methods
	
	[Fact]
	public void Validate_ValidCode_Succeeds()
	{
		var result = SchemaValidation.ValidateField(new CodeFieldInfo { Name = "snippet" }, """{ "snippet": { "code": "let a = 1;", "language": "javascript" } }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact(Skip = "Bug (finding #11): the predefined properties of location/code/image/video fields are never validated (ObjectFieldInfoBase has no content validation)")]
	public void Validate_CodeWithoutLanguage_Fails()
	{
		var result = SchemaValidation.ValidateField(new CodeFieldInfo { Name = "snippet" }, """{ "snippet": { "code": "let a = 1;" } }""");
		
		Assert.False(result.IsValid);
	}
	
	#endregion
	
	#region Image & Video Methods
	
	[Fact]
	public void Validate_ValidImage_Succeeds()
	{
		var result = SchemaValidation.ValidateField(new ImageFieldInfo { Name = "photo" }, $$"""{ "photo": {{FileJson("image/png")}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData(1, "File count can not be less than 2")]
	[InlineData(4, "File count can not be greater than 3")]
	public void Validate_MultipleImagesWithCountOutOfTheBounds_Fails(int count, string expectedMessage)
	{
		var fieldInfo = new ImageFieldInfo { Name = "photos", Multiple = true, MinCount = 2, MaxCount = 3 };
		var files = string.Join(", ", Enumerable.Repeat(FileJson("image/png"), count));
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "photos": [{{files}}] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	[Fact]
	public void Validate_ValidVideo_Succeeds()
	{
		var result = SchemaValidation.ValidateField(new VideoFieldInfo { Name = "clip" }, $$"""{ "clip": {{FileJson("video/mp4")}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact(Skip = "Bug (finding #11): the predefined properties of location/code/image/video fields are never validated (ObjectFieldInfoBase has no content validation)")]
	public void Validate_VideoWithoutRequiredProperties_Fails()
	{
		var result = SchemaValidation.ValidateField(new VideoFieldInfo { Name = "clip" }, """{ "clip": { "name": "a.mp4" } }""");
		
		Assert.False(result.IsValid);
	}
	
	private static string FileJson(string mimeType)
	{
		return $$"""{ "id": "1", "name": "a", "path": "/a", "fullPath": "/files/a", "mimeType": "{{mimeType}}", "size": 1024, "url": "https://cdn.example.com/files/a" }""";
	}
	
	#endregion
	
	#region Json Methods
	
	[Theory]
	[InlineData("{ \"a\": { \"b\": [1, 2] } }")]
	[InlineData("[1, \"two\", { \"three\": 3 }]")]
	[InlineData("\"text\"")]
	[InlineData("42")]
	public void Validate_AnyJson_Succeeds(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new JsonFieldInfo { Name = "data" }, $$"""{ "data": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	#endregion
}
