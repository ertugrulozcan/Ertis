using System.Dynamic;
using Ertis.Schema.Extensions;

namespace Ertis.Schema.Tests.Extensions;

public class ExpandoObjectExtensionsTests
{
	#region Methods
	
	[Fact]
	public void ToExpandoObject_ReadsTheProperties()
	{
		var expando = new { Name = "Jane", Address = new { City = "Istanbul" } }.ToExpandoObject();
		
		Assert.Equal("Jane", expando.GetProperty<string>("Name"));
		Assert.Equal("Istanbul", expando.GetProperty<string>("Address.City"));
	}
	
	[Theory]
	[InlineData("Missing")]
	[InlineData("Missing.City")]
	public void GetProperty_WithUndefinedPath_ReturnsDefault(string path)
	{
		var expando = new { Name = "Jane" }.ToExpandoObject();
		
		Assert.Null(expando.GetProperty<string>(path));
	}
	
	[Fact]
	public void GetProperty_WithEmptyPath_Throws()
	{
		var expando = new { Name = "Jane" }.ToExpandoObject();
		
		Assert.Throws<ArgumentNullException>(() => expando.GetProperty<string>(string.Empty));
	}
	
	[Fact]
	public void SetProperty_SetsTopLevelAndNestedProperties()
	{
		var expando = new { Name = "Jane", Address = new { City = "Istanbul" } }.ToExpandoObject();
		
		expando = expando.SetProperty("Name", "John");
		expando = expando.SetProperty("Age", 30);
		expando = expando.SetProperty("Address.City", "Ankara");
		
		Assert.Equal("John", expando.GetProperty<string>("Name"));
		Assert.Equal(30, expando.GetProperty<int>("Age"));
		Assert.Equal("Ankara", expando.GetProperty<string>("Address.City"));
	}
	
	[Fact]
	public void RemoveProperty_RemovesTheProperty()
	{
		var expando = new { Name = "Jane", Age = 30 }.ToExpandoObject();
		
		expando = expando.RemoveProperty("Age");
		
		Assert.False(((IDictionary<string, object?>) expando).ContainsKey("Age"));
		Assert.Equal("Jane", expando.GetProperty<string>("Name"));
	}
	
	[Fact]
	public void Clone_CreatesAnIndependentCopy()
	{
		var original = new { Name = "Jane", Address = new { City = "Istanbul" } }.ToExpandoObject();
		
		var clone = original.Clone().SetProperty("Address.City", "Ankara");
		
		Assert.Equal("Istanbul", original.GetProperty<string>("Address.City"));
		Assert.Equal("Ankara", clone.GetProperty<string>("Address.City"));
		Assert.IsType<ExpandoObject>(clone);
	}
	
	#endregion
}
