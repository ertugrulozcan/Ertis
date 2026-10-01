using BenchmarkDotNet.Running;
using Ertis.ImageProcessing.Benchmarks;

// Quality: dotnet run -c Release --project Ertis.ImageProcessing.Benchmarks -- --quality
// Run all: dotnet run -c Release --project Ertis.ImageProcessing.Benchmarks -- --filter '*'
// The sample photos are downloaded into the temp folder on the first run (ERTIS_BENCHMARK_ASSETS overrides it)
if (args.Contains("--quality"))
{
	await QualityReport.RunAsync();
	return;
}

SampleImages.Prepare();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);