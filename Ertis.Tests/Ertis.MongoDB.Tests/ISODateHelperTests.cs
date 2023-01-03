using Ertis.MongoDB.Helpers;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Tests;

public class ISODateHelperTests
{
	#region Methods

	[Test]
	public void TryParseDateTime_Test()
	{
		Assert.IsTrue(ISODateHelper.TryParseDateTime("2022-12-09T16:34:43Z", out var datetime));
		Assert.AreEqual(2022, datetime.Year);
		Assert.AreEqual(12, datetime.Month);
		Assert.AreEqual(09, datetime.Day);
		Assert.AreEqual(19, datetime.Hour);
		Assert.AreEqual(34, datetime.Minute);
		Assert.AreEqual(43, datetime.Second);
	}
	
	[Test]
	public void TryParseDateTime_UTCShift_Test()
	{
		Assert.IsTrue(ISODateHelper.TryParseDateTime("2022-12-09T21:34:43Z", out var datetime));
		Assert.AreEqual(2022, datetime.Year);
		Assert.AreEqual(12, datetime.Month);
		Assert.AreEqual(10, datetime.Day);
		Assert.AreEqual(00, datetime.Hour);
		Assert.AreEqual(34, datetime.Minute);
		Assert.AreEqual(43, datetime.Second);
	}
	
	[Test]
	public void TryParseDateTime_StringInterpolation_Test()
	{
		Assert.IsTrue(ISODateHelper.TryParseDateTime("2022-12-09T21:34:43Z", out var datetime));
		Assert.AreEqual("2022-12-10T00:34:43Z", $"{datetime:yyyy-MM-ddTHH:mm:ssZ}");
	}

	[Test]
	public void Query_Ensure_Test()
	{
		var query1 = "{ \"sys.published_at\": { \"$gt\": \"2022-12-09T21:00:00.528Z\" }, \"organization_id\": \"6356f3240e37638afd92c516\" }";
		var query2 = QueryHelper.EnsureObjectIdsAndISODates(query1);
		Assert.AreEqual(
			"{ \"sys.published_at\": { \"$gt\": ISODate(\"2022-12-09T21:00:00Z\") }, \"organization_id\": \"6356f3240e37638afd92c516\" }"
				.Replace("\n", string.Empty)
				.Replace(" ", string.Empty)
				.Trim(), 
			query2
				.Replace("\n", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
			);
	}

	#endregion
}