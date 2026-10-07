using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class BooleanFieldInfoTests
{
	#region Data Validation Methods
	
	[Theory]
	[InlineData("true")]
	[InlineData("false")]
	public void Validate_WithBoolean_Succeeds(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new BooleanFieldInfo { Name = "active" }, $$"""{ "active": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData("\"true\"")]
	[InlineData("1")]
	[InlineData("0")]
	public void Validate_WithNonBoolean_FailsWithTypeMismatch(string jsonValue)
	{
		var result = SchemaValidation.ValidateField(new BooleanFieldInfo { Name = "active" }, $$"""{ "active": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal(["Type mismatch error. 'active' is must be 'boolean'"], result.Messages);
	}
	
	[Fact]
	public void Validate_MissingWithDefault_SetsTheDefault()
	{
		var result = SchemaValidation.ValidateField(new BooleanFieldInfo { Name = "active", DefaultValue = true }, "{}");
		
		Assert.True(result.IsValid);
		Assert.Equal(true, result.Content.GetValue("active"));
	}
	
	#endregion
}
