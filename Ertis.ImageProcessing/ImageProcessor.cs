using System.Diagnostics.CodeAnalysis;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;

namespace Ertis.ImageProcessing;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class ImageProcessor
{
	#region Methods

	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public static void Crop(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat, int? quality = null)
	{
		try
		{
			using var image = Image.Load(imageStream);
			image.Mutate(x => x.Crop(bounds.ToRectangle(image.Width, image.Height))); 
			image.Save(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality));
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public static async Task CropAsync(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat, int? quality = null, CancellationToken cancellationToken = default)
	{
		try
		{
			using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
			image.Mutate(x => x.Crop(bounds.ToRectangle(image.Width, image.Height))); 
			await image.SaveAsync(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality), cancellationToken: cancellationToken);
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	public static void Resize(Stream imageStream, Stream outputStream, int? width, int? height, ImageFormat destinationFormat, ResizeMode? mode = null, Anchor? anchor = null, SamplerAlgorithm? sampler = null, int? quality = null)
	{
		if (width == null && height == null)
		{
			return;
		}

		try
		{
			using var image = Image.Load(imageStream);
			var options = new ResizeOptions
			{
				Size = new Size(width ?? 0, height ?? 0),
				Mode = mode ?? ResizeModeEnum.Crop,
				Position = anchor ?? AnchorPositionMode.Center,
				Sampler = (sampler ?? SamplerAlgorithm.Bicubic).ToResampler()!
			};
			
			image.Mutate(x => x.Resize(options)); 
			image.Save(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality));
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	public static async Task ResizeAsync(Stream imageStream, Stream outputStream, int? width, int? height, ImageFormat destinationFormat, ResizeMode? mode = null, Anchor? anchor = null, SamplerAlgorithm? sampler = null, int? quality = null, CancellationToken cancellationToken = default)
	{
		if (width == null && height == null)
		{
			return;
		}

		try
		{
			using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
			var options = new ResizeOptions
			{
				Size = new Size(width ?? 0, height ?? 0),
				Mode = mode ?? ResizeModeEnum.Crop,
				Position = anchor ?? AnchorPositionMode.Center,
				Sampler = (sampler ?? SamplerAlgorithm.Bicubic).ToResampler()!
			};
			
			image.Mutate(x => x.Resize(options)); 
			await image.SaveAsync(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality), cancellationToken: cancellationToken);
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	public static void Convert(Stream imageStream, Stream outputStream, ImageFormat destinationFormat, int? quality = null)
	{
		try
		{
			using var image = Image.Load(imageStream);
			switch (destinationFormat)
			{
				case ImageFormat.Bmp:
					image.SaveAsBmpAsync(outputStream);
					break;
				case ImageFormat.Gif:
					image.SaveAsGifAsync(outputStream);
					break;
				case ImageFormat.Jpeg:
					image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = quality });
					break;
				case ImageFormat.Pbm:
					image.SaveAsPbmAsync(outputStream);
					break;
				case ImageFormat.Png:
					image.SaveAsPngAsync(outputStream);
					break;
				case ImageFormat.Tga:
					image.SaveAsTgaAsync(outputStream);
					break;
				case ImageFormat.Tiff:
					image.SaveAsTiffAsync(outputStream);
					break;
				case ImageFormat.Webp:
					image.SaveAsWebpAsync(outputStream, new WebpEncoder { FileFormat = WebpFileFormatType.Lossy, NearLossless = false, Quality = quality ?? Constants.DefaultQuality });
					break;
			}
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	public static async Task ConvertAsync(Stream imageStream, Stream outputStream, ImageFormat destinationFormat, int? quality = null, CancellationToken cancellationToken = default)
	{
		try
		{
			using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
			switch (destinationFormat)
			{
				case ImageFormat.Bmp:
					await image.SaveAsBmpAsync(outputStream, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Gif:
					await image.SaveAsGifAsync(outputStream, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Jpeg:
					await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = quality }, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Pbm:
					await image.SaveAsPbmAsync(outputStream, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Png:
					await image.SaveAsPngAsync(outputStream, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Tga:
					await image.SaveAsTgaAsync(outputStream, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Tiff:
					await image.SaveAsTiffAsync(outputStream, cancellationToken: cancellationToken);
					break;
				case ImageFormat.Webp:
					await image.SaveAsWebpAsync(outputStream, new WebpEncoder { FileFormat = WebpFileFormatType.Lossy, NearLossless = false, Quality = quality ?? Constants.DefaultQuality }, cancellationToken: cancellationToken);
					break;
			}
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	#endregion
}