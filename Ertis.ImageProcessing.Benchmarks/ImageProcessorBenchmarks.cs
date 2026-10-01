using BenchmarkDotNet.Attributes;

namespace Ertis.ImageProcessing.Benchmarks;

/// <summary>
/// The typical media api requests on real photos
/// </summary>
[MemoryDiagnoser]
public class ImageProcessorBenchmarks
{
	#region Fields
	
	private byte[] _image = [];
	
	#endregion
	
	#region Properties
	
	[Params(SampleImages.Detailed, SampleImages.Phone, SampleImages.Rotated)]
	public string Image { get; set; } = SampleImages.Detailed;
	
	[Params(ResizeQuality.Balanced, ResizeQuality.High)]
	public ResizeQuality ResizeQuality { get; set; }
	
	#endregion
	
	#region Methods
	
	[GlobalSetup]
	public void Setup()
	{
		this._image = SampleImages.Load(this.Image);
	}
	
	[Benchmark]
	public Task<long> ResizeTo1200Jpeg() => this.ResizeAsync(1200, null, ImageFormat.Jpeg);
	
	[Benchmark]
	public Task<long> ResizeTo800Webp() => this.ResizeAsync(800, null, ImageFormat.Webp);
	
	[Benchmark]
	public Task<long> ThumbnailTo200x200Jpeg() => this.ResizeAsync(200, 200, ImageFormat.Jpeg);
	
	[Benchmark]
	public async Task<long> Crop1000x1000Jpeg()
	{
		using var input = new MemoryStream(this._image);
		using var output = new MemoryStream();
		await ImageProcessor.CropAsync(input, output, new CropBounds { X = 500, Y = 500, Width = 1000, Height = 1000 }, ImageFormat.Jpeg, quality: 75);
		return output.Length;
	}
	
	private async Task<long> ResizeAsync(int? width, int? height, ImageFormat format)
	{
		using var input = new MemoryStream(this._image);
		using var output = new MemoryStream();
		await ImageProcessor.ResizeAsync(input, output, width, height, format, quality: 75, resizeQuality: this.ResizeQuality);
		return output.Length;
	}
	
	#endregion
}
