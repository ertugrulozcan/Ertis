using System.Net;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

using ImageProcessingException = Ertis.ImageProcessing.Exceptions.ImageProcessingException;

namespace Ertis.ImageProcessing;

public static class FormatEncoder
{
	#region Methods
	
	public static IImageEncoder GetDefaultFormatter(ImageFormat format, int? quality = null, int? level = null)
	{
		if (quality is < 1 or > 100 && format is ImageFormat.JPEG or ImageFormat.WEBP)
		{
			throw new ImageProcessingException(HttpStatusCode.BadRequest, "Quality must be between 1 and 100.", "InvalidQuality");
		}
		
		return format switch
		{
			ImageFormat.BMP => new BmpEncoder(),
			ImageFormat.GIF => new GifEncoder(),
			ImageFormat.JPEG => new JpegEncoder
			{
				Quality = quality ?? Constants.DefaultQuality,
				Interleaved = true
			},
			ImageFormat.PBM => new PbmEncoder(),
			ImageFormat.PNG => new PngEncoder
			{
				// The level is the (lossless) compression level, 1 (the fastest) to 9 (the smallest); the default is 6
				CompressionLevel = level is >= 1 and <= 9 ? (PngCompressionLevel) level.Value : PngCompressionLevel.DefaultCompression
			},
			ImageFormat.TGA => new TgaEncoder(),
			ImageFormat.TIFF => new TiffEncoder(),
			ImageFormat.WEBP => new WebpEncoder
			{
				Quality = quality ?? Constants.DefaultQuality,
				Method = level != null && Enum.IsDefined(typeof(WebpEncodingMethod), level) ? (WebpEncodingMethod)(object)level : WebpEncodingMethod.Level2,
				FileFormat = WebpFileFormatType.Lossy,
				NearLossless = false
			},
			_ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
		};
	}
	
	#endregion
}