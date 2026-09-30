using System.Globalization;

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
				return QueryHelper.ToJsonString(QueryHelper.ToUniversalTime(dateTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
			case DateTimeOffset dateTimeOffset:
				return QueryHelper.ToJsonString(dateTimeOffset.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
			case Guid guid:
				return QueryHelper.ToJsonString(guid.ToString());
			case Enum enumValue:
				return QueryHelper.ToJsonString(enumValue.ToString());
			case byte or sbyte or short or ushort or int or uint or long or ulong or nint or nuint or float or double or decimal:
				return ((IFormattable) this.Value).ToString(null, CultureInfo.InvariantCulture);
			default:
				return this.Value.ToString() ?? "null";
		}
	}
	
	#endregion
}