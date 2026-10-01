using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace Ertis.ImageProcessing.Tests.TestHelpers;

public static class TestImages
{
	#region Methods
	
	/// <summary>
	/// A png (or another format) image: the left half red, the right half blue; with an optional EXIF orientation and GPS tag
	/// </summary>
	public static MemoryStream Create(int width, int height, IImageEncoder? encoder = null, ushort? orientation = null, bool withGps = false)
	{
		using var image = new Image<Rgba32>(width, height);
		image.ProcessPixelRows(accessor =>
		{
			for (var y = 0; y < accessor.Height; y++)
			{
				var row = accessor.GetRowSpan(y);
				for (var x = 0; x < row.Length; x++)
				{
					row[x] = x < width / 2 ? Color.Red : Color.Blue;
				}
			}
		});
		
		if (orientation != null || withGps)
		{
			image.Metadata.ExifProfile = new ExifProfile();
			if (orientation != null)
			{
				image.Metadata.ExifProfile.SetValue(ExifTag.Orientation, orientation.Value);
			}
			
			if (withGps)
			{
				image.Metadata.ExifProfile.SetValue(ExifTag.GPSLatitudeRef, "N");
			}
		}
		
		var stream = new MemoryStream();
		image.Save(stream, encoder ?? new SixLabors.ImageSharp.Formats.Png.PngEncoder());
		stream.Position = 0;
		return stream;
	}
	
	public static Image Load(MemoryStream stream)
	{
		stream.Position = 0;
		return Image.Load(stream);
	}
	
	public static IImageFormat DetectFormat(MemoryStream stream)
	{
		stream.Position = 0;
		return Image.DetectFormat(stream);
	}
	
	#endregion
}

/// <summary>
/// A stream which can not seek (e.g. a network stream)
/// </summary>
public sealed class NonSeekableStream(Stream inner) : Stream
{
	public override bool CanRead => true;
	public override bool CanSeek => false;
	public override bool CanWrite => false;
	public override long Length => throw new NotSupportedException();
	public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
	public override void Flush() { }
	public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
	public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
	public override void SetLength(long value) => throw new NotSupportedException();
	public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
