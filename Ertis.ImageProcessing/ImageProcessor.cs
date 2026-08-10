using System.Diagnostics.CodeAnalysis;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Processing;
using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;
using ImageProcessingException = Ertis.ImageProcessing.Exceptions.ImageProcessingException;

// ReSharper disable once UnusedType.Global
namespace Ertis.ImageProcessing;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class ImageProcessor
{
	#region Methods
	
	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public static void Crop(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat, int? quality = null, int? level = null)
	{
		try
		{
			using var image = Image.Load(imageStream);
			image.Mutate(x => x.Crop(bounds.ToRectangle(image.Width, image.Height))); 
			image.Save(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level));
		}
		catch (ImageProcessingException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex) when (ex is not OutOfMemoryException)
		{
			throw new ImageProcessingException(HttpStatusCode.InternalServerError, ex.Message, "ImageProcessingError", ex);
		}
	}
	
	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public static async Task CropAsync(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat, int? quality = null, int? level = null, CancellationToken cancellationToken = default)
	{
		try
		{
			using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
			image.Mutate(x => x.Crop(bounds.ToRectangle(image.Width, image.Height))); 
			await image.SaveAsync(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level), cancellationToken: cancellationToken);
		}
		catch (ImageProcessingException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex) when (ex is not OutOfMemoryException)
		{
			throw new ImageProcessingException(HttpStatusCode.InternalServerError, ex.Message, "ImageProcessingError", ex);
		}
	}
	
	public static void Resize(
		Stream imageStream, 
		Stream outputStream, 
		int? width, 
		int? height, 
		ImageFormat destinationFormat, 
		ResizeMode? mode = null, 
		Anchor? anchor = null, 
		SamplerAlgorithm? sampler = null, 
		int? quality = null, 
		int? level = null)
	{
		ResizeAsync(imageStream, outputStream, width, height, destinationFormat, mode, anchor, sampler, quality, level).ConfigureAwait(false).GetAwaiter().GetResult();
	}
	
	public static async Task ResizeAsync(
		Stream imageStream, 
		Stream outputStream, 
		int? width, 
		int? height, 
		ImageFormat destinationFormat, 
		ResizeMode? mode = null, 
		Anchor? anchor = null, 
		SamplerAlgorithm? sampler = null, 
		int? quality = null, 
		int? level = null, 
		CancellationToken cancellationToken = default)
	{
		if (width == null && height == null)
		{
			return;
		}
		
		if (width is <= 0 || height is <= 0)
		{
			throw new ImageProcessingException(HttpStatusCode.BadRequest, "Width and height must be greater than zero.", "InvalidDimensions");
		}
		
		try
		{
			var sourceInfo = await Image.IdentifyAsync(imageStream, cancellationToken);
			imageStream.Position = 0;
			
			var resizeMode = mode ?? ResizeModeEnum.Crop;
			var decoderOptions = GetDecoderOptions(sourceInfo, resizeMode, width, height, out var targetWidth, out var targetHeight);
			using var image = await Image.LoadAsync(decoderOptions, imageStream, cancellationToken: cancellationToken);
			var options = new ResizeOptions
			{
				Size = new Size(targetWidth, targetHeight),
				Mode = resizeMode,
				Position = anchor ?? AnchorPositionMode.Center,
				Sampler = (sampler ?? SamplerAlgorithm.Bicubic).ToResampler()!
			};
			
			image.Mutate(x => x.Resize(options)); 
			await image.SaveAsync(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level), cancellationToken: cancellationToken);
		}
		catch (ImageProcessingException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (InvalidImageContentException ex) when (ex.InnerException is InvalidMemoryOperationException)
		{
			throw new ImageProcessingException(HttpStatusCode.ServiceUnavailable, "Image is too large to process.", "ImageTooLarge", ex);
		}
		catch (Exception ex) when (ex is InvalidImageContentException or UnknownImageFormatException or NotSupportedException)
		{
			throw new ImageProcessingException(HttpStatusCode.BadRequest, "The image could not be decoded.", "InvalidImageContent", ex);
		}
		catch (Exception ex) when (ex is not OutOfMemoryException)
		{
			throw new ImageProcessingException(HttpStatusCode.InternalServerError, ex.Message, "ImageProcessingError", ex);
		}
	}
	
	public static void Convert(Stream imageStream, Stream outputStream, ImageFormat destinationFormat, int? quality = null, int? level = null)
	{
		try
		{
			using var image = Image.Load(imageStream);
			switch (destinationFormat)
			{
				case ImageFormat.Bmp:
					image.SaveAsBmp(outputStream);
					break;
				case ImageFormat.Gif:
					image.SaveAsGif(outputStream);
					break;
				case ImageFormat.Jpeg:
					image.SaveAsJpeg(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level) as JpegEncoder);
					break;
				case ImageFormat.Pbm:
					image.SaveAsPbm(outputStream);
					break;
				case ImageFormat.Png:
					image.SaveAsPng(outputStream);
					break;
				case ImageFormat.Tga:
					image.SaveAsTga(outputStream);
					break;
				case ImageFormat.Tiff:
					image.SaveAsTiff(outputStream);
					break;
				case ImageFormat.Webp:
					image.SaveAsWebp(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level) as WebpEncoder);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(destinationFormat), destinationFormat, "Unsupported image format");
			}
		}
		catch (ImageProcessingException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex) when (ex is not OutOfMemoryException)
		{
			throw new ImageProcessingException(HttpStatusCode.InternalServerError, ex.Message, "ImageProcessingError", ex);
		}
	}
	
	public static async Task ConvertAsync(Stream imageStream, Stream outputStream, ImageFormat destinationFormat, int? quality = null, int? level = null, CancellationToken cancellationToken = default)
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
					await image.SaveAsJpegAsync(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level) as JpegEncoder, cancellationToken: cancellationToken);
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
					await image.SaveAsWebpAsync(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level) as WebpEncoder, cancellationToken: cancellationToken);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(destinationFormat), destinationFormat, "Unsupported image format");
			}
		}
		catch (ImageProcessingException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex) when (ex is not OutOfMemoryException)
		{
			throw new ImageProcessingException(HttpStatusCode.InternalServerError, ex.Message, "ImageProcessingError", ex);
		}
	}
	
	public static async Task<ImageMetadata?> GetMetadataAsync(Stream imageStream, CancellationToken cancellationToken = default)
	{
		using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
		return image.Metadata;
	}
	
	private static DecoderOptions GetDecoderOptions(ImageInfo sourceInfo, ResizeModeEnum resizeMode, int? width, int? height, out int targetWidth, out int targetHeight)
	{
		targetWidth = width ?? Math.Max(1, (int)Math.Round(sourceInfo.Width * ((double)height!.Value / sourceInfo.Height)));
		targetHeight = height ?? Math.Max(1, (int)Math.Round(sourceInfo.Height * ((double)width!.Value / sourceInfo.Width)));
		
		return new DecoderOptions
		{
			TargetSize = GetDecoderTargetSize(new Size(sourceInfo.Width, sourceInfo.Height), new Size(targetWidth, targetHeight), resizeMode)
		};
	}
	
	private static Size? GetDecoderTargetSize(Size sourceSize, Size targetSize, ResizeModeEnum mode)
	{
		var scaleX = (double)targetSize.Width / sourceSize.Width;
		var scaleY = (double)targetSize.Height / sourceSize.Height;
		
		var scale = mode switch
		{
			ResizeModeEnum.Max or ResizeModeEnum.Pad or ResizeModeEnum.BoxPad => Math.Min(scaleX, scaleY),
			_ => Math.Max(scaleX, scaleY)
		};
		
		const double DecodeScaleThreshold = 0.5;
		if (scale >= DecodeScaleThreshold)
		{
			return null;
		}
		
		var width = Math.Max(1, (int)Math.Ceiling(sourceSize.Width * scale));
		var height = Math.Max(1, (int)Math.Ceiling(sourceSize.Height * scale));
		
		return new Size(width, height);
	}
	
	#endregion
}