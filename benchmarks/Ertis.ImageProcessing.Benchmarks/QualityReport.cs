using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;

namespace Ertis.ImageProcessing.Benchmarks;

/// <summary>
/// Compares the resize outputs with a reference (the full image decoded and resized with the same sampler), as PSNR in dB:
/// the higher the closer to the reference (identical images: infinity)
/// </summary>
public static class QualityReport
{
	#region Methods
	
	public static async Task RunAsync()
	{
		var samplers = new (string Name, SamplerAlgorithm Sampler, IResampler Resampler)[]
		{
			("Bicubic", SamplerAlgorithm.Bicubic, KnownResamplers.Bicubic),
			("Lanczos", SamplerAlgorithm.Lanczos, KnownResamplers.Lanczos3)
		};
		
		Console.WriteLine($"{"Image",-12} {"Quality",-9} {"Sampler",-8} {"Width",6} {"Height",6} {"PSNR (dB)",10}");
		// 4:3 photos: the target sizes (and the decoder sizes) keep the aspect ratio exactly, so the outputs are not shifted by the rounding
		foreach (var name in new[] { SampleImages.Phone, SampleImages.Rotated })
		{
			var bytes = SampleImages.Load(name);
			foreach (var resizeQuality in Enum.GetValues<ResizeQuality>())
			{
				foreach (var (samplerName, sampler, resampler) in samplers)
				{
					foreach (var width in new[] { 1600, 800, 400, 200 })
					{
						using var output = new MemoryStream();
						await ImageProcessor.ResizeAsync(new MemoryStream(bytes), output, width, null, ImageFormat.PNG, sampler: sampler, resizeQuality: resizeQuality);
						output.Position = 0;
						using var result = await Image.LoadAsync<Rgb24>(output);
						
						using var reference = Image.Load<Rgb24>(bytes);
						
						var resultWidth = result.Width;
						var resultHeight = result.Height;
						reference.Mutate(x => x.AutoOrient().Resize(new ResizeOptions { Size = new Size(resultWidth, resultHeight), Mode = ResizeModeEnum.Crop, Sampler = resampler }));
						
						Console.WriteLine($"{name,-12} {resizeQuality,-9} {samplerName,-8} {result.Width,6} {result.Height,6} {Psnr(reference, result),10:F2}");
					}
				}
			}
		}
	}
	
	private static double Psnr(Image<Rgb24> expected, Image<Rgb24> actual)
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
	
	#endregion
}
