using System.Net;
using SixLabors.ImageSharp;
using ImageProcessingException = Ertis.ImageProcessing.Exceptions.ImageProcessingException;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
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
		var x = this.X ?? 0;
		var y = this.Y ?? 0;
		var width = this.Width ?? originalWidth - x;
		var height = this.Height ?? originalHeight - y;
		
		if (x < 0 || y < 0 || width <= 0 || height <= 0 || x + width > originalWidth || y + height > originalHeight)
		{
			throw new ImageProcessingException(HttpStatusCode.BadRequest, $"Crop rectangle should be within the source bounds ({originalWidth}x{originalHeight})", "CropBoundsOverflow");	
		}
		
		return new Rectangle(x, y, width, height);
	}
	
	#endregion
}