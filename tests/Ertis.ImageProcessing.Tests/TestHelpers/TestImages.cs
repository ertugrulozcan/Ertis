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
	
	/// <summary>
	/// A detailed (textured) jpeg photo-like image
	/// </summary>
	public static byte[] CreateDetailedJpeg(int width, int height)
	{
		using var image = new Image<Rgb24>(width, height);
		var random = new Random(42);
		image.ProcessPixelRows(accessor =>
		{
			for (var y = 0; y < accessor.Height; y++)
			{
				var row = accessor.GetRowSpan(y);
				for (var x = 0; x < row.Length; x++)
				{
					var stripe = (x / 3 + y / 5) % 2 == 0 ? 60 : 0;
					row[x] = new Rgb24((byte) (x * 255 / width), (byte) Math.Clamp(y * 255 / height + stripe, 0, 255), (byte) random.Next(80, 120));
				}
			}
		});
		
		using var stream = new MemoryStream();
		image.SaveAsJpeg(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 90 });
		return stream.ToArray();
	}
	
	/// <summary>
	/// A png of random pixels (every position is distinguishable) with an EXIF orientation
	/// </summary>
	public static byte[] CreateRandomPng(int width, int height, ushort orientation)
	{
		using var image = new Image<Rgb24>(width, height);
		var random = new Random(orientation);
		image.ProcessPixelRows(accessor =>
		{
			for (var y = 0; y < accessor.Height; y++)
			{
				var row = accessor.GetRowSpan(y);
				for (var x = 0; x < row.Length; x++)
				{
					row[x] = new Rgb24((byte) random.Next(256), (byte) random.Next(256), (byte) random.Next(256));
				}
			}
		});
		
		image.Metadata.ExifProfile = new ExifProfile();
		image.Metadata.ExifProfile.SetValue(ExifTag.Orientation, orientation);
		using var stream = new MemoryStream();
		image.SaveAsPng(stream);
		return stream.ToArray();
	}
	
	public static double Psnr(Image<Rgb24> expected, Image<Rgb24> actual)
	{
		double sum = 0;
		long count = 0;
		for (var y = 0; y < expected.Height; y++)
		{
			for (var x = 0; x < expected.Width; x++)
			{
				var a = expected[x, y];
				var b = actual[x, y];
				sum += Math.Pow(a.R - b.R, 2) + Math.Pow(a.G - b.G, 2) + Math.Pow(a.B - b.B, 2);
				count += 3;
			}
		}
		
		var mse = sum / count;
		return mse == 0 ? double.PositiveInfinity : 10 * Math.Log10(255 * 255 / mse);
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
