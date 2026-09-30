using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using Ertis.Schema.Serialization;

using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Benchmarks;

/// <summary>
/// DynamicObjectJsonConverter inside a model, like the request/response bodies of the consumers
/// </summary>
[MemoryDiagnoser]
public class ConverterBenchmarks
{
	#region Fields
	
	private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
	{
		Converters = { new DynamicObjectJsonConverter() }
	};
	
	private string _json = null!;
	private EventModel _model = null!;
	
	#endregion
	
	#region Setup
	
	[GlobalSetup]
	public void Setup()
	{
		this._json = $$"""{ "id": "1", "document": {{SampleData.MEMBER_JSON}}, "prior": {{SampleData.MEMBER_JSON}} }""";
		this._model = JsonSerializer.Deserialize<EventModel>(this._json, Options)!;
	}
	
	#endregion
	
	#region Benchmarks
	
	[Benchmark]
	public EventModel? Read()
	{
		return JsonSerializer.Deserialize<EventModel>(this._json, Options);
	}
	
	[Benchmark]
	public string Write()
	{
		return JsonSerializer.Serialize(this._model, Options);
	}
	
	#endregion
	
	#region Models
	
	public sealed class EventModel
	{
		[JsonPropertyName("id")]
		public string? Id { get; set; }
		
		[JsonPropertyName("document")]
		public DynamicObject? Document { get; set; }
		
		[JsonPropertyName("prior")]
		public DynamicObject? Prior { get; set; }
	}
	
	#endregion
}
