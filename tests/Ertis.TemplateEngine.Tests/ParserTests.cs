namespace Ertis.TemplateEngine.Tests;

public class ParserTests
{
	#region Methods
	
	[Fact]
	public void Parse_SplitsTheTemplateIntoSegments()
	{
		var segments = new Parser().Parse("a {{b}} c {{d}}").ToArray();
		
		Assert.Equal(["a ", "{{b}}", " c ", "{{d}}"], segments.Select(x => x.ToString()));
		Assert.Equal([SegmentType.RawPart, SegmentType.PlaceHolder, SegmentType.RawPart, SegmentType.PlaceHolder], segments.Select(x => x.Type));
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void Parse_WithAnEmptyText_ReturnsOneRawPart(string? text)
	{
		var segment = Assert.Single(new Parser().Parse(text!));
		
		Assert.Equal(SegmentType.RawPart, segment.Type);
	}
	
	[Fact]
	public void Parse_WithCustomBrackets_FindsThePlaceholders()
	{
		var segments = new Parser(new ParserOptions { OpenBrackets = "<%", CloseBrackets = "%>" }).Parse("x <% a %> y {{b}}").ToArray();
		
		Assert.Equal("a", Assert.IsType<PlaceHolder>(segments[1]).Value);
		Assert.Equal(" y {{b}}", segments[2].ToString());
	}
	
	[Theory]
	[InlineData("", "}}")]
	[InlineData("{{", "")]
	public void Constructor_WithEmptyBrackets_Throws(string openBrackets, string closeBrackets)
	{
		Assert.Throws<ArgumentException>(() => new Parser(new ParserOptions { OpenBrackets = openBrackets, CloseBrackets = closeBrackets }));
	}
	
	[Fact]
	public void Parse_WithACloseBracketBeforeAPlaceholder_FindsThePlaceholder()
	{
		var segments = new Parser().Parse("a }} b {{name}} c").ToArray();
		
		Assert.Equal(["a }} b ", "{{name}}", " c"], segments.Select(x => x.ToString()));
		Assert.Equal("name", Assert.IsType<PlaceHolder>(segments[1]).Value);
	}
	
	[Theory]
	[InlineData("a {{ b")]
	[InlineData("a }} b")]
	public void Parse_WithAnUnclosedPlaceholder_ReturnsTheTextAsARawPart(string text)
	{
		var segment = Assert.Single(new Parser().Parse(text));
		
		Assert.Equal(SegmentType.RawPart, segment.Type);
		Assert.Equal(text, segment.ToString());
	}
	
	#endregion
}
