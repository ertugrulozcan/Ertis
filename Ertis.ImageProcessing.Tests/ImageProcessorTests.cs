using System.Net;
using Ertis.ImageProcessing.Exceptions;
using Ertis.ImageProcessing.Tests.TestHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using ImageProcessingException = Ertis.ImageProcessing.Exceptions.ImageProcessingException;

namespace Ertis.ImageProcessing.Tests;

public class ImageProcessorTests
{
	#region Properties
	
	private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;
	
	#endregion
	
	#region Convert Methods
	
	[Theory]
	[InlineData(ImageFormat.Bmp, "BMP")]
	[InlineData(ImageFormat.Gif, "GIF")]
	[InlineData(ImageFormat.Jpeg, "JPEG")]
	[InlineData(ImageFormat.Pbm, "PBM")]
	[InlineData(ImageFormat.Png, "PNG")]
	[InlineData(ImageFormat.Tga, "TGA")]
	[InlineData(ImageFormat.Tiff, "TIFF")]
	[InlineData(ImageFormat.Webp, "Webp")]
	public async Task Convert_WritesTheFormat(ImageFormat format, string expectedFormat)
	{
		using var input = TestImages.Create(20, 10);
		using var output = new MemoryStream();
		using var syncOutput = new MemoryStream();
		
		await ImageProcessor.ConvertAsync(input, output, format, cancellationToken: CancellationToken);
		input.Position = 0;
		ImageProcessor.Convert(input, syncOutput, format);
		
		Assert.Equal(expectedFormat, TestImages.DetectFormat(output).Name);
		Assert.Equal(output.ToArray(), syncOutput.ToArray());
		using var image = TestImages.Load(output);
		Assert.Equal((20, 10), (image.Width, image.Height));
	}
	
	[Fact]
	public async Task Convert_WithAQuality_EncodesWithIt()
	{
		using var input = TestImages.Create(64, 64);
		using var low = new MemoryStream();
		using var high = new MemoryStream();
		
		await ImageProcessor.ConvertAsync(input, low, ImageFormat.Jpeg, quality: 10, cancellationToken: CancellationToken);
		input.Position = 0;
		await ImageProcessor.ConvertAsync(input, high, ImageFormat.Jpeg, quality: 100, cancellationToken: CancellationToken);
		
		Assert.True(low.Length < high.Length);
	}
	
	[Fact]
	public void Convert_WithAnUndefinedFormat_ThrowsImageProcessingException()
	{
		using var input = TestImages.Create(4, 4);
		
		var exception = Assert.Throws<ImageProcessingException>(() => ImageProcessor.Convert(input, new MemoryStream(), (ImageFormat) 99));
		
		Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
	}
	
	[Theory]
	[InlineData(0)]
	[InlineData(101)]
	public async Task Convert_WithAnInvalidQuality_ThrowsBadRequest(int quality)
	{
		using var input = TestImages.Create(4, 4);
		
		var exception = await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.ConvertAsync(input, new MemoryStream(), ImageFormat.Jpeg, quality: quality, cancellationToken: CancellationToken));
		
		Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
		Assert.Equal("InvalidQuality", exception.ErrorCode);
	}
	
	#endregion
	
	#region Crop Methods
	
	[Fact]
	public async Task Crop_CropsTheImage()
	{
		using var input = TestImages.Create(20, 10);
		using var output = new MemoryStream();
		using var syncOutput = new MemoryStream();
		
		await ImageProcessor.CropAsync(input, output, new CropBounds { X = 10, Y = 2, Width = 5, Height = 4 }, ImageFormat.Png, cancellationToken: CancellationToken);
		input.Position = 0;
		ImageProcessor.Crop(input, syncOutput, new CropBounds { X = 12 }, ImageFormat.Png);
		
		using var image = TestImages.Load(output).CloneAs<Rgba32>();
		Assert.Equal((5, 4), (image.Width, image.Height));
		Assert.Equal(Color.Blue.ToPixel<Rgba32>(), image[0, 0]);
		using var syncImage = TestImages.Load(syncOutput);
		Assert.Equal((8, 10), (syncImage.Width, syncImage.Height));
	}
	
	[Fact]
	public async Task Crop_WithBoundsOutOfTheImage_ThrowsBadRequest()
	{
		using var input = TestImages.Create(20, 10);
		
		var exception = await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.CropAsync(input, new MemoryStream(), new CropBounds { X = 18, Width = 5 }, ImageFormat.Png, cancellationToken: CancellationToken));
		
		Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
		Assert.Equal("CropBoundsOverflow", exception.ErrorCode);
	}
	
	[Theory]
	[InlineData(-1, 0, 5, 5)]
	[InlineData(0, -1, 5, 5)]
	[InlineData(0, 0, 0, 5)]
	[InlineData(0, 0, 5, -2)]
	[InlineData(25, 0, null, null)]
	public void CropBounds_WithInvalidBounds_ThrowsBadRequest(int x, int y, int? width, int? height)
	{
		var exception = Assert.Throws<ImageProcessingException>(() => new CropBounds { X = x, Y = y, Width = width, Height = height }.ToRectangle(20, 10));
		
		Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
		Assert.Equal("CropBoundsOverflow", exception.ErrorCode);
	}
	
	#endregion
	
	#region Resize Methods
	
	[Theory]
	[InlineData(10, null, 10, 5)]
	[InlineData(null, 5, 10, 5)]
	[InlineData(8, 8, 8, 8)]
	public async Task Resize_ResizesTheImage(int? width, int? height, int expectedWidth, int expectedHeight)
	{
		using var input = TestImages.Create(20, 10);
		using var output = new MemoryStream();
		
		await ImageProcessor.ResizeAsync(input, output, width, height, ImageFormat.Png, cancellationToken: CancellationToken);
		
		using var image = TestImages.Load(output);
		Assert.Equal((expectedWidth, expectedHeight), (image.Width, image.Height));
	}
	
	[Fact]
	public void Resize_WithTheOptions_ResizesTheImage()
	{
		using var input = TestImages.Create(400, 200);
		using var max = new MemoryStream();
		using var stretch = new MemoryStream();
		using var withoutTargetSize = new MemoryStream();
		
		ImageProcessor.Resize(input, max, 50, 50, ImageFormat.Jpeg, ResizeMode.Max, Anchor.TopLeft, SamplerAlgorithm.Lanczos, quality: 90);
		input.Position = 0;
		ImageProcessor.Resize(input, stretch, 50, 50, ImageFormat.Webp, ResizeMode.Stretch, sampler: SamplerAlgorithm.NearestNeighbor, level: 4);
		input.Position = 0;
		ImageProcessor.Resize(input, withoutTargetSize, 100, null, ImageFormat.Png, targetSizeMode: TargetSizeMode.None);
		
		using var maxImage = TestImages.Load(max);
		using var stretchImage = TestImages.Load(stretch);
		using var withoutTargetSizeImage = TestImages.Load(withoutTargetSize);
		Assert.Equal((50, 25), (maxImage.Width, maxImage.Height));
		Assert.Equal((50, 50), (stretchImage.Width, stretchImage.Height));
		Assert.Equal((100, 50), (withoutTargetSizeImage.Width, withoutTargetSizeImage.Height));
	}
	
	[Theory]
	[InlineData(0, 5)]
	[InlineData(5, -1)]
	public async Task Resize_WithInvalidDimensions_ThrowsBadRequest(int? width, int? height)
	{
		using var input = TestImages.Create(20, 10);
		
		var exception = await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.ResizeAsync(input, new MemoryStream(), width, height, ImageFormat.Png, cancellationToken: CancellationToken));
		
		Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
		Assert.Equal("InvalidDimensions", exception.ErrorCode);
	}
	
	[Fact]
	public async Task Resize_WithoutDimensions_ConvertsTheImage()
	{
		using var input = TestImages.Create(20, 10);
		using var output = new MemoryStream();
		
		await ImageProcessor.ResizeAsync(input, output, null, null, ImageFormat.Webp, cancellationToken: CancellationToken);
		
		Assert.Equal("Webp", TestImages.DetectFormat(output).Name);
	}
	
	[Fact]
	public async Task Resize_ReadsTheStreamFromItsPosition()
	{
		using var image = TestImages.Create(20, 10);
		using var input = new MemoryStream();
		input.Write([1, 2, 3]);
		image.CopyTo(input);
		input.Position = 3;
		using var output = new MemoryStream();
		
		await ImageProcessor.ResizeAsync(input, output, 10, null, ImageFormat.Png, cancellationToken: CancellationToken);
		
		using var result = TestImages.Load(output);
		Assert.Equal(10, result.Width);
	}
	
	[Fact]
	public async Task Resize_WithANonSeekableStream_ResizesTheImage()
	{
		using var image = TestImages.Create(20, 10);
		using var output = new MemoryStream();
		
		await ImageProcessor.ResizeAsync(new NonSeekableStream(image), output, 10, null, ImageFormat.Png, cancellationToken: CancellationToken);
		
		using var result = TestImages.Load(output);
		Assert.Equal(10, result.Width);
	}
	
	#endregion
	
	#region Invalid Content Methods
	
	[Fact]
	public async Task Resize_WithAnInvalidImage_ThrowsBadRequest()
	{
		using var input = new MemoryStream("not an image"u8.ToArray());
		
		var exception = await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.ResizeAsync(input, new MemoryStream(), 10, 10, ImageFormat.Png, cancellationToken: CancellationToken));
		
		Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
		Assert.Equal("InvalidImageContent", exception.ErrorCode);
	}
	
	[Fact]
	public async Task CropAndConvert_WithAnInvalidImage_ThrowBadRequest()
	{
		var exceptions = new[]
		{
			await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.CropAsync(new MemoryStream("x"u8.ToArray()), new MemoryStream(), new CropBounds(), ImageFormat.Png, cancellationToken: CancellationToken)),
			Assert.Throws<ImageProcessingException>(() => ImageProcessor.Crop(new MemoryStream("x"u8.ToArray()), new MemoryStream(), new CropBounds(), ImageFormat.Png)),
			await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.ConvertAsync(new MemoryStream("x"u8.ToArray()), new MemoryStream(), ImageFormat.Png, cancellationToken: CancellationToken)),
			Assert.Throws<ImageProcessingException>(() => ImageProcessor.Convert(new MemoryStream("x"u8.ToArray()), new MemoryStream(), ImageFormat.Png))
		};
		
		Assert.All(exceptions, x =>
		{
			Assert.Equal(HttpStatusCode.BadRequest, x.StatusCode);
			Assert.Equal("InvalidImageContent", x.ErrorCode);
		});
	}
	
	[Fact]
	public async Task Operations_WhenCanceled_ThrowOperationCanceledException()
	{
		using var cancellationTokenSource = new CancellationTokenSource();
		await cancellationTokenSource.CancelAsync();
		
		await Assert.ThrowsAnyAsync<OperationCanceledException>(() => ImageProcessor.ResizeAsync(TestImages.Create(4, 4), new MemoryStream(), 2, 2, ImageFormat.Png, cancellationToken: cancellationTokenSource.Token));
		await Assert.ThrowsAnyAsync<OperationCanceledException>(() => ImageProcessor.CropAsync(TestImages.Create(4, 4), new MemoryStream(), new CropBounds(), ImageFormat.Png, cancellationToken: cancellationTokenSource.Token));
		await Assert.ThrowsAnyAsync<OperationCanceledException>(() => ImageProcessor.ConvertAsync(TestImages.Create(4, 4), new MemoryStream(), ImageFormat.Png, cancellationToken: cancellationTokenSource.Token));
	}
	
	#endregion
	
	#region Metadata Methods
	
	[Fact]
	public async Task GetMetadata_ReadsTheMetadata()
	{
		using var input = TestImages.Create(20, 10, new JpegEncoder(), orientation: 6);
		
		var metadata = await ImageProcessor.GetMetadataAsync(input, CancellationToken);
		
		Assert.NotNull(metadata?.ExifProfile);
		Assert.True(metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientation));
		Assert.Equal((ushort) 6, orientation.Value);
	}
	
	/// <summary>
	/// Orientation 6: the stored 20x10 image is displayed rotated 90 degrees clockwise (10x20)
	/// </summary>
	[Fact]
	public async Task Operations_ApplyTheExifOrientation()
	{
		using var resizeOutput = new MemoryStream();
		using var cropOutput = new MemoryStream();
		
		await ImageProcessor.ResizeAsync(TestImages.Create(20, 10, new JpegEncoder(), orientation: 6), resizeOutput, 5, null, ImageFormat.Png, cancellationToken: CancellationToken);
		await ImageProcessor.CropAsync(TestImages.Create(20, 10, new JpegEncoder(), orientation: 6), cropOutput, new CropBounds { Width = 10, Height = 15 }, ImageFormat.Png, cancellationToken: CancellationToken);
		
		using var resized = TestImages.Load(resizeOutput);
		using var cropped = TestImages.Load(cropOutput);
		Assert.Equal((5, 10), (resized.Width, resized.Height));
		Assert.Equal((10, 15), (cropped.Width, cropped.Height));
	}
	
	/// <summary>
	/// The orientation is reset after it is applied, so the viewers do not rotate the image again
	/// </summary>
	[Fact]
	public async Task Operations_ResetTheExifOrientation()
	{
		using var output = new MemoryStream();
		
		await ImageProcessor.ConvertAsync(TestImages.Create(20, 10, new JpegEncoder(), orientation: 6), output, ImageFormat.Jpeg, cancellationToken: CancellationToken);
		
		using var image = TestImages.Load(output);
		Assert.Equal((10, 20), (image.Width, image.Height));
		IExifValue<ushort>? orientation = null;
		image.Metadata.ExifProfile?.TryGetValue(ExifTag.Orientation, out orientation);
		Assert.True(orientation == null || orientation.Value == ExifOrientationMode.TopLeft);
	}
	
	[Fact]
	public async Task Operations_WithStripMetadata_RemoveTheMetadata()
	{
		using var kept = new MemoryStream();
		using var stripped = new MemoryStream();
		using var resized = new MemoryStream();
		using var cropped = new MemoryStream();
		
		await ImageProcessor.ConvertAsync(TestImages.Create(20, 10, new JpegEncoder(), withGps: true), kept, ImageFormat.Jpeg, cancellationToken: CancellationToken);
		ImageProcessor.Convert(TestImages.Create(20, 10, new JpegEncoder(), withGps: true), stripped, ImageFormat.Jpeg, stripMetadata: true);
		await ImageProcessor.ResizeAsync(TestImages.Create(20, 10, new JpegEncoder(), withGps: true), resized, 10, null, ImageFormat.Jpeg, stripMetadata: true, cancellationToken: CancellationToken);
		ImageProcessor.Crop(TestImages.Create(20, 10, new JpegEncoder(), withGps: true), cropped, new CropBounds { Width = 5 }, ImageFormat.Webp, stripMetadata: true);
		
		Assert.NotNull((await ImageProcessor.GetMetadataAsync(Rewind(kept), CancellationToken))?.ExifProfile);
		Assert.Null((await ImageProcessor.GetMetadataAsync(Rewind(stripped), CancellationToken))?.ExifProfile);
		Assert.Null((await ImageProcessor.GetMetadataAsync(Rewind(resized), CancellationToken))?.ExifProfile);
		Assert.Null((await ImageProcessor.GetMetadataAsync(Rewind(cropped), CancellationToken))?.ExifProfile);
	}
	
	[Fact]
	public async Task GetMetadata_WithAnInvalidImage_ThrowsBadRequest()
	{
		var exception = await Assert.ThrowsAsync<ImageProcessingException>(() => ImageProcessor.GetMetadataAsync(new MemoryStream("x"u8.ToArray()), CancellationToken));
		
		Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
	}
	
	[Fact]
	public async Task Resize_WithAnExifOrientationAndALargeDownscale_KeepsTheDisplayedAspectRatio()
	{
		using var output = new MemoryStream();
		
		await ImageProcessor.ResizeAsync(TestImages.Create(400, 200, new JpegEncoder(), orientation: 6), output, 20, null, ImageFormat.Png, cancellationToken: CancellationToken);
		
		using var image = TestImages.Load(output);
		Assert.Equal((20, 40), (image.Width, image.Height));
	}
	
	private static MemoryStream Rewind(MemoryStream stream)
	{
		stream.Position = 0;
		return stream;
	}
	
	#endregion
}
