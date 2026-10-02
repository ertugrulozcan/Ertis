using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ertis.Schema.Tests.TestHelpers;
using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Tests.Dynamics;

/// <summary>
/// The mapping of json and CLR values into the value model of DynamicObject
/// </summary>
public class DynamicValueMappingTests
{
	#region Json Methods
	
	[Theory]
	[InlineData("5", typeof(long))]
	[InlineData("-9223372036854775808", typeof(long))]
	[InlineData("9223372036854775808", typeof(decimal))]
	[InlineData("1e400", typeof(double))]
	[InlineData("0.5", typeof(double))]
	[InlineData("1.0", typeof(double))]
	[InlineData("2e3", typeof(double))]
	public void Parse_MapsTheNumbers(string jsonNumber, Type expectedType)
	{
		var dynamicObject = DynamicObject.Parse($$"""{ "value": {{jsonNumber}} }""");
		
		Assert.IsType(expectedType, dynamicObject.GetValue("value"));
	}
	
	[Theory]
	[InlineData("2026-01-31T10:00:00Z")]
	[InlineData("2026-01-31T10:00:00+03:00")]
	[InlineData("2026-01-31")]
	public void Parse_KeepsDateLikeStrings(string value)
	{
		var dynamicObject = DynamicObject.Parse($$"""{ "value": "{{value}}" }""");
		
		Assert.Equal(value, dynamicObject.GetValue("value"));
	}
	
	[Fact]
	public void Parse_AllowsCommentsAndTrailingCommas()
	{
		var dynamicObject = DynamicObject.Parse("""{ "a": 1, /* comment */ "b": [1, 2,], }""");
		
		Assert.Equal(1L, dynamicObject.GetValue("a"));
		Assert.Equal(2, ((object[]) dynamicObject.GetValue("b")!).Length);
	}
	
	[Fact]
	public void Parse_WithDuplicateKeys_KeepsTheLastValue()
	{
		var dynamicObject = DynamicObject.Parse("""{ "a": 1, "a": 2 }""");
		
		Assert.Equal(2L, dynamicObject.GetValue("a"));
	}
	
	[Theory]
	[InlineData("[1, 2]")]
	[InlineData("5")]
	public void Parse_NonObjectJson_ReturnsAnEmptyObject(string json)
	{
		var dynamicObject = DynamicObject.Parse(json);
		
		Assert.Empty(dynamicObject.ToDictionary());
	}
	
	[Fact]
	public void Parse_InvalidJson_ThrowsJsonException()
	{
		Assert.ThrowsAny<JsonException>(() => DynamicObject.Parse("""{ 'a': 1 }"""));
	}
	
	#endregion
	
	#region CLR Object Methods
	
	[Fact]
	public void Constructor_WithPoco_UsesTheSystemTextJsonContract()
	{
		var poco = new Poco
		{
			Id = new FakeObjectId("65a0f0c2e4b0a1b2c3d4e5f6"),
			Name = "Jane",
			Secret = "hidden",
			Nickname = null,
			Status = PocoStatus.Active,
			Level = PocoStatus.Passive,
			Field = 7
		};
		
		var dictionary = new DynamicObject(poco).ToDictionary();
		
		Assert.Equal("65a0f0c2e4b0a1b2c3d4e5f6", dictionary["_id"]);
		Assert.Equal("Jane", dictionary["name"]);
		Assert.False(dictionary.ContainsKey("Secret"));
		Assert.False(dictionary.ContainsKey("nickname"));
		Assert.Equal("Active", dictionary["status"]);
		Assert.Equal(1, dictionary["level"]);
		Assert.Equal(7, dictionary["Field"]);
	}
	
	[Fact]
	public void Constructor_KeepsTheClrTypesOfTheValues()
	{
		var createdAt = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
		var guid = Guid.NewGuid();
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["_id"] = new FakeObjectId("65a0f0c2e4b0a1b2c3d4e5f6");
		expando["count"] = 5;
		expando["ratio"] = 0.5m;
		expando["createdAt"] = createdAt;
		expando["startsAt"] = new DateTimeOffset(createdAt);
		expando["guid"] = guid;
		expando["bytes"] = new byte[] { 1, 2 };
		expando["letter"] = 'x';
		expando["sys"] = new Dictionary<string, object?> { ["modifiedAt"] = createdAt };
		expando["scores"] = new List<int> { 1, 2 };
		expando["labels"] = new Dictionary<string, int> { ["a"] = 1 };
		expando["json"] = JsonSerializer.SerializeToElement(new { a = 1 });
		
		var dynamicObject = new DynamicObject(expando);
		
		Assert.Equal("65a0f0c2e4b0a1b2c3d4e5f6", dynamicObject.GetValue("_id"));
		Assert.Equal(5, dynamicObject.GetValue("count"));
		Assert.Equal(0.5m, dynamicObject.GetValue("ratio"));
		Assert.Equal(createdAt, dynamicObject.GetValue("createdAt"));
		Assert.Equal(DateTimeKind.Utc, ((DateTime) dynamicObject.GetValue("createdAt")!).Kind);
		Assert.IsType<DateTimeOffset>(dynamicObject.GetValue("startsAt"));
		Assert.Equal(guid.ToString(), dynamicObject.GetValue("guid"));
		Assert.Equal("AQI=", dynamicObject.GetValue("bytes"));
		Assert.Equal("x", dynamicObject.GetValue("letter"));
		Assert.Equal(createdAt, dynamicObject.GetValue("sys.modifiedAt"));
		Assert.Equal(new object[] { 1, 2 }, dynamicObject.GetValue("scores"));
		Assert.Equal(1, dynamicObject.GetValue("labels.a"));
		Assert.Equal(1L, dynamicObject.GetValue("json.a"));
	}
	
	[Fact]
	public void Constructor_WritesTheConvertiblesWithoutBaseTypeAsStrings()
	{
		const string id = "65a0f0c2e4b0a1b2c3d4e5f6";
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["_id"] = new FakeConvertibleId(id);
		expando["owner"] = new Dictionary<string, object?> { ["_id"] = new FakeConvertibleId(id) };
		expando["refs"] = new List<FakeConvertibleId> { new(id) };
		
		var dynamicObject = new DynamicObject(expando);
		
		Assert.Equal(id, dynamicObject.GetValue("_id"));
		Assert.Equal(id, dynamicObject.GetValue("owner._id"));
		Assert.Equal(new object[] { id }, dynamicObject.GetValue("refs"));
	}
	
	[Fact]
	public void Constructor_ConvertsTheConvertiblesIntoTheBaseTypesOfTheirTypeCodes()
	{
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["number"] = new FakeConvertibleNumber(42);
		expando["text"] = new FakeConvertibleText("text");
		expando["dbNull"] = DBNull.Value;
		
		var dynamicObject = new DynamicObject(expando);
		
		Assert.Equal(42, dynamicObject.GetValue("number"));
		Assert.Equal("text", dynamicObject.GetValue("text"));
		Assert.Null(dynamicObject.GetValue("dbNull"));
	}
	
	[Fact]
	public void Constructor_KeepsTheConvertibleCollectionsAsCollections()
	{
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["items"] = new FakeConvertibleCollection(1, 2);
		
		var dynamicObject = new DynamicObject(expando);
		
		Assert.Equal(new object[] { 1, 2 }, dynamicObject.GetValue("items"));
	}
	
	[Fact]
	public void Deserialize_WithConvertibleId_ReadsTheIdAsString()
	{
		const string id = "65a0f0c2e4b0a1b2c3d4e5f6";
		IDictionary<string, object?> expando = new ExpandoObject();
		expando["_id"] = new FakeConvertibleId(id);
		expando["name"] = "Jane";
		
		var model = new DynamicObject(expando).Deserialize<StringIdModel>();
		
		Assert.NotNull(model);
		Assert.Equal(id, model.Id);
		Assert.Equal("Jane", model.Name);
	}
	
	[Theory]
	[InlineData("text")]
	[InlineData(5)]
	public void Constructor_WithNonObjectValue_Throws(object value)
	{
		Assert.Throws<ArgumentException>(() => new DynamicObject(value));
	}
	
	#endregion
	
	#region Copy & Conversion Methods
	
	[Fact]
	public void Clone_KeepsTheClrTypes()
	{
		var createdAt = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
		var original = DynamicObject.Create(new Dictionary<string, object?> { ["createdAt"] = createdAt, ["tags"] = new object[] { "a" } });
		
		var clone = (DynamicObject) original.Clone();
		clone.SetValue("tags[0]", "b");
		
		Assert.Equal(createdAt, clone.GetValue("createdAt"));
		Assert.Equal("a", original.GetValue("tags[0]"));
	}
	
	[Fact]
	public void GetValueOfT_ConvertsCultureInvariant()
	{
		using var _ = new CultureScope("tr-TR");
		var dynamicObject = DynamicObject.Create(new Dictionary<string, object?>
		{
			["ratio"] = 0.5,
			["createdAt"] = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc),
			["startsAt"] = "2026-01-31T10:00:00+03:00",
			["guid"] = "0f8fad5b-d9cb-469f-a165-70867728950e",
			["count"] = "12"
		});
		
		Assert.Equal("0.5", dynamicObject.GetValue<string>("ratio"));
		Assert.Equal("2026-01-31T10:00:00.0000000Z", dynamicObject.GetValue<string>("createdAt"));
		Assert.Equal(new DateTimeOffset(2026, 1, 31, 7, 0, 0, TimeSpan.Zero), dynamicObject.GetValue<DateTimeOffset>("startsAt"));
		Assert.Equal(new DateTime(2026, 1, 31, 7, 0, 0, DateTimeKind.Utc), dynamicObject.GetValue<DateTime>("startsAt").ToUniversalTime());
		Assert.Equal(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), dynamicObject.GetValue<Guid>("guid"));
		Assert.Equal(12, dynamicObject.GetValue<int?>("count"));
	}
	
	#endregion
	
	#region Test Types
	
	public enum PocoStatus
	{
		Passive = 1,
		Active = 2
	}
	
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public sealed class Poco
	{
		[JsonPropertyName("_id")]
		public required FakeObjectId Id { get; init; }
		
		[JsonPropertyName("name")]
		public required string Name { get; init; }
		
		[JsonIgnore]
		public string? Secret { get; init; }
		
		[JsonPropertyName("nickname")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Nickname { get; init; }
		
		[JsonPropertyName("status")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public PocoStatus Status { get; init; }
		
		[JsonPropertyName("level")]
		public PocoStatus Level { get; init; }
		
		// ReSharper disable once NotAccessedField.Global
		public int Field;
	}
	
	/// <summary>
	/// A type with a string TypeConverter
	/// </summary>
	[TypeConverter(typeof(FakeObjectIdConverter))]
	public readonly struct FakeObjectId(string value)
	{
		public string Value { get; } = value;
		
		// ReSharper disable once UnusedMember.Global
		public int Timestamp => 1;
		
		public override string ToString() => this.Value;
	}
	
	public sealed class FakeObjectIdConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		
		public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
		{
			return destinationType == typeof(string) && value is FakeObjectId id ? id.Value : base.ConvertTo(context, culture, value, destinationType);
		}
	}
	
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public sealed class StringIdModel
	{
		[JsonPropertyName("_id")]
		public string Id { get; init; } = null!;
		
		[JsonPropertyName("name")]
		public string? Name { get; init; }
	}
	
	/// <summary>
	/// A convertible which supports no conversion unless overridden
	/// </summary>
	public abstract class BaseFakeConvertible : IConvertible
	{
		public abstract TypeCode GetTypeCode();
		
		public virtual object ToType(Type conversionType, IFormatProvider? provider) => throw new InvalidCastException();
		
		public virtual string ToString(IFormatProvider? provider) => throw new InvalidCastException();
		
		public virtual int ToInt32(IFormatProvider? provider) => throw new InvalidCastException();
		
		public bool ToBoolean(IFormatProvider? provider) => throw new InvalidCastException();
		
		public byte ToByte(IFormatProvider? provider) => throw new InvalidCastException();
		
		public char ToChar(IFormatProvider? provider) => throw new InvalidCastException();
		
		public DateTime ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();
		
		public decimal ToDecimal(IFormatProvider? provider) => throw new InvalidCastException();
		
		public double ToDouble(IFormatProvider? provider) => throw new InvalidCastException();
		
		public short ToInt16(IFormatProvider? provider) => throw new InvalidCastException();
		
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
	public sealed class FakeConvertibleId(string value) : BaseFakeConvertible
	{
		// ReSharper disable once UnusedMember.Global
		public int Timestamp => 1;
		
		public override TypeCode GetTypeCode() => TypeCode.Object;
		
		public override object ToType(Type conversionType, IFormatProvider? provider) => conversionType == typeof(string) ? value : throw new InvalidCastException();
	}
	
	/// <summary>
	/// A convertible with an Int32 base type, like MongoDB.Bson.BsonInt32
	/// </summary>
	public sealed class FakeConvertibleNumber(int value) : BaseFakeConvertible
	{
		public override TypeCode GetTypeCode() => TypeCode.Int32;
		
		public override int ToInt32(IFormatProvider? provider) => value;
	}
	
	/// <summary>
	/// A convertible with a String base type, like MongoDB.Bson.BsonString
	/// </summary>
	public sealed class FakeConvertibleText(string value) : BaseFakeConvertible
	{
		public override TypeCode GetTypeCode() => TypeCode.String;
		
		public override string ToString(IFormatProvider? provider) => value;
	}
	
	/// <summary>
	/// A convertible collection which can not be converted to string, like MongoDB.Bson.BsonArray
	/// </summary>
	public sealed class FakeConvertibleCollection(params int[] items) : BaseFakeConvertible, IEnumerable<int>
	{
		public override TypeCode GetTypeCode() => TypeCode.Object;
		
		public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>) items).GetEnumerator();
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
	
	#endregion
}
