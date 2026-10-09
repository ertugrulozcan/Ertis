using System.Text.Json;
using Ertis.Schema.Serialization;
using Ertis.Schema.Tests.TestHelpers;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.Types;

public class FieldInfoCloneTests
{
	#region Fields
	
	private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
	{
		Converters = { new FieldInfoJsonConverter() }
	};
	
	#endregion
	
	#region Methods
	
	public static TheoryData<string> FieldNames()
	{
		return new TheoryData<string>(SampleFields.All().Select(x => x.Name));
	}
	
	[Theory]
	[MemberData(nameof(FieldNames))]
	public void Clone_CreatesAnEqualCopy(string fieldName)
	{
		var original = SampleFields.All().Single(x => x.Name == fieldName);
		
		var clone = (IFieldInfo) original.Clone();
		
		Assert.NotSame(original, clone);
		Assert.IsType(original.GetType(), clone);
		Assert.Equal(JsonSerializer.Serialize(original, Options), JsonSerializer.Serialize(clone, Options));
	}
	
	[Fact]
	public void Clone_OfArray_ClonesTheItemSchema()
	{
		var original = new ArrayFieldInfo { Name = "tags", ItemSchema = new StringFieldInfo { Name = "$schema" } };
		
		var clone = (ArrayFieldInfo) original.Clone();
		
		Assert.NotSame(original.ItemSchema, clone.ItemSchema);
		Assert.Same(clone, clone.ItemSchema?.Parent);
	}
	
	[Fact]
	public void Clone_OfObject_ClonesTheProperties()
	{
		var original = new ObjectFieldInfo([new StringFieldInfo { Name = "city" }]) { Name = "address" };
		
		var clone = (ObjectFieldInfo) original.Clone();
		
		Assert.NotSame(original.Properties.Single(), clone.Properties.Single());
		Assert.Same(clone, clone.Properties.Single().Parent);
	}
	
	#endregion
}
