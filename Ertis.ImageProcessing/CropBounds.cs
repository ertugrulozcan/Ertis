using System.Net;
using SixLabors.ImageSharp;
using ImageProcessingException = Ertis.ImageProcessing.Exceptions.ImageProcessingException;

// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.ImageProcessing;

public class CropBounds
{
	#region Properties

	public int? X { get; init; }
	
	public int? Y { get; init; }
	
	public int? Width { get; init; }
	
	public int? Height { get; init; }

	#endregion

	#region Methods

	public Rectangle ToRectangle(int originalWidth, int originalHeight)
	{
		var width = this.Width ?? 0;
		var height = this.Height ?? 0;
		var x = this.X ?? 0;
		var y = this.Y ?? 0;
		
		if (x + width > originalWidth || y + height > originalHeight)
		{
			throw new ImageProcessingException(HttpStatusCode.BadRequest, $"Crop rectangle should be smaller than the source bounds ({originalWidth}x{originalHeight})", "CropBoundsOverflow");	
		}
		
		return new Rectangle(x, y, this.Width ?? originalWidth - x, this.Height ?? originalHeight - y);
	}

	#endregion
}