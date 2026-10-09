using BenchmarkDotNet.Attributes;
using Ertis.Schema.Validation;
using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Benchmarks;

[MemoryDiagnoser]
public class ValidationBenchmarks
{
	#region Fields
	
	private MemberSchema _schema = null!;
	private DynamicObject _document = null!;
	
	#endregion
	
	#region Setup
	
	[GlobalSetup]
	public void Setup()
	{
		this._schema = SampleData.CreateSchema();
		this._document = DynamicObject.Parse(SampleData.MEMBER_JSON);
	}
	
	#endregion
	
	#region Benchmarks
	
	/// <summary>
	/// The validation of an already parsed document (the document keeps the values set by the first validation)
	/// </summary>
	[Benchmark]
	public bool Validate()
	{
		return this._schema.ValidateContent(this._document, new FieldValidationContext(this._document));
	}
	
	/// <summary>
	/// A request body: parse + validate
	/// </summary>
	[Benchmark]
	public bool ParseAndValidate()
	{
		var document = DynamicObject.Parse(SampleData.MEMBER_JSON);
		return this._schema.ValidateContent(document, new FieldValidationContext(document));
	}
	
	#endregion
}
