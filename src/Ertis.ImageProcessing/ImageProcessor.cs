using System.Diagnostics.CodeAnalysis;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;
using ImageProcessingException = Ertis.ImageProcessing.Exceptions.ImageProcessingException;

// ReSharper disable once UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.ImageProcessing;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
public static class ImageProcessor
{
	#region Methods
	
	public static void Crop(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat, int? quality = null, int? level = null, bool stripMetadata = false)
	{
		try
		{
			var encoder = FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level);
			using var image = Image.Load(imageStream);
			CropCore(image, bounds, stripMetadata);
			image.Save(outputStream, encoder);
		}
		catch (Exception ex) when (ToImageProcessingException(ex) is { } exception)
		{
			throw exception;
		}
	}
	
	public static async Task CropAsync(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat, int? quality = null, int? level = null, bool stripMetadata = false, CancellationToken cancellationToken = default)
	{
		try
		{
			var encoder = FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level);
			using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
			CropCore(image, bounds, stripMetadata);
			await image.SaveAsync(outputStream, encoder, cancellationToken: cancellationToken);
		}
		catch (Exception ex) when (ToImageProcessingException(ex) is { } exception)
		{
			throw exception;
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
		int? level = null,
		TargetSizeMode targetSizeMode = TargetSizeMode.Auto,
		bool stripMetadata = false,
		ResizeQuality resizeQuality = ResizeQuality.Balanced)
	{
		ResizeAsync(imageStream, outputStream, width, height, destinationFormat, mode, anchor, sampler, quality, level, targetSizeMode, stripMetadata, resizeQuality).ConfigureAwait(false).GetAwaiter().GetResult();
	}
	
	/// <summary>
	/// Resizes the image; without a width and a height the image is converted to the destination format
	/// </summary>
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
		TargetSizeMode targetSizeMode = TargetSizeMode.Auto, 
		bool stripMetadata = false,
		ResizeQuality resizeQuality = ResizeQuality.Balanced,
		CancellationToken cancellationToken = default)
	{
		if (width is <= 0 || height is <= 0)
		{
			throw new ImageProcessingException(HttpStatusCode.BadRequest, "Width and height must be greater than zero.", "InvalidDimensions");
		}
		
		if (width == null && height == null)
		{
			await ConvertAsync(imageStream, outputStream, destinationFormat, quality, level, stripMetadata, cancellationToken);
			return;
		}
		
		MemoryStream? bufferedStream = null;
		try
		{
			var encoder = FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level);
			
			var resizeMode = mode ?? ResizeModeEnum.Crop;
			var resampler = (sampler ?? SamplerAlgorithm.Bicubic).ToResampler()!;
			DecoderOptions decoderOptions;
			int targetWidth, targetHeight;
			if (targetSizeMode == TargetSizeMode.Auto)
			{
				// The image is identified first (for the decoder target size), then decoded from the same position
				if (!imageStream.CanSeek)
				{
					bufferedStream = new MemoryStream();
					await imageStream.CopyToAsync(bufferedStream, cancellationToken);
					bufferedStream.Position = 0;
					imageStream = bufferedStream;
				}
				
				var startPosition = imageStream.Position;
				var sourceInfo = await Image.IdentifyAsync(imageStream, cancellationToken);
				imageStream.Position = startPosition;
				
				decoderOptions = GetDecoderOptions(sourceInfo, resizeMode, resampler, resizeQuality, width, height, out targetWidth, out targetHeight);
			}
			else
			{
				// The whole image is decoded, a missing dimension (0) keeps the aspect ratio
				decoderOptions = new DecoderOptions();
				targetWidth = width ?? 0;
				targetHeight = height ?? 0;
			}
			
			using var image = await Image.LoadAsync(decoderOptions, imageStream, cancellationToken: cancellationToken);
			Prepare(image, stripMetadata);
			var options = new ResizeOptions
			{
				Size = new Size(targetWidth, targetHeight),
				Mode = resizeMode,
				Position = anchor ?? AnchorPositionMode.Center,
				Sampler = resampler
			};
			
			image.Mutate(x => x.Resize(options)); 
			await image.SaveAsync(outputStream, encoder, cancellationToken: cancellationToken);
		}
		catch (Exception ex) when (ToImageProcessingException(ex) is { } exception)
		{
			throw exception;
		}
		finally
		{
			if (bufferedStream != null)
			{
				await bufferedStream.DisposeAsync();
			}
		}
	}
	
	public static void Convert(Stream imageStream, Stream outputStream, ImageFormat destinationFormat, int? quality = null, int? level = null, bool stripMetadata = false)
	{
		try
		{
			var encoder = FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level);
			using var image = Image.Load(imageStream);
			Prepare(image, stripMetadata);
			image.Save(outputStream, encoder);
		}
		catch (Exception ex) when (ToImageProcessingException(ex) is { } exception)
		{
			throw exception;
		}
	}
	
	public static async Task ConvertAsync(Stream imageStream, Stream outputStream, ImageFormat destinationFormat, int? quality = null, int? level = null, bool stripMetadata = false, CancellationToken cancellationToken = default)
	{
		try
		{
			var encoder = FormatEncoder.GetDefaultFormatter(destinationFormat, quality, level);
			using var image = await Image.LoadAsync(imageStream, cancellationToken: cancellationToken);
			Prepare(image, stripMetadata);
			await image.SaveAsync(outputStream, encoder, cancellationToken: cancellationToken);
		}
		catch (Exception ex) when (ToImageProcessingException(ex) is { } exception)
		{
			throw exception;
		}
	}
	
	/// <summary>
	/// Reads the metadata without decoding the pixels
	/// </summary>
	public static async Task<ImageMetadata?> GetMetadataAsync(Stream imageStream, CancellationToken cancellationToken = default)
	{
		try
		{
			var imageInfo = await Image.IdentifyAsync(imageStream, cancellationToken);
			return imageInfo.Metadata;
		}
		catch (Exception ex) when (ToImageProcessingException(ex) is { } exception)
		{
			throw exception;
		}
	}
	
	/// <summary>
	/// Crops the image by the bounds given in the displayed (EXIF oriented) coordinates: the stored image is cropped first, then only the cropped part is oriented
	/// </summary>
	private static void CropCore(Image image, CropBounds bounds, bool stripMetadata)
	{
		var orientation = GetOrientation(image.Metadata);
		var isRotated = orientation is >= ExifOrientationMode.LeftTop and <= ExifOrientationMode.LeftBottom;
		var rectangle = isRotated ? bounds.ToRectangle(image.Height, image.Width) : bounds.ToRectangle(image.Width, image.Height);
		var storedRectangle = ToStoredRectangle(rectangle, orientation, image.Width, image.Height);
		image.Mutate(x => x.Crop(storedRectangle));
		Prepare(image, stripMetadata);
	}
	
	/// <summary>
	/// The rectangle of the stored image which is displayed as the given rectangle (the bounding box of its mapped corner pixels)
	/// </summary>
	private static Rectangle ToStoredRectangle(Rectangle rectangle, ushort orientation, int storedWidth, int storedHeight)
	{
		var (x1, y1) = ToStoredPixel(rectangle.Left, rectangle.Top, orientation, storedWidth, storedHeight);
		var (x2, y2) = ToStoredPixel(rectangle.Right - 1, rectangle.Bottom - 1, orientation, storedWidth, storedHeight);
		return Rectangle.FromLTRB(Math.Min(x1, x2), Math.Min(y1, y2), Math.Max(x1, x2) + 1, Math.Max(y1, y2) + 1);
	}
	
	/// <summary>
	/// The stored pixel which is displayed at the given pixel
	/// </summary>
	private static (int X, int Y) ToStoredPixel(int x, int y, ushort orientation, int storedWidth, int storedHeight)
	{
		return orientation switch
		{
			ExifOrientationMode.TopRight => (storedWidth - 1 - x, y),
			ExifOrientationMode.BottomRight => (storedWidth - 1 - x, storedHeight - 1 - y),
			ExifOrientationMode.BottomLeft => (x, storedHeight - 1 - y),
			ExifOrientationMode.LeftTop => (y, x),
			ExifOrientationMode.RightTop => (y, storedHeight - 1 - x),
			ExifOrientationMode.RightBottom => (storedWidth - 1 - y, storedHeight - 1 - x),
			ExifOrientationMode.LeftBottom => (storedWidth - 1 - y, x),
			_ => (x, y)
		};
	}
	
	private static ushort GetOrientation(ImageMetadata metadata)
	{
		return metadata.ExifProfile != null && metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientation) ? orientation.Value : ExifOrientationMode.Unknown;
	}
	
	/// <summary>
	/// Applies the EXIF orientation (the pixels are processed as they are displayed) and removes the metadata if requested
	/// </summary>
	private static void Prepare(Image image, bool stripMetadata)
	{
		image.Mutate(x => x.AutoOrient());
		if (stripMetadata)
		{
			// The color profile (ICC) is kept, it is needed to display the colors correctly
			image.Metadata.ExifProfile = null;
			image.Metadata.IptcProfile = null;
			image.Metadata.XmpProfile = null;
		}
	}
	
	/// <summary>
	/// The exception thrown to the caller; null to rethrow the exception as it is
	/// </summary>
	private static ImageProcessingException? ToImageProcessingException(Exception ex)
	{
		return ex switch
		{
			ImageProcessingException or OperationCanceledException or OutOfMemoryException => null,
			InvalidMemoryOperationException or InvalidImageContentException { InnerException: InvalidMemoryOperationException } => 
				new ImageProcessingException(HttpStatusCode.ServiceUnavailable, "Image is too large to process.", "ImageTooLarge", ex),
			InvalidImageContentException or UnknownImageFormatException or NotSupportedException => 
				new ImageProcessingException(HttpStatusCode.BadRequest, "The image could not be decoded.", "InvalidImageContent", ex),
			_ => new ImageProcessingException(HttpStatusCode.InternalServerError, ex.Message, "ImageProcessingError", ex)
		};
	}
	
	private static DecoderOptions GetDecoderOptions(ImageInfo sourceInfo, ResizeModeEnum resizeMode, IResampler resampler, ResizeQuality resizeQuality, int? width, int? height, out int targetWidth, out int targetHeight)
	{
		// The size as the image is displayed (the EXIF orientations 5-8 swap the width and the height)
		var isRotated = IsRotated(sourceInfo);
		var sourceWidth = isRotated ? sourceInfo.Height : sourceInfo.Width;
		var sourceHeight = isRotated ? sourceInfo.Width : sourceInfo.Height;
		
		targetWidth = width ?? Math.Max(1, (int)Math.Round(sourceWidth * ((double)height!.Value / sourceHeight)));
		targetHeight = height ?? Math.Max(1, (int)Math.Round(sourceHeight * ((double)width!.Value / sourceWidth)));
		
		var targetSize = GetDecoderTargetSize(new Size(sourceWidth, sourceHeight), new Size(targetWidth, targetHeight), resizeMode, resizeQuality);
		if (targetSize != null && isRotated)
		{
			// The decoder works on the stored (not rotated) image
			targetSize = new Size(targetSize.Value.Height, targetSize.Value.Width);
		}
		
		// The decoder resizes its scaled image to the target size with the requested sampler (its default is the Box sampler)
		return new DecoderOptions
		{
			TargetSize = targetSize,
			Sampler = resampler
		};
	}
	
	private static bool IsRotated(ImageInfo imageInfo)
	{
		return GetOrientation(imageInfo.Metadata) is >= ExifOrientationMode.LeftTop and <= ExifOrientationMode.LeftBottom;
	}
	
	private static Size? GetDecoderTargetSize(Size sourceSize, Size targetSize, ResizeModeEnum mode, ResizeQuality resizeQuality)
	{
		var scaleX = (double)targetSize.Width / sourceSize.Width;
		var scaleY = (double)targetSize.Height / sourceSize.Height;
		
		var scale = mode switch
		{
			ResizeModeEnum.Max or ResizeModeEnum.Pad or ResizeModeEnum.BoxPad => Math.Min(scaleX, scaleY),
			_ => Math.Max(scaleX, scaleY)
		};
		
		// High: the decoder scales down to twice the target size at most, the last halving is done by the requested sampler
		if (resizeQuality == ResizeQuality.High)
		{
			scale *= 2;
		}
		
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