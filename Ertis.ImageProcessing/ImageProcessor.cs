using System.Diagnostics.CodeAnalysis;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;

namespace Ertis.ImageProcessing;

public static class ImageProcessor
{
	#region Methods

	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public static void Crop(Stream imageStream, Stream outputStream, CropBounds bounds, ImageFormat destinationFormat)
	{
		try
		{
			using (var image = Image.Load(imageStream))
			{
				image.Mutate(x => x.Crop(bounds.ToRectangle(image.Width, image.Height))); 
				image.Save(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat));
			}
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public static void Resize(Stream imageStream, Stream outputStream, int? width, int? height, ImageFormat destinationFormat, ResizeMode? mode = null, Anchor? anchor = null, SamplerAlgorithm? sampler = null)
	{
		if (width == null && height == null)
		{
			return;
		}

		try
		{
			using (var image = Image.Load(imageStream))
			{
				var options = new ResizeOptions
				{
					Size = new Size(width ?? 0, height ?? 0),
					Mode = mode ?? ResizeModeEnum.Crop,
					Position = anchor ?? AnchorPositionMode.Center,
					Sampler = (sampler ?? SamplerAlgorithm.Bicubic).ToResampler()
				};
			
				image.Mutate(x => x.Resize(options)); 
				image.Save(outputStream, FormatEncoder.GetDefaultFormatter(destinationFormat)); 
			}
		}
		catch (Exception ex)
		{
			throw new Ertis.ImageProcessing.Exceptions.ImageProcessingException(HttpStatusCode.BadRequest, ex.Message, "ImageProcessingError");
		}
	}
	
	#endregion
}