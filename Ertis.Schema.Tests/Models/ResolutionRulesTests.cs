using Ertis.Schema.Models;

namespace Ertis.Schema.Tests.Models;

public class ResolutionRulesTests
{
	#region Methods
	
	[Fact]
	public void Create_WithValidBounds_Succeeds()
	{
		var rules = new ResolutionRules { MinWidth = 100, MaxWidth = 200, MinHeight = 50, MaxHeight = 100, AspectRatioRequired = true };
		
		Assert.Equal(100, rules.MinWidth);
		Assert.Equal(100, rules.MaxHeight);
	}
	
	[Fact]
	public void Create_WithNegativeMinWidth_Throws()
	{
		var exception = Assert.Throws<Exception>(() => new ResolutionRules { MinWidth = -1 });
		
		Assert.StartsWith("The 'minWidth' value can not be less than zero", exception.Message);
	}
	
	[Fact]
	public void Create_WithMinWidthGreaterThanMaxWidth_Throws()
	{
		var exception = Assert.Throws<Exception>(() => new ResolutionRules { MaxWidth = 100, MinWidth = 200 });
		
		Assert.StartsWith("The 'minWidth' value can not be greater than the 'maxWidth' value", exception.Message);
	}
	
	[Fact]
	public void Create_WithMinHeightGreaterThanMaxHeight_Throws()
	{
		var exception = Assert.Throws<Exception>(() => new ResolutionRules { MaxHeight = 100, MinHeight = 200 });
		
		Assert.StartsWith("The 'minHeight' value can not be greater than the 'maxHeight' value", exception.Message);
	}
	
	[Fact]
	public void Clone_CopiesTheRules()
	{
		var rules = new ResolutionRules { MinWidth = 100, MaxWidth = 200, RecommendedWidth = 150, MinSizesRequired = true };
		
		var clone = (ResolutionRules) rules.Clone();
		
		Assert.Equal(rules, clone);
	}
	
	#endregion
}
