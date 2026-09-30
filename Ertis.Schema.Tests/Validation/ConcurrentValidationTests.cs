using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Validation;

/// <summary>
/// The consumers cache their schemas (e.g. ErtisAuth caches the user types) and validate concurrent requests with the same instance
/// </summary>
public class ConcurrentValidationTests
{
	#region Methods
	
	[Fact]
	public void Validate_SequentialContentsWithTheSameSchema_KeepTheirOwnValues()
	{
		var schema = CreateSchema();
		
		var first = SchemaValidation.Validate(schema, """{ "firstname": "Jane", "lastname": "Doe" }""");
		var second = SchemaValidation.Validate(schema, """{ "firstname": "John" }""");
		
		Assert.Equal("Jane", first.Content.GetValue("firstname"));
		Assert.Equal("Doe", first.Content.GetValue("lastname"));
		Assert.Equal("John", second.Content.GetValue("firstname"));
		Assert.False(second.Content.ContainsProperty("lastname"));
	}
	
	[Fact]
	public void Validate_ConcurrentContentsWithTheSameSchema_KeepTheirOwnValues()
	{
		var schema = CreateSchema();
		var mismatches = 0;
		
		Parallel.For(0, 20_000, i =>
		{
			var result = SchemaValidation.Validate(schema, $$"""{ "firstname": "first-{{i}}", "lastname": "last-{{i}}" }""");
			if (!Equals(result.Content.GetValue("firstname"), $"first-{i}") || !Equals(result.Content.GetValue("lastname"), $"last-{i}"))
			{
				Interlocked.Increment(ref mismatches);
			}
		});
		
		Assert.Equal(0, mismatches);
	}
	
	private static TestSchema CreateSchema()
	{
		return TestSchema.Of(
			new StringFieldInfo { Name = "firstname", IsRequired = true },
			new StringFieldInfo { Name = "lastname" });
	}
	
	#endregion
}
