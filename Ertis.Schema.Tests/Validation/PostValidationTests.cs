using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Validation;

/// <summary>
/// The post validation steps (default values, constants, format patterns, dates) and their order:
/// every step is applied to all the fields before the next one (defaults -> constants -> format patterns -> dates)
/// </summary>
public class PostValidationTests
{
	#region Methods
	
	[Fact]
	public void FormatPattern_UsesADefaultValueDeclaredAfterIt()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "fullname", FormatPattern = "{title} {firstname}" },
			new StringFieldInfo { Name = "firstname" },
			new StringFieldInfo { Name = "title", DefaultValue = "Dr." });
		
		var result = SchemaValidation.Validate(schema, """{ "firstname": "Jane" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("Dr. Jane", result.Content.GetValue("fullname"));
	}
	
	[Fact]
	public void FormatPattern_UsesAConstantDeclaredAfterIt()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "label", FormatPattern = "{kind}-{firstname}" },
			new StringFieldInfo { Name = "firstname" },
			new ConstantFieldInfo { Name = "kind", ValueType = ConstantFieldInfo.ConstantType.@string, Value = "member" });
		
		var result = SchemaValidation.Validate(schema, """{ "firstname": "Jane", "kind": "admin" }""");
		
		Assert.True(result.IsValid);
		Assert.Equal("member-Jane", result.Content.GetValue("label"));
	}
	
	[Fact]
	public void FormatPattern_UsesTheSentStringOfADateField()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "label", FormatPattern = "{startsAt}" },
			new DateTimeFieldInfo { Name = "startsAt" });
		
		var result = SchemaValidation.Validate(schema, """{ "startsAt": "2026-01-31T10:00:00+03:00" }""");
		
		Assert.Equal("2026-01-31T10:00:00+03:00", result.Content.GetValue("label"));
		Assert.Equal(new DateTime(2026, 1, 31, 7, 0, 0, DateTimeKind.Utc), result.Content.GetValue("startsAt"));
	}
	
	[Fact]
	public void DefaultValue_OfADateField_IsConvertedToDateTime()
	{
		var schema = TestSchema.Of(new DateFieldInfo { Name = "since", DefaultValue = "2026-01-01" });
		
		var result = SchemaValidation.Validate(schema, "{}");
		
		Assert.Equal(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Content.GetValue("since"));
	}
	
	[Fact]
	public void FormatPattern_InANestedObject_UsesTheNestedDefault()
	{
		var schema = TestSchema.Of(new ObjectFieldInfo([
			new StringFieldInfo { Name = "label", FormatPattern = "{address.city}/{address.country}" },
			new StringFieldInfo { Name = "city" },
			new StringFieldInfo { Name = "country", DefaultValue = "TR" }
		])
		{
			Name = "address"
		});
		
		var result = SchemaValidation.Validate(schema, """{ "address": { "city": "Istanbul" } }""");
		
		Assert.Equal("Istanbul/TR", result.Content.GetValue("address.label"));
	}
	
	[Fact]
	public void FormatPattern_WithMissingValue_KeepsTheSentValue()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "firstname" },
			new StringFieldInfo { Name = "label", FormatPattern = "{firstname}-{missing}" });
		
		var result = SchemaValidation.Validate(schema, """{ "firstname": "Jane", "label": "sent" }""");
		
		Assert.Equal("sent", result.Content.GetValue("label"));
	}
	
	[Fact]
	public void DefaultValue_DoesNotOverrideASentValue()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title", DefaultValue = "Dr." });
		
		var result = SchemaValidation.Validate(schema, """{ "title": "Prof." }""");
		
		Assert.Equal("Prof.", result.Content.GetValue("title"));
	}
	
	#endregion
}
