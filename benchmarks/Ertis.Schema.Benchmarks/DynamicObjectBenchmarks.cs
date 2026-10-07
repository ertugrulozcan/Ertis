using BenchmarkDotNet.Attributes;

using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Benchmarks;

[MemoryDiagnoser]
public class DynamicObjectBenchmarks
{
	#region Fields
	
	private DynamicObject _document = null!;
	private Member _member = null!;
	
	#endregion
	
	#region Setup
	
	[GlobalSetup]
	public void Setup()
	{
		this._document = DynamicObject.Parse(SampleData.MEMBER_JSON);
		this._member = SampleData.CreateMember();
	}
	
	#endregion
	
	#region Benchmarks
	
	[Benchmark]
	public DynamicObject Parse()
	{
		return DynamicObject.Parse(SampleData.MEMBER_JSON);
	}
	
	[Benchmark]
	public string ToJson()
	{
		return this._document.ToJson();
	}
	
	[Benchmark]
	public Member? Deserialize()
	{
		return this._document.Deserialize<Member>();
	}
	
	[Benchmark]
	public object Clone()
	{
		return this._document.Clone();
	}
	
	[Benchmark]
	public DynamicObject CreateFromPoco()
	{
		return new DynamicObject(this._member);
	}
	
	[Benchmark]
	public object? GetValue()
	{
		this._document.GetValue("firstname");
		this._document.GetValue("address.city");
		return this._document.GetValue("phones[1].number");
	}
	
	[Benchmark]
	public bool TryGetValueOfT()
	{
		return this._document.TryGetValue<string>("address.city", out _);
	}
	
	[Benchmark]
	public void SetValue()
	{
		this._document.SetValue("address.city", "Ankara");
	}
	
	#endregion
}
