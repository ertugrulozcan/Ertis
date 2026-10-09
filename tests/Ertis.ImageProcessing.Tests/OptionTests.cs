using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;

namespace Ertis.ImageProcessing.Tests;

public class OptionTests
{
	#region Parse Methods
	
	[Theory]
	[InlineData("crop", ResizeModeEnum.Crop)]
	[InlineData("Pad", ResizeModeEnum.Pad)]
	[InlineData("boxpad", ResizeModeEnum.BoxPad)]
	[InlineData("max", ResizeModeEnum.Max)]
	[InlineData("min", ResizeModeEnum.Min)]
	[InlineData("stretch", ResizeModeEnum.Stretch)]
	[InlineData("manual", ResizeModeEnum.Manual)]
	public void ResizeMode_Parse_ReturnsTheMode(string key, ResizeModeEnum expected)
	{
		Assert.Equal(expected, (ResizeModeEnum) ResizeMode.Parse(key)!);
	}
	
	[Theory]
	[InlineData("center", AnchorPositionMode.Center)]
	[InlineData("top", AnchorPositionMode.Top)]
	[InlineData("bottom", AnchorPositionMode.Bottom)]
	[InlineData("left", AnchorPositionMode.Left)]
	[InlineData("right", AnchorPositionMode.Right)]
	[InlineData("topleft", AnchorPositionMode.TopLeft)]
	[InlineData("topright", AnchorPositionMode.TopRight)]
	[InlineData("bottomleft", AnchorPositionMode.BottomLeft)]
	[InlineData("bottomright", AnchorPositionMode.BottomRight)]
	public void Anchor_Parse_ReturnsTheAnchor(string key, AnchorPositionMode expected)
	{
		Assert.Equal(expected, (AnchorPositionMode) Anchor.Parse(key)!);
	}
	
	[Theory]
	[InlineData("bicubic", typeof(BicubicResampler))]
	[InlineData("box", typeof(BoxResampler))]
	[InlineData("cubic", typeof(CubicResampler))]
	[InlineData("lanczos", typeof(LanczosResampler))]
	[InlineData("triangle", typeof(TriangleResampler))]
	[InlineData("welch", typeof(WelchResampler))]
	[InlineData("nearestneighbor", typeof(NearestNeighborResampler))]
	public void SamplerAlgorithm_Parse_ReturnsTheSampler(string key, Type expectedResampler)
	{
		Assert.IsType(expectedResampler, SamplerAlgorithm.Parse(key)!.ToResampler());
	}
	
	[Theory]
	[InlineData("unknown")]
	[InlineData("")]
	public void Parse_WithAnUnknownKey_ReturnsNull(string key)
	{
		Assert.Null(ResizeMode.Parse(key));
		Assert.Null(Anchor.Parse(key));
		Assert.Null(SamplerAlgorithm.Parse(key));
	}
	
	#endregion
	
	#region Encoder Methods
	
	[Theory]
	[InlineData(null, WebpEncodingMethod.Level2)]
	[InlineData(6, WebpEncodingMethod.BestQuality)]
	[InlineData(9, WebpEncodingMethod.Level2)]
	public void FormatEncoder_Webp_UsesTheLevel(int? level, WebpEncodingMethod expected)
	{
		var encoder = Assert.IsType<WebpEncoder>(FormatEncoder.GetDefaultFormatter(ImageFormat.WEBP, 80, level));
		
		Assert.Equal(expected, encoder.Method);
		Assert.Equal(80, encoder.Quality);
	}
	
	[Theory]
	[InlineData(null, PngCompressionLevel.DefaultCompression)]
	[InlineData(1, PngCompressionLevel.BestSpeed)]
	[InlineData(9, PngCompressionLevel.BestCompression)]
	[InlineData(12, PngCompressionLevel.DefaultCompression)]
	public void FormatEncoder_Png_UsesTheLevel(int? level, PngCompressionLevel expected)
	{
		Assert.Equal(expected, Assert.IsType<PngEncoder>(FormatEncoder.GetDefaultFormatter(ImageFormat.PNG, level: level)).CompressionLevel);
	}
	
	[Fact]
	public void FormatEncoder_WithAnUndefinedFormat_Throws()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => FormatEncoder.GetDefaultFormatter((ImageFormat) 99));
	}
	
	#endregion
}
