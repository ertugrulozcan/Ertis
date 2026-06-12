// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.TemplateEngine;

public class PlaceHolder : ITemplateSegment
{
	#region Properties
	
	public SegmentType Type => SegmentType.PlaceHolder;
	
	public required string Inner { get; init; }
	
	public required string Outer { get; init; }
	
	public string? OpenBrackets { get; init; }
	
	public string? CloseBrackets { get; init; }
	
	public required string Value { get; init; }
	
	public int StartIndex { get; init; }
	
	public int Length { get; init; }
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		return this.Outer;
	}
	
	#endregion
}