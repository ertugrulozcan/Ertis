using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

namespace Ertis.ImageProcessing;

public static class FormatEncoder
{
	#region Methods

	public static IImageEncoder GetDefaultFormatter(ImageFormat format, int? quality = null)
	{
		return format switch
		{
			ImageFormat.Bmp => new BmpEncoder(),
			ImageFormat.Gif => new GifEncoder(),
			ImageFormat.Jpeg => new JpegEncoder { Quality = quality ?? Constants.DefaultQuality },
			ImageFormat.Pbm => new PbmEncoder(),
			ImageFormat.Png => new PngEncoder(),
			ImageFormat.Tga => new TgaEncoder(),
			ImageFormat.Tiff => new TiffEncoder(),
			ImageFormat.Webp => new WebpEncoder { Quality = quality ?? Constants.DefaultQuality },
			_ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
		};
	}

	#endregion
}