using System.Globalization;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.MongoDB.Helpers;

public static class ISODateHelper
{
	#region Fields
	
	private static readonly string[] DateFormats =
	[
		"yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK",
		"yyyy-MM-dd'T'HH:mmK",
		"yyyy-MM-dd"
	];
	
	#endregion
	
	#region Methods
	
	public static string? EnsureDatetimeFieldsToISODate(string json)
	{
		return QueryHelper.Ensure(json, convertObjectIds: false, convertDates: true);
	}
	
	public static JToken? EnsureDatetimeFieldsToISODate(JToken? node)
	{
		if (node == null)
		{
			return null;
		}
		
		try
		{
			if (node is JValue jValue)
			{
				if (node.Type is JTokenType.String or JTokenType.Date)
				{
					var nodeValue = node.Value<string>();
					if (nodeValue != null && TryParseDateTime(nodeValue, out var dateTime))
					{
						jValue.Replace(new JRaw($"ISODate(\"{dateTime:yyyy-MM-ddTHH:mm:ssZ}\")"));
					}
				}	
			}
			
			foreach (var child in node)
			{
				EnsureDatetimeFieldsToISODate(child);
			}
			
			return node;
		}
		catch
		{
			return node;
		}
	}
	
	/// <summary>
	/// Parses an ISO 8601 date (with or without the time); the result is in UTC, a value without an offset is taken as UTC
	/// </summary>
	public static bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
	{
		return DateTime.TryParseExact(dateTimeString, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime);
	}
	
	#endregion
}