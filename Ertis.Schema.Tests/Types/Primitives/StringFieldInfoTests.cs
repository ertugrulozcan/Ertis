using Ertis.Schema.Dynamics;
using Ertis.Schema.Extensions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Tests.Types.Primitives;

public class StringFieldInfoTests
{
	#region Methods
	
	[Theory]
	[InlineData("abc", true)]
	[InlineData("abcde", true)]
	[InlineData("abcdef", false)]
	public void ValidateData_WithMaxLength_ValidatesTheLength(string value, bool expected)
	{
		var schema = new ObjectFieldInfo([new StringFieldInfo { Name = "title", MaxLength = 5 }])
		{
			Name = "root"
		};
		
		var content = DynamicObject.Create(new Dictionary<string, object?> { ["title"] = value });
		var validationContext = new FieldValidationContext(content);
		
		var isValid = schema.ValidateData(content, validationContext);
		
		Assert.Equal(expected, isValid);
		Assert.Equal(expected, validationContext.Errors.Count == 0);
	}
	
	#endregion
}
