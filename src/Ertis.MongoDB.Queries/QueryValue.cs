using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace Ertis.MongoDB.Queries;

public class QueryValue<T> : IQuery
{
	#region Properties
	
	// ReSharper disable once MemberCanBePrivate.Global
	protected T Value { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value"></param>
	public QueryValue(T value)
	{
		this.Value = value;
	}
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		switch (this.Value)
		{
			case null:
				return "null";
			case IQuery query:
				return query.ToString();
			case string text:
				return QueryHelper.ToJsonString(text);
			case char character:
				return QueryHelper.ToJsonString(character.ToString());
			case bool boolean:
				return boolean ? "true" : "false";
			case DateTime dateTime:
				return QueryHelper.ToExtendedJsonDate(QueryHelper.ToUniversalTime(dateTime));
			case DateTimeOffset dateTimeOffset:
				return QueryHelper.ToExtendedJsonDate(dateTimeOffset.UtcDateTime);
			case DateOnly date:
				// A date is stored as the UTC midnight (the date fields of Ertis.Schema, the DateOnly serializer of the MongoDB driver)
				return QueryHelper.ToExtendedJsonDate(date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
			case TimeOnly time:
				// MongoDB has no time type: written like System.Text.Json writes it
				return QueryHelper.ToJsonString(time.ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture));
			case TimeSpan timeSpan:
				return QueryHelper.ToJsonString(timeSpan.ToString("c", CultureInfo.InvariantCulture));
			case double doubleValue when !double.IsFinite(doubleValue):
				return QueryHelper.ToExtendedJsonDouble(doubleValue);
			case float floatValue when !float.IsFinite(floatValue):
				return QueryHelper.ToExtendedJsonDouble(floatValue);
			case Half halfValue when !Half.IsFinite(halfValue):
				return QueryHelper.ToExtendedJsonDouble((double) halfValue);
			case Guid guid:
				return QueryHelper.ToJsonString(guid.ToString());
			case Uri uri:
				return QueryHelper.ToJsonString(uri.OriginalString);
			case Enum enumValue:
				return QueryHelper.ToJsonString(enumValue.ToString());
			case byte or sbyte or short or ushort or int or uint or long or ulong or nint or nuint or float or double or decimal or Half or Int128 or UInt128 or BigInteger:
				return ((IFormattable) this.Value).ToString(null, CultureInfo.InvariantCulture);
			case IConvertible convertible when convertible.GetTypeCode() == TypeCode.Object:
				// A convertible without a base type (e.g. MongoDB ObjectId) is written as its invariant string, like Ertis.Schema does
				return QueryHelper.ToJsonString(convertible.ToString(CultureInfo.InvariantCulture));
			case IEnumerable enumerable and not IDictionary:
				// Each item by these rules (e.g. the dates of an array are dates)
				return "[ " + string.Join(", ", enumerable.Cast<object?>().Select(x => new QueryValue<object?>(x).ToString())) + " ]";
			default:
				// Any other value (dictionaries, objects) is serialized as json: never written as it is, so it can't change the structure of the query
				return JsonSerializer.Serialize(this.Value, this.Value.GetType());
		}
	}
	
	#endregion
}