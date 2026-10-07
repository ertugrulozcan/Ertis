using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types.Primitives;

public class ConstantFieldInfoTests
{
	#region Data Validation Methods
	
	[Theory]
	[InlineData(ConstantFieldInfo.ConstantType.@string, "\"user\"")]
	[InlineData(ConstantFieldInfo.ConstantType.integer, "5")]
	[InlineData(ConstantFieldInfo.ConstantType.@float, "0.5")]
	[InlineData(ConstantFieldInfo.ConstantType.boolean, "true")]
	public void Validate_WithCompatibleValue_Succeeds(ConstantFieldInfo.ConstantType valueType, string jsonValue)
	{
		var fieldInfo = new ConstantFieldInfo { Name = "kind", ValueType = valueType };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "kind": {{jsonValue}} }""");
		
		Assert.True(result.IsValid);
	}
	
	[Theory]
	[InlineData(ConstantFieldInfo.ConstantType.@string, "5")]
	[InlineData(ConstantFieldInfo.ConstantType.integer, "\"abc\"")]
	[InlineData(ConstantFieldInfo.ConstantType.boolean, "\"yes\"")]
	public void Validate_WithIncompatibleValue_Fails(ConstantFieldInfo.ConstantType valueType, string jsonValue)
	{
		var fieldInfo = new ConstantFieldInfo { Name = "kind", ValueType = valueType };
		
		var result = SchemaValidation.ValidateField(fieldInfo, $$"""{ "kind": {{jsonValue}} }""");
		
		Assert.False(result.IsValid);
		Assert.Equal([$"Constant value is must be {valueType}"], result.Messages);
	}
	
	[Fact]
	public void Validate_SetsTheConstantValue()
	{
		var fieldInfo = new ConstantFieldInfo { Name = "kind", ValueType = ConstantFieldInfo.ConstantType.@string, Value = "user" };
		
		var result = SchemaValidation.ValidateField(fieldInfo, "{}");
		
		Assert.True(result.IsValid);
		Assert.Equal("user", result.Content.GetValue("kind"));
	}
	
	[Fact]
	public void Validate_OverridesTheSentValue()
	{
		var fieldInfo = new ConstantFieldInfo { Name = "kind", ValueType = ConstantFieldInfo.ConstantType.@string, Value = "user" };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "kind": "admin" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("user", result.Content.GetValue("kind"));
	}
	
	[Fact]
	public void Validate_NestedConstant_SetsTheConstantValue()
	{
		var fieldInfo = new ObjectFieldInfo([new ConstantFieldInfo { Name = "version", ValueType = ConstantFieldInfo.ConstantType.integer, Value = 2 }]) { Name = "meta" };
		
		var result = SchemaValidation.ValidateField(fieldInfo, """{ "meta": {} }""");
		
		Assert.True(result.IsValid);
		Assert.Equal(2, result.Content.GetValue("meta.version"));
	}
	
	#endregion
}
