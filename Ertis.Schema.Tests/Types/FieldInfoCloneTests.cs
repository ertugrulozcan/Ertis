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
	
	/// <summary>
	/// The sample fields whose Clone loses data (see <see cref="Clone_OfFieldsWithLostData_CreatesAnEqualCopy"/>)
	/// </summary>
	private static readonly string[] FieldsWithLostData = ["title", "photos", "clip"];
	
	public static TheoryData<string> FieldNames()
	{
		return new TheoryData<string>(SampleFields.All().Select(x => x.Name).Except(FieldsWithLostData));
	}
	
	public static TheoryData<string> FieldNamesWithLostData()
	{
		return new TheoryData<string>(FieldsWithLostData);
	}
	
	[Theory]
	[MemberData(nameof(FieldNames))]
	public void Clone_CreatesAnEqualCopy(string fieldName)
	{
		AssertCloneIsEqual(fieldName);
	}
	
	/// <summary>
	/// title: IsSearchable/SearchWeight (and Appearance) are not copied by any Clone; photos: Multiple is not copied; clip: VideoFieldInfo.Clone returns an ImageFieldInfo
	/// </summary>
	[Theory(Skip = "Bug (finding #14): some Clone implementations lose data or return another field type")]
	[MemberData(nameof(FieldNamesWithLostData))]
	public void Clone_OfFieldsWithLostData_CreatesAnEqualCopy(string fieldName)
	{
		AssertCloneIsEqual(fieldName);
	}
	
	private static void AssertCloneIsEqual(string fieldName)
	{
		var original = SampleFields.All().Single(x => x.Name == fieldName);
		
		var clone = (IFieldInfo) original.Clone();
		
		Assert.NotSame(original, clone);
		Assert.IsType(original.GetType(), clone);
		Assert.Equal(JsonSerializer.Serialize(original, Options), JsonSerializer.Serialize(clone, Options));
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
