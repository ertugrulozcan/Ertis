using BenchmarkDotNet.Running;

// Run all: dotnet run -c Release --project Ertis.Schema.Benchmarks -- --filter '*'
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
