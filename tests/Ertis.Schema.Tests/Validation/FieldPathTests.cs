using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Validation;

/// <summary>
/// The paths of the validation errors (FieldValidationException.FieldPath)
/// </summary>
public class FieldPathTests
{
	#region Methods
	
	[Theory]
	[InlineData("""{ "title": 5 }""", "test-schema.title")]
	[InlineData("""{ "address": { "city": 5 } }""", "test-schema.address.city")]
	[InlineData("""{ "scores": [1, "x", 3] }""", "test-schema.scores[1]")]
	[InlineData("""{ "phones": [{ "number": "1" }, {}] }""", "test-schema.phones[1].number")]
	[InlineData("""{ "phones": [{}, { "number": "1" }] }""", "test-schema.phones[0].number")]
	[InlineData("""{ "phones": [{ "number": "1", "tags": ["a", "b", 5] }] }""", "test-schema.phones[0].tags[2]")]
	public void Validate_InvalidValue_ReportsItsPath(string json, string expectedPath)
	{
		var result = SchemaValidation.Validate(CreateSchema(), json);
		
		Assert.False(result.IsValid);
		Assert.Equal(expectedPath, Assert.Single(result.Errors).FieldPath);
	}
	
	[Fact]
	public void Validate_WithObjectFieldInfoSchema_ReportsThePathFromTheSchemaName()
	{
		var schema = new ObjectFieldInfo([new ArrayFieldInfo { Name = "scores", ItemSchema = new IntegerFieldInfo { Name = "$schema" } }]) { Name = "person" };
		
		var result = SchemaValidation.Validate(schema, """{ "scores": [1, "x"] }""");
		
		Assert.Equal("person.scores[1]", Assert.Single(result.Errors).FieldPath);
	}
	
	[Fact]
	public void Validate_ConcurrentContents_ReportTheirOwnPaths()
	{
		var schema = CreateSchema();
		var mismatches = 0;
		
		Parallel.For(0, 5_000, i =>
		{
			var index = i % 7;
			var scores = string.Join(", ", Enumerable.Range(0, 7).Select(x => x == index ? "\"x\"" : x.ToString()));
			
			var result = SchemaValidation.Validate(schema, $$"""{ "scores": [{{scores}}] }""");
			if (result.Errors.Count != 1 || result.Errors[0].FieldPath != $"test-schema.scores[{index}]")
			{
				Interlocked.Increment(ref mismatches);
			}
		});
		
		Assert.Equal(0, mismatches);
	}
	
	[Fact]
	public void Path_OfArrayItemSchema_HasNoIndex()
	{
		var schema = CreateSchema();
		var phones = (ArrayFieldInfo) schema.Properties.Single(x => x.Name == "phones");
		
		SchemaValidation.Validate(schema, """{ "phones": [{ "number": "1" }, { "number": "2" }] }""");
		
		Assert.Equal("phones[]", phones.ItemSchema?.Path);
	}
	
	private static TestSchema CreateSchema()
	{
		return TestSchema.Of(
			new StringFieldInfo { Name = "title" },
			new ObjectFieldInfo([new StringFieldInfo { Name = "city" }]) { Name = "address" },
			new ArrayFieldInfo { Name = "scores", ItemSchema = new IntegerFieldInfo { Name = "$schema" } },
			new ArrayFieldInfo
			{
				Name = "phones",
				ItemSchema = new ObjectFieldInfo([
					new StringFieldInfo { Name = "number", IsRequired = true },
					new ArrayFieldInfo { Name = "tags", ItemSchema = new StringFieldInfo { Name = "$schema" } }
				])
				{
					Name = "$schema"
				}
			});
	}
	
	#endregion
}
