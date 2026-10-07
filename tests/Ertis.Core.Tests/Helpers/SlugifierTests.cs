using Ertis.Core.Helpers;

namespace Ertis.Core.Tests.Helpers;

/// <summary>
/// The slugs are stored by the consumers (e.g. ErtisAuth role, membership slugs), so the outputs are characterized as they are
/// </summary>
public class SlugifierTests
{
	#region Methods
	
	[Theory]
	[InlineData("Hello World", "hello-world")]
	[InlineData("  Şule Öztürk  ", "sule-ozturk")]
	[InlineData("İstanbul ığüşöç ĞÇ", "istanbul-igusoc-gc")]
	[InlineData("a--b__c", "a-b-c")]
	[InlineData("Ünal's Café!", "unals-cafe")]
	[InlineData("hello!world", "helloworld")]
	[InlineData("Straße", "strasse")]
	[InlineData("Москва", "moskva")]
	[InlineData("Ελλάδα", "ellada")]
	[InlineData("a_b c", "a-b-c")]
	[InlineData("_x_", "x")]
	[InlineData("x.y/z\\w=q,r", "x-y-z-w-q-r")]
	[InlineData("ABC 123", "abc-123")]
	[InlineData("日本", "")]
	[InlineData("-", "")]
	public void Slugify_CreatesTheSlug(string input, string expected)
	{
		Assert.Equal(expected, Slugifier.Slugify(input));
	}
	
	[Theory]
	[InlineData("a_b c", "a_b-c")]
	[InlineData("_x_", "_x_")]
	[InlineData("a--b__c", "a-b__c")]
	[InlineData("Super Admin_Role", "super-admin_role")]
	public void Slugify_WithIgnoredCharacters_KeepsThem(string input, string expected)
	{
		Assert.Equal(expected, Slugifier.Slugify(input, Slugifier.Options.Ignore('_')));
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void Slugify_WithAnEmptyInput_ReturnsIt(string? input)
	{
		Assert.Equal(input, Slugifier.Slugify(input!));
	}
	
	[Theory]
	[InlineData("Đorđe", "dorde")]
	[InlineData("Ștefan Țepeș", "stefan-tepes")]
	[InlineData("Ầu", "au")]
	[InlineData("Љубав", "ljubav")]
	[InlineData("Ǽ ¶ ϒ", "ae-p-y")]
	public void Slugify_CreatesALowercaseSlug(string input, string expected)
	{
		Assert.Equal(expected, Slugifier.Slugify(input));
	}
	
	#endregion
}
