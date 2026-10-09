using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.CustomTypes;

namespace Ertis.Schema.Tests.Types.CustomTypes;

public class ReferenceFieldInfoTests
{
	#region Single Reference Methods
	
	[Theory]
	[InlineData("\"65a0f0c2e4b0a1b2c3d4e5f6\"")]
	[InlineData("{ \"_id\": \"65a0f0c2e4b0a1b2c3d4e5f6\", \"title\": \"Post\" }")]
	public void Validate_SingleWithAnId_Succeeds(string jsonValue)
	{
		var fieldInfo = new ReferenceFieldInfo { Name = "author", ReferenceType = ReferenceFieldInfo.ReferenceTypes.single };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "author": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("{ \"title\": \"Post\" }")]
	[InlineData("5")]
	public void Validate_SingleWithoutAnId_Fails(string jsonValue)
	{
		var fieldInfo = new ReferenceFieldInfo { Name = "author", ReferenceType = ReferenceFieldInfo.ReferenceTypes.single };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "author": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["The reference field [author] has no _id field"], result.Messages);
	}
	
	#endregion
	
	#region Multiple Reference Methods
	
	[Fact]
	public void Validate_MultipleWithIds_Succeeds()
	{
		var fieldInfo = new ReferenceFieldInfo { Name = "tags", ReferenceType = ReferenceFieldInfo.ReferenceTypes.multiple };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": ["a", { "_id": "b" }] }""");
		
		Assert.True(result.IsValid);
	}
	
	[Fact]
	public void Validate_MultipleWithItemsWithoutId_ReportsEachItem()
	{
		var fieldInfo = new ReferenceFieldInfo { Name = "tags", ReferenceType = ReferenceFieldInfo.ReferenceTypes.multiple };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "tags": ["a", { "title": "b" }, { "_id": "c" }, 5] }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["The reference field [tags] item has no _id field", "The reference field [tags] item has no _id field"], result.Messages);
		Assert.Equal(["test-schema.tags[1]", "test-schema.tags[3]"], result.Errors.Select(x => x.FieldPath));
	}
	
	[Theory]
	[InlineData("[\"a\"]", "Multiple reference array length can not be less than 2")]
	[InlineData("[\"a\", \"b\", \"c\", \"d\"]", "Multiple reference array length can not be greater than 3")]
	public void Validate_MultipleWithCountOutOfTheBounds_Fails(string jsonValue, string expectedMessage)
	{
		var fieldInfo = new ReferenceFieldInfo
		{
			Name = "tags",
			ReferenceType = ReferenceFieldInfo.ReferenceTypes.multiple,
			MultipleReferenceOptions = new MultipleReferenceOptions { MinCount = 2, MaxCount = 3 }
		};
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "tags": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([expectedMessage], result.Messages);
	}
	
	#endregion
}
