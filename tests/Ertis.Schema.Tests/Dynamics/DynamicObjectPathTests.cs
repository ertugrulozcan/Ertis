using Ertis.Schema.Exceptions;

using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Tests.Dynamics;

/// <summary>
/// The path resolution of DynamicObject (GetValue, SetValue, TryGetValue, ContainsProperty, RemoveProperty) on the edge cases
/// </summary>
public class DynamicObjectPathTests
{
	#region Constants
	
	private const string SAMPLE_JSON =
		"""
		{
			"name": "Jane",
			"address": { "city": "Istanbul" },
			"profile": { "tags": ["x", "y"] },
			"tags": ["a", "b"],
			"matrix": [[1, 2], [3, 4]],
			"phones": [{ "number": "555" }]
		}
		""";
	
	#endregion
	
	#region GetValue Methods
	
	[Theory]
	[InlineData("address..city")]
	[InlineData(".name")]
	[InlineData("name.")]
	public void GetValue_WithEmptySegment_ThrowsUndefinedFieldException(string path)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<UndefinedFieldException>(() => dynamicObject.GetValue(path));
	}
	
	[Fact]
	public void GetValue_WithTrailingDotOnObject_ThrowsArgumentException()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<ArgumentException>(() => dynamicObject.GetValue("address."));
	}
	
	[Fact]
	public void GetValue_WithEmptyIndexer_ThrowsInvalidOperationException()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		var exception = Assert.Throws<InvalidOperationException>(() => dynamicObject.GetValue("tags[]"));
		
		Assert.Equal("Array index is not valid integer ('')", exception.Message);
	}
	
	[Theory]
	[InlineData("tags[-1]", "Out of range (length: 2, index: -1)")]
	[InlineData("matrix[0][2]", "Out of range (length: 2, index: 2)")]
	[InlineData("matrix[0][x]", "Array index is not valid integer ('x')")]
	[InlineData("tags[0][0]", "Indexed node is not an array")]
	public void GetValue_WithInvalidIndexer_ThrowsInvalidOperationException(string path, string expectedMessage)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		var exception = Assert.Throws<InvalidOperationException>(() => dynamicObject.GetValue(path));
		
		Assert.Equal(expectedMessage, exception.Message);
	}
	
	[Theory]
	[InlineData("matrix[1][0]", 3L)]
	[InlineData("matrix[0][1]", 2L)]
	public void GetValue_WithNestedIndexers_ReturnsTheItem(string path, object expected)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal(expected, dynamicObject.GetValue(path));
	}
	
	[Fact]
	public void GetValue_WithIndexerInANestedObject_ReturnsTheItem()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal("y", dynamicObject.GetValue("profile.tags[1]"));
	}
	
	[Fact]
	public void GetValue_WithDefault_ReturnsTheDefaultOnlyForUndefinedFields()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Equal("none", dynamicObject.GetValue("address.street", (object) "none"));
		Assert.Throws<InvalidOperationException>(() => dynamicObject.GetValue("tags[9]", (object) "none"));
	}
	
	[Fact]
	public void TryGetValue_WithInvalidIndexer_ReturnsFalse()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.False(dynamicObject.TryGetValue("tags[9]", out _, out var exception));
		Assert.IsType<InvalidOperationException>(exception);
		Assert.False(dynamicObject.TryGetValue<string>("tags[9]", out _));
	}
	
	[Fact]
	public void ContainsProperty_WithInvalidIndexer_RethrowsTheException()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<InvalidOperationException>(() => dynamicObject.ContainsProperty("tags[9]"));
	}
	
	#endregion
	
	#region SetValue & RemoveProperty Methods
	
	[Fact]
	public void SetValue_WithIndexerInANestedObject_ReplacesTheItem()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.SetValue("profile.tags[1]", "z");
		
		Assert.Equal("z", dynamicObject.GetValue("profile.tags[1]"));
		Assert.Equal("x", dynamicObject.GetValue("profile.tags[0]"));
	}
	
	[Fact]
	public void SetValue_WithNestedIndexers_ReplacesTheItem()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.SetValue("matrix[1][0]", 9L);
		
		Assert.Equal(9L, dynamicObject.GetValue("matrix[1][0]"));
		Assert.Equal(4L, dynamicObject.GetValue("matrix[1][1]"));
	}
	
	[Theory]
	[InlineData("tags[-1]")]
	[InlineData("matrix[1][5]")]
	public void SetValue_WithInvalidIndex_ThrowsInvalidOperationException(string path)
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<InvalidOperationException>(() => dynamicObject.SetValue(path, "z"));
	}
	
	[Fact]
	public void SetValue_WithCreateIfNotExistAndAnIndexerInAMissingParent_Throws()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<UndefinedFieldException>(() => dynamicObject.SetValue("missing.items[0]", "z", createIfNotExist: true));
		Assert.False(dynamicObject.ContainsProperty("missing"));
	}
	
	[Fact]
	public void SetValue_WithIndexerOutOfRange_Throws()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<InvalidOperationException>(() => dynamicObject.SetValue("tags[5]", "z"));
		Assert.False(dynamicObject.TrySetValue("tags[5]", "z", out var exception));
		Assert.IsType<InvalidOperationException>(exception);
	}
	
	[Fact]
	public void SetValue_OnAPrimitiveParent_ThrowsUndefinedFieldException()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<UndefinedFieldException>(() => dynamicObject.SetValue("name.first", "x", createIfNotExist: true));
	}
	
	[Fact]
	public void RemoveProperty_WithIndexer_DoesNothing()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.RemoveProperty("tags[0]");
		dynamicObject.RemoveProperty("phones[0].number");
		
		Assert.Equal(2, ((object[]) dynamicObject.GetValue("tags")!).Length);
		Assert.Equal("555", dynamicObject.GetValue("phones[0].number"));
	}
	
	[Fact]
	public void RemoveProperty_WithEmptyPath_Throws()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		Assert.Throws<ArgumentException>(() => dynamicObject.RemoveProperty(string.Empty));
	}
	
	#endregion
	
	#region Dispose Methods
	
	[Fact]
	public void Dispose_ClearsTheProperties()
	{
		var dynamicObject = DynamicObject.Parse(SAMPLE_JSON);
		
		dynamicObject.Dispose();
		
		Assert.Empty(dynamicObject.ToDictionary());
	}
	
	#endregion
}
