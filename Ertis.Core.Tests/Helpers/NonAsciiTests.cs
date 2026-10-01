using Ertis.Core.Helpers;

namespace Ertis.Core.Tests.Helpers;

public class NonAsciiTests
{
	#region Methods
	
	[Fact]
	public void Maps_AreExposed()
	{
		var maps = new[]
		{
			NonAscii.International, NonAscii.Latin, NonAscii.Turkish, NonAscii.Greek, NonAscii.Czech, NonAscii.Arabic, NonAscii.Vietnamese,
			NonAscii.Polish, NonAscii.Latvian, NonAscii.German, NonAscii.Ukrainian, NonAscii.Serbian, NonAscii.Russian
		};
		
		Assert.All(maps, Assert.NotEmpty);
		Assert.Same(NonAscii.Turkish, NonAscii.Turkish);
		Assert.Equal("S", NonAscii.Turkish['Ş']);
	}
	
	[Theory]
	[InlineData('ş', "s")]
	[InlineData('ß', "ss")]
	[InlineData('ж', "zh")]
	[InlineData('Ψ', "ps")]
	[InlineData('日', "")]
	public void Slugify_RemapsTheNonAsciiCharacters(char c, string expected)
	{
		Assert.Equal("x" + expected, Slugifier.Slugify("x" + c));
	}
	
	/// <summary>
	/// Every mapped non-ASCII character is remapped (in lowercase) from the first map containing it (International, Latin, Turkish, ...); the ASCII characters (e.g. @) are not remapped
	/// </summary>
	[Fact]
	public void Slugify_UsesTheFirstMapContainingTheCharacter()
	{
		var maps = new[]
		{
			NonAscii.International, NonAscii.Latin, NonAscii.Turkish, NonAscii.Greek, NonAscii.Czech, NonAscii.Arabic, NonAscii.Vietnamese,
			NonAscii.Polish, NonAscii.Latvian, NonAscii.German, NonAscii.Ukrainian, NonAscii.Serbian, NonAscii.Russian
		};
		
		foreach (var c in maps.SelectMany(x => x.Keys).Distinct().Where(x => x >= 128))
		{
			Assert.Equal("x" + maps.First(x => x.ContainsKey(c))[c].ToLowerInvariant(), Slugifier.Slugify("x" + c));
		}
	}
	
	#endregion
}
