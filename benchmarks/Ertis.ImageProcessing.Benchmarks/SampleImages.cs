using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

namespace Ertis.ImageProcessing.Benchmarks;

/// <summary>
/// Real photographs (CC0, Wikimedia Commons), downloaded once into a cache folder outside the repository
/// </summary>
public static class SampleImages
{
	#region Constants
	
	/// <summary>
	/// A detailed outdoor photo, 3608x2184 (https://commons.wikimedia.org/wiki/File:Cemetery_of_the_Bayezid_II_Mosque_01.jpg, CC0)
	/// </summary>
	public const string Detailed = "detailed.jpg";
	
	/// <summary>
	/// A phone photo, 4032x3024 (https://commons.wikimedia.org/wiki/File:Stary_Sanov_-_Teplice_Cc-zero_IMG_8762.JPG, CC0)
	/// </summary>
	public const string Phone = "phone.jpg";
	
	/// <summary>
	/// The phone photo stored like a portrait camera photo: the pixels rotated counterclockwise (3024x4032) with the EXIF orientation 6, displayed as the original (4032x3024)
	/// </summary>
	public const string Rotated = "rotated.jpg";
	
	private static readonly Dictionary<string, string> Sources = new()
	{
		[Detailed] = "https://upload.wikimedia.org/wikipedia/commons/a/ac/Cemetery_of_the_Bayezid_II_Mosque_01.jpg",
		[Phone] = "https://upload.wikimedia.org/wikipedia/commons/e/e3/Stary_Sanov_-_Teplice_Cc-zero_IMG_8762.JPG"
	};
	
	#endregion
	
	#region Properties
	
	// ReSharper disable once MemberCanBePrivate.Global
	public static string Folder => Environment.GetEnvironmentVariable("ERTIS_BENCHMARK_ASSETS") ?? Path.Combine(Path.GetTempPath(), "ertis-benchmark-assets");
	
	#endregion
	
	#region Methods
	
	public static byte[] Load(string name)
	{
		var path = Path.Combine(Folder, name);
		if (!File.Exists(path))
		{
			Prepare();
		}
		
		return File.ReadAllBytes(path);
	}
	
	public static void Prepare()
	{
		Directory.CreateDirectory(Folder);
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ErtisImageProcessingBenchmarks/1.0 (https://www.nuget.org/packages/Ertis.ImageProcessing)");
		foreach (var (name, url) in Sources)
		{
			var path = Path.Combine(Folder, name);
			if (!File.Exists(path))
			{
				File.WriteAllBytes(path, httpClient.GetByteArrayAsync(url).GetAwaiter().GetResult());
			}
		}
		
		var rotatedPath = Path.Combine(Folder, Rotated);
		if (!File.Exists(rotatedPath))
		{
			// Orientation 6 means "rotate 90 degrees clockwise to display", so the stored pixels are rotated counterclockwise
			using var image = Image.Load(Path.Combine(Folder, Phone));
			image.Mutate(x => x.Rotate(RotateMode.Rotate270));
			image.Metadata.ExifProfile ??= new ExifProfile();
			image.Metadata.ExifProfile.SetValue(ExifTag.Orientation, (ushort) 6);
			image.SaveAsJpeg(rotatedPath, new JpegEncoder { Quality = 92 });
		}
	}
	
	#endregion
}
