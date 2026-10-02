using System.Collections;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Ertis.TemplateEngine.Tests.TestHelpers;
using Microsoft.AspNetCore.Routing;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.TemplateEngine.Tests;

public class FormatterTests
{
	#region Fields
	
	private static readonly object SampleData = new
	{
		name = "Jane",
		user = new { address = new { city = "Istanbul" } },
		tags = new[] { "a", "b" },
		items = new[] { new { n = 1 }, new { n = 2 } },
		ratio = 1.5,
		date = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc),
		flag = true,
		nothing = (string?) null
	};
	
	#endregion
	
	#region Placeholder Methods
	
	[Theory]
	[InlineData("Hi {{name}}!", "Hi Jane!")]
	[InlineData("Hi {{ name }}!", "Hi Jane!")]
	[InlineData("{{user.address.city}}", "Istanbul")]
	[InlineData("{{tags[1]}}", "b")]
	[InlineData("{{items[1].n}}", "2")]
	[InlineData("{{flag}}", "True")]
	[InlineData("{{name}} and {{name}}", "Jane and Jane")]
	[InlineData("no placeholders", "no placeholders")]
	public void Format_ReplacesThePlaceholders(string template, string expected)
	{
		Assert.Equal(expected, new Formatter().Format(template, SampleData));
	}
	
	[Fact]
	public void Format_WritesTheValuesWithTheCurrentCulture()
	{
		using var _ = new CultureScope("tr-TR");
		
		Assert.Equal("1,5 31.01.2026 10:00:00", new Formatter().Format("{{ratio}} {{date}}", SampleData));
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void Format_WithAnEmptyTemplate_ReturnsTheTemplate(string? template)
	{
		Assert.Equal(template, new Formatter().Format(template!, SampleData));
	}
	
	[Fact]
	public void Format_WithoutData_ReturnsTheTemplate()
	{
		Assert.Equal("{{name}}", new Formatter().Format("{{name}}", null));
	}
	
	[Fact]
	public void Format_DoesNotEncodeTheValues()
	{
		Assert.Equal("<b>x</b> & \"q\"", new Formatter().Format("{{name}}", new { name = "<b>x</b> & \"q\"" }));
	}
	
	[Fact]
	public void Format_WithAFormatProvider_WritesTheValuesWithIt()
	{
		using var _ = new CultureScope("tr-TR");
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", FormatProvider = CultureInfo.InvariantCulture });
		
		Assert.Equal("1.5 01/31/2026 10:00:00 True", formatter.Format("{{ratio}} {{date}} {{flag}}", SampleData));
	}
	
	#endregion
	
	#region Data Shape Methods
	
	[Fact]
	public void Format_WithAnExpandoObject_ReadsItsKeys()
	{
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["event_type"] = "UserCreated";
		expando["document"] = new Dictionary<string, object?> { ["email"] = "a@b.c", ["sub"] = new Dictionary<string, object?> { ["x"] = 1 } };
		
		Assert.Equal("UserCreated a@b.c 1", new Formatter().Format("{{event_type}} {{document.email}} {{document.sub.x}}", expando));
	}
	
	[Fact]
	public void Format_WithRouteValues_ReadsThem()
	{
		var routeValues = new RouteValueDictionary { ["membershipId"] = "m1", ["id"] = "42" };
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{", CloseBrackets = "}" });
		
		Assert.Equal("users.42@m1", formatter.Format("users.{id}@{membershipId}", routeValues));
	}
	
	[Fact]
	public void Format_WithAPoco_UsesTheJsonPropertyNames()
	{
		var poco = new Poco { FirstName = "Jane", Address = new PocoAddress { City = "Ankara" }, Day = DayOfWeek.Monday };
		
		Assert.Equal("Jane Ankara 1", new Formatter().Format("{{first_name}} {{address.city}} {{Day}}", poco));
	}
	
	[Fact]
	public void Format_WithADottedKey_CanNotReadIt()
	{
		Assert.Equal("{{a.b}}", new Formatter().Format("{{a.b}}", new Dictionary<string, object?> { ["a.b"] = "flat" }));
	}
	
	[Fact]
	public void Format_WithAPoco_AppliesTheJsonContract()
	{
		var poco = new ContractPoco { Status = DayOfWeek.Friday, Secret = "hidden", Field = "f", Numbers = [1, 2, 3] };
		
		Assert.Equal("Friday f 3 {{Secret}}", new Formatter().Format("{{status}} {{Field}} {{Numbers[2]}} {{Secret}}", poco));
	}
	
	[Fact]
	public void Format_WithTypedDictionaries_ReadsThem()
	{
		var data = new Dictionary<string, object?>
		{
			["strings"] = new Dictionary<string, string> { ["a"] = "1" },
			["table"] = new Hashtable { [5] = "five" },
			["list"] = new List<Dictionary<string, object?>> { new() { ["x"] = "y" } }
		};
		
		Assert.Equal("1 five y", new Formatter().Format("{{strings.a}} {{table.5}} {{list[0].x}}", data));
	}
	
	[Fact]
	public void Format_WithJsonData_ReadsIt()
	{
		const string json = """{ "name": "Jane", "age": 30, "ratio": 1.5, "ok": true, "tags": ["a", { "k": "v" }], "none": null }""";
		const string template = "{{name}} {{age}} {{ratio}} {{ok}} {{tags[1].k}} {{none}}";
		const string expected = "Jane 30 1.5 True v {{none}}";
		
		using var _ = new CultureScope("en-US");
		using var document = JsonDocument.Parse(json);
		Assert.Equal(expected, new Formatter().Format(template, document.RootElement));
		Assert.Equal(expected, new Formatter().Format(template, JsonNode.Parse(json)));
		Assert.Equal("Jane v", new Formatter().Format("{{nested.name}} {{nested.tags[1].k}}", new { nested = JsonNode.Parse(json) }));
	}
	
	[Fact]
	public void Format_WithDatesAndCustomTypes_WritesThem()
	{
		using var _ = new CultureScope("en-US");
		var data = new { day = new DateOnly(2026, 1, 31), version = new Version(1, 2, 3), id = Guid.Empty };
		
		Assert.Equal("1/31/2026 1.2.3 00000000-0000-0000-0000-000000000000", new Formatter().Format("{{day}} {{version}} {{id}}", data));
	}
	
	[Fact]
	public void Format_WithConvertiblesWithoutBaseType_WritesThemAsStrings()
	{
		const string id = "65a0f0c2e4b0a1b2c3d4e5f6";
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["_id"] = new FakeConvertibleId(id);
		expando["document"] = new Dictionary<string, object?> { ["_id"] = new FakeConvertibleId(id) };
		expando["refs"] = new[] { new FakeConvertibleId(id) };
		
		Assert.Equal($"{id} {id} {id}", new Formatter().Format("{{_id}} {{document._id}} {{refs[0]}}", expando));
	}
	
	[Fact]
	public void Format_WithConvertiblesWithBaseType_WritesThemAsTheirBaseTypes()
	{
		using var _ = new CultureScope("tr-TR");
		var data = new { number = new FakeConvertibleNumber(1.5), text = new FakeConvertibleText("text"), dbNull = DBNull.Value };

		// DBNull is null, so its placeholder is kept like the placeholder of any null value
		Assert.Equal("1,5 text [{{dbNull}}]",new Formatter().Format("{{number}} {{text}} [{{dbNull}}]", data));
	}
	
	[Fact]
	public void Format_WithAConvertibleCollection_ReadsItAsCollection()
	{
		Assert.Equal("2", new Formatter().Format("{{items[1]}}", new { items = new FakeConvertibleCollection(1, 2) }));
	}
	
	[Theory]
	[InlineData("text")]
	[InlineData(5)]
	public void Format_WithANonObjectData_Throws(object data)
	{
		Assert.Throws<ArgumentException>(() => new Formatter().Format("{{x}}", data));
	}
	
	#endregion
	
	#region Value Encoder Methods
	
	[Fact]
	public void Format_WithAValueEncoder_EncodesTheResolvedValues()
	{
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", ValueEncoder = WebUtility.HtmlEncode });
		
		Assert.Equal("<p>&lt;b&gt;x&lt;/b&gt; & 5</p>", formatter.Format("<p>{{name}} & {{n}}</p>", new { name = "<b>x</b>", n = 5 }));
	}
	
	[Theory]
	[InlineData(UndefinedStrategy.Ignore, "<{{missing}}>")]
	[InlineData(UndefinedStrategy.Swap, "<<none>>")]
	public void Format_WithAValueEncoder_DoesNotEncodeTheUnresolvedPlaceholders(UndefinedStrategy strategy, string expected)
	{
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", UndefinedStrategy = strategy, Fallback = "<none>", ValueEncoder = WebUtility.HtmlEncode });
		
		Assert.Equal(expected, formatter.Format("<{{missing}}>", new { name = "x" }));
	}
	
	[Fact]
	public void Format_WithAValueEncoder_UsesTheCurrentCulture()
	{
		using var _ = new CultureScope("tr-TR");
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", ValueEncoder = x => $"[{x}]" });
		
		Assert.Equal("[1,5]", formatter.Format("{{ratio}}", SampleData));
	}
	
	#endregion
	
	#region Undefined Strategy Methods
	
	[Theory]
	[InlineData("x{{missing}}y")]
	[InlineData("x{{nothing}}y")]
	public void Format_WithAnUndefinedValue_KeepsThePlaceholderByDefault(string template)
	{
		Assert.Equal(template, new Formatter().Format(template, SampleData));
	}
	
	[Theory]
	[InlineData(UndefinedStrategy.Ignore, "x{{missing}}y")]
	[InlineData(UndefinedStrategy.Remove, "xy")]
	[InlineData(UndefinedStrategy.Swap, "x?y")]
	public void Format_WithAnUndefinedValue_AppliesTheStrategy(UndefinedStrategy strategy, string expected)
	{
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", UndefinedStrategy = strategy, Fallback = "?" });
		
		Assert.Equal(expected, formatter.Format("x{{missing}}y", SampleData));
	}
	
	[Fact]
	public void Format_WithAnUndefinedValueAndThrowStrategy_Throws()
	{
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", UndefinedStrategy = UndefinedStrategy.Throw });
		
		var exception = Assert.Throws<ArgumentException>(() => formatter.Format("x{{missing}}y", SampleData));
		
		Assert.Equal("missing is undefined", exception.Message);
	}
	
	[Fact]
	public void Format_WithSwapStrategyWithoutFallback_RemovesThePlaceholder()
	{
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", UndefinedStrategy = UndefinedStrategy.Swap });
		
		Assert.Equal("xy", formatter.Format("x{{missing}}y", SampleData));
	}
	
	#endregion
	
	#region Array Index Methods
	
	[Theory]
	[InlineData("{{tags[-1]}}", "b")]
	[InlineData("{{tags[-2]}}", "a")]
	[InlineData("{{items[-1].n}}", "2")]
	public void Format_WithANegativeIndex_CountsFromTheEnd(string template, string expected)
	{
		Assert.Equal(expected, new Formatter().Format(template, SampleData));
	}
	
	[Theory]
	[InlineData("{{tags[2]}}")]
	[InlineData("{{tags[5]}}")]
	[InlineData("{{tags[-3]}}")]
	[InlineData("{{items[2].n}}")]
	public void Format_WithAnIndexOutOfTheRange_AppliesTheUndefinedStrategy(string template)
	{
		var formatter = new Formatter(new ParserOptions { OpenBrackets = "{{", CloseBrackets = "}}", UndefinedStrategy = UndefinedStrategy.Swap, Fallback = "?" });
		
		Assert.Equal(template, new Formatter().Format(template, SampleData));
		Assert.Equal("?", formatter.Format(template, SampleData));
	}
	
	#endregion
	
	#region LookUp Methods
	
	[Fact]
	public void LookUp_ReturnsTheSegments()
	{
		var segments = new Formatter().LookUp("Hi {{ name }}!");
		
		Assert.Equal([SegmentType.RawPart, SegmentType.PlaceHolder, SegmentType.RawPart], segments.Select(x => x.Type));
		var placeHolder = Assert.IsType<PlaceHolder>(segments[1]);
		Assert.Equal("name", placeHolder.Value);
		Assert.Equal(" name ", placeHolder.Inner);
		Assert.Equal("{{ name }}", placeHolder.Outer);
		Assert.Equal(3, placeHolder.StartIndex);
		Assert.Equal(10, placeHolder.Length);
		Assert.Equal("{{", placeHolder.OpenBrackets);
		Assert.Equal("}}", placeHolder.CloseBrackets);
	}
	
	#endregion
	
	#region Test Types
	
	private sealed class Poco
	{
		[JsonPropertyName("first_name")]
		public string? FirstName { get; set; }
		
		[JsonPropertyName("address")]
		public PocoAddress? Address { get; set; }
		
		public DayOfWeek Day { get; set; }
	}
	
	private sealed class ContractPoco
	{
		[JsonPropertyName("status")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DayOfWeek Status { get; set; }
		
		[JsonIgnore]
		public string? Secret { get; set; }
		
		// ReSharper disable once NotAccessedField.Local
		public string? Field;
		
		public int[]? Numbers { get; set; }
	}
	
	private sealed class PocoAddress
	{
		[JsonPropertyName("city")]
		public string? City { get; set; }
	}
	
	/// <summary>
	/// A convertible which supports no conversion unless overridden
	/// </summary>
	private abstract class BaseFakeConvertible : IConvertible
	{
		public abstract TypeCode GetTypeCode();
		
		public virtual object ToType(Type conversionType, IFormatProvider? provider) => throw new InvalidCastException();
		
		public virtual string ToString(IFormatProvider? provider) => throw new InvalidCastException();
		
		public virtual double ToDouble(IFormatProvider? provider) => throw new InvalidCastException();
		
		public bool ToBoolean(IFormatProvider? provider) => throw new InvalidCastException();
		
		public byte ToByte(IFormatProvider? provider) => throw new InvalidCastException();
		
		public char ToChar(IFormatProvider? provider) => throw new InvalidCastException();
		
		public DateTime ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();
		
		public decimal ToDecimal(IFormatProvider? provider) => throw new InvalidCastException();
		
		public short ToInt16(IFormatProvider? provider) => throw new InvalidCastException();
		
		public int ToInt32(IFormatProvider? provider) => throw new InvalidCastException();
		
		public long ToInt64(IFormatProvider? provider) => throw new InvalidCastException();
		
		public sbyte ToSByte(IFormatProvider? provider) => throw new InvalidCastException();
		
		public float ToSingle(IFormatProvider? provider) => throw new InvalidCastException();
		
		public ushort ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
		
		public uint ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
		
		public ulong ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();
	}
	
	/// <summary>
	/// A convertible without a base type which converts only to string, like MongoDB.Bson.ObjectId
	/// </summary>
	private sealed class FakeConvertibleId(string value) : BaseFakeConvertible
	{
		// ReSharper disable once UnusedMember.Local
		public int Timestamp => 1;
		
		public override TypeCode GetTypeCode() => TypeCode.Object;
		
		public override object ToType(Type conversionType, IFormatProvider? provider) => conversionType == typeof(string) ? value : throw new InvalidCastException();
	}
	
	/// <summary>
	/// A convertible with a Double base type, like MongoDB.Bson.BsonDouble
	/// </summary>
	private sealed class FakeConvertibleNumber(double value) : BaseFakeConvertible
	{
		public override TypeCode GetTypeCode() => TypeCode.Double;
		
		public override double ToDouble(IFormatProvider? provider) => value;
	}
	
	/// <summary>
	/// A convertible with a String base type, like MongoDB.Bson.BsonString
	/// </summary>
	private sealed class FakeConvertibleText(string value) : BaseFakeConvertible
	{
		public override TypeCode GetTypeCode() => TypeCode.String;
		
		public override string ToString(IFormatProvider? provider) => value;
	}
	
	/// <summary>
	/// A convertible collection which can not be converted to string, like MongoDB.Bson.BsonArray
	/// </summary>
	private sealed class FakeConvertibleCollection(params int[] items) : BaseFakeConvertible, IEnumerable<int>
	{
		public override TypeCode GetTypeCode() => TypeCode.Object;
		
		public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>) items).GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
	
	#endregion
}
