using System.Diagnostics.CodeAnalysis;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;
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
			Prepare(image, stripMetadata);
			var rectangle = bounds.ToRectangle(image.Width, image.Height);
			image.Mutate(x => x.Crop(rectangle));
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
			Prepare(image, stripMetadata);
			var rectangle = bounds.ToRectangle(image.Width, image.Height);
			image.Mutate(x => x.Crop(rectangle));
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
		bool stripMetadata = false)
	{
		ResizeAsync(imageStream, outputStream, width, height, destinationFormat, mode, anchor, sampler, quality, level, targetSizeMode, stripMetadata).ConfigureAwait(false).GetAwaiter().GetResult();
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
			
			var resizeMode = mode ?? ResizeModeEnum.Crop;
			var decoderOptions = GetDecoderOptions(sourceInfo, targetSizeMode, resizeMode, width, height, out var targetWidth, out var targetHeight);
			using var image = await Image.LoadAsync(decoderOptions, imageStream, cancellationToken: cancellationToken);
			Prepare(image, stripMetadata);
			var options = new ResizeOptions
			{
				Size = new Size(targetWidth, targetHeight),
				Mode = resizeMode,
				Position = anchor ?? AnchorPositionMode.Center,
				Sampler = (sampler ?? SamplerAlgorithm.Bicubic).ToResampler()!
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
	
	private static DecoderOptions GetDecoderOptions(ImageInfo sourceInfo, TargetSizeMode targetSizeMode, ResizeModeEnum resizeMode, int? width, int? height, out int targetWidth, out int targetHeight)
	{
		Size? targetSize = null;
		if (targetSizeMode == TargetSizeMode.Auto)
		{
			// The size as the image is displayed (the EXIF orientations 5-8 swap the width and the height)
			var isRotated = IsRotated(sourceInfo);
			var sourceWidth = isRotated ? sourceInfo.Height : sourceInfo.Width;
			var sourceHeight = isRotated ? sourceInfo.Width : sourceInfo.Height;
			
			targetWidth = width ?? Math.Max(1, (int)Math.Round(sourceWidth * ((double)height!.Value / sourceHeight)));
			targetHeight = height ?? Math.Max(1, (int)Math.Round(sourceHeight * ((double)width!.Value / sourceWidth)));
			
			targetSize = GetDecoderTargetSize(new Size(sourceWidth, sourceHeight), new Size(targetWidth, targetHeight), resizeMode);
			if (targetSize != null && isRotated)
			{
				// The decoder works on the stored (not rotated) image
				targetSize = new Size(targetSize.Value.Height, targetSize.Value.Width);
			}
		}
		else
		{
			targetWidth = width ?? 0;
			targetHeight = height ?? 0;
		}
		
		return new DecoderOptions
		{
			TargetSize = targetSize
		};
	}
	
	private static bool IsRotated(ImageInfo imageInfo)
	{
		return imageInfo.Metadata.ExifProfile != null && 
			imageInfo.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientation) && 
			orientation.Value is >= ExifOrientationMode.LeftTop and <= ExifOrientationMode.LeftBottom;
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