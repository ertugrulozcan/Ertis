using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Extensions;

public class SchemaExtensionsTests
{
	#region Schema Validation Methods
	
	[Fact]
	public void Validate_WithValidSchema_Succeeds()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" }, CreateAddress());
		
		schema.Validate(out var exception);
		
		Assert.Null(exception);
	}
	
	[Fact]
	public void Validate_WithDuplicatePropertyNames_Fails()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" }, new IntegerFieldInfo { Name = "title" });
		
		schema.Validate(out var exception);
		
		Assert.IsType<SchemaValidationException>(exception);
		Assert.Equal("Duplicate property declaration in schema. Property names are must be unique.", exception.Message);
	}
	
	[Fact]
	public void Create_ArrayWithUniqueFieldInItems_Throws()
	{
		var exception = Assert.Throws<SchemaValidationException>(() => new ArrayFieldInfo
		{
			Name = "phones",
			ItemSchema = new ObjectFieldInfo([new StringFieldInfo { Name = "number", IsUnique = true }]) { Name = "$schema" }
		});
		
		Assert.Equal("The unique constraints could not use in arrays. Use the 'uniqueBy' feature instead of. ('number')", exception.Message);
	}
	
	#endregion
	
	#region Merge Methods
	
	[Fact]
	public void MergeTypeProperties_AppendsTheSubTypeProperties()
	{
		var baseType = TestSchema.Of(new StringFieldInfo { Name = "a" }, new StringFieldInfo { Name = "b" });
		var subType = TestSchema.Of(new StringFieldInfo { Name = "c" });
		
		var properties = baseType.MergeTypeProperties(subType);
		
		Assert.Equal(["a", "b", "c"], properties.Select(x => x.Name));
	}
	
	[Fact]
	public void MergeTypeProperties_WithDuplicateField_Throws()
	{
		var baseType = TestSchema.Of(new StringFieldInfo { Name = "a" });
		var subType = TestSchema.Of(new StringFieldInfo { Name = "a" });
		
		var exception = Assert.Throws<SchemaValidationException>(() => baseType.MergeTypeProperties(subType).ToList());
		
		Assert.Equal("'a' field is already exist in base type.", exception.Message);
	}
	
	[Fact]
	public void MergeTypeProperties_WithDuplicateFieldWhenAllowed_ReplacesTheBaseFieldInPlace()
	{
		var subField = new StringFieldInfo { Name = "a", MaxLength = 5 };
		var baseType = TestSchema.Of(new StringFieldInfo { Name = "a" }, new StringFieldInfo { Name = "b" });
		var subType = TestSchema.Of(subField);
		
		var properties = baseType.MergeTypeProperties(subType, allowDuplicateFieldWithBaseType: true).ToList();
		
		Assert.Equal(["a", "b"], properties.Select(x => x.Name));
		Assert.Same(subField, properties[0]);
	}
	
	[Fact]
	public void MergeTypeProperties_WithVirtualFieldOfTheSameType_KeepsTheBaseField()
	{
		var baseField = new StringFieldInfo { Name = "a" };
		var baseType = TestSchema.Of(baseField);
		var subType = TestSchema.Of(new StringFieldInfo { Name = "a", IsVirtual = true });
		
		var properties = baseType.MergeTypeProperties(subType).ToList();
		
		Assert.Same(baseField, Assert.Single(properties));
	}
	
	[Fact]
	public void MergeTypeProperties_WithVirtualFieldOfAnotherType_Throws()
	{
		var baseType = TestSchema.Of(new StringFieldInfo { Name = "a" });
		var subType = TestSchema.Of(new IntegerFieldInfo { Name = "a", IsVirtual = true });
		
		var exception = Assert.Throws<SchemaValidationException>(() => baseType.MergeTypeProperties(subType).ToList());
		
		Assert.Equal("The field type cannot be overwritten on virtual fields. (a)", exception.Message);
	}
	
	#endregion
	
	#region Schema Tree Methods
	
	[Theory]
	[InlineData("title", "title")]
	[InlineData("address.city", "city")]
	public void FindField_WithExistingPath_ReturnsTheField(string path, string expectedName)
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" }, CreateAddress());
		
		var fieldInfo = schema.FindField(path);
		
		Assert.NotNull(fieldInfo);
		Assert.Equal(expectedName, fieldInfo.Name);
	}
	
	[Theory]
	[InlineData("missing")]
	[InlineData("address")]
	[InlineData("address.street")]
	public void FindField_WithMissingOrNonLeafPath_ReturnsNull(string path)
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" }, CreateAddress());
		
		Assert.Null(schema.FindField(path));
	}
	
	[Fact]
	public void ValidateData_DoesNotChangeTheFieldPaths()
	{
		var schema = TestSchema.Of(new StringFieldInfo { Name = "title" }, CreateAddress());
		
		SchemaValidation.Validate(schema, """{ "title": "a" }""");
		
		Assert.NotNull(schema.FindField("address.city"));
		Assert.Equal("title", schema.Properties.First().Path);
	}
	
	[Fact]
	public void GetUniqueProperties_ReturnsNestedUniqueFields()
	{
		var schema = TestSchema.Of(
			new StringFieldInfo { Name = "email", IsUnique = true },
			new StringFieldInfo { Name = "title" },
			new ObjectFieldInfo([new IntegerFieldInfo { Name = "number", IsUnique = true }]) { Name = "identity" });
		
		var uniqueProperties = schema.GetUniqueProperties();
		
		Assert.Equal(["email", "number"], uniqueProperties.Select(x => x.Name));
	}
	
	[Fact]
	public void GetReferenceProperties_ReturnsNestedReferenceFields()
	{
		var schema = TestSchema.Of(
			new ReferenceFieldInfo { Name = "author", ReferenceType = ReferenceFieldInfo.ReferenceTypes.single },
			new ObjectFieldInfo([new ReferenceFieldInfo { Name = "category", ReferenceType = ReferenceFieldInfo.ReferenceTypes.single }]) { Name = "meta" });
		
		var referenceProperties = schema.GetReferenceProperties();
		
		Assert.Equal(["author", "category"], referenceProperties.Select(x => x.Name));
	}
	
	[Fact]
	public void GetSelfPath_RemovesTheSchemaSlug()
	{
		var city = new StringFieldInfo { Name = "city" };
		var schema = new ObjectFieldInfo([new ObjectFieldInfo([city]) { Name = "address" }]) { Name = "person" };
		
		Assert.Equal("person.address.city", city.Path);
		Assert.Equal("address.city", city.GetSelfPath(schema));
	}
	
	[Fact]
	public void GetSelfPath_OfAFieldNamedAsTheSchemaSlug_KeepsTheFieldName()
	{
		var city = new StringFieldInfo { Name = "city" };
		var schema = new TestSchema { Slug = "address", Properties = [new ObjectFieldInfo([city]) { Name = "address" }] };
		
		Assert.Equal("address.city", city.GetSelfPath(schema));
	}
	
	private static ObjectFieldInfo CreateAddress()
	{
		return new ObjectFieldInfo([new StringFieldInfo { Name = "city" }]) { Name = "address" };
	}
	
	#endregion
}
