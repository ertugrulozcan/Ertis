using System.Globalization;

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
	
	public static string EnsureDatetimeFieldsToISODate(string json)
	{
		return QueryHelper.EnsureQuery(json, convertObjectIds: false, convertDates: true);
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