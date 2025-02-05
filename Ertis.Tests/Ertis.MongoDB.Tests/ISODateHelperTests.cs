using System;
using Ertis.MongoDB.Helpers;
using Ertis.MongoDB.Queries;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Tests;

public class ISODateHelperTests
{
	#region Methods

	[Test]
	public void TryParseDateTime_Test()
	{
		Assert.That(ISODateHelper.TryParseDateTime("2022-12-09T16:34:43Z", out var datetime));
		Assert.That(2022 == datetime.Year);
		Assert.That(12 == datetime.Month);
		Assert.That(09 == datetime.Day);
		Assert.That(19 == datetime.Hour);
		Assert.That(34 == datetime.Minute);
		Assert.That(43 == datetime.Second);
	}
	
	[Test]
	public void TryParseDateTime_UTCShift_Test()
	{
		Assert.That(ISODateHelper.TryParseDateTime("2022-12-09T21:34:43Z", out var datetime));
		Assert.That(2022 == datetime.Year);
		Assert.That(12 == datetime.Month);
		Assert.That(10 == datetime.Day);
		Assert.That(00 == datetime.Hour);
		Assert.That(34 == datetime.Minute);
		Assert.That(43 == datetime.Second);
	}
	
	[Test]
	public void TryParseDateTime_StringInterpolation_Test()
	{
		Assert.That(ISODateHelper.TryParseDateTime("2022-12-09T21:34:43Z", out var datetime));
		Assert.That("2022-12-10T00:34:43Z" == $"{datetime:yyyy-MM-ddTHH:mm:ssZ}");
	}

	[Test]
	public void Query_Ensure_Test_With_String_Query()
	{
		var query1 = "{ \"sys.published_at\": { \"$gt\": \"2022-12-09T21:00:00.528Z\" }, \"organization_id\": \"6356f3240e37638afd92c516\" }";
		var query2 = QueryHelper.EnsureObjectIdsAndISODates(query1);
		Assert.That(
			"{ \"sys.published_at\": { \"$gt\": ISODate(\"2022-12-09T21:00:00Z\") }, \"organization_id\": \"6356f3240e37638afd92c516\" }"
				.Replace("\n", string.Empty)
				.Replace(" ", string.Empty)
				.Trim() == 
			query2
				.Replace("\n", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
			);
	}
	
	[Test]
	public void Query_Ensure_Test_With_String_Builder()
	{
		IQuery[] expressions = 
		{
			QueryBuilder.GreaterThan("sys.published_at", new DateTime(2022, 12, 09, 21, 00, 00)), 
			QueryBuilder.Equals("organization_id", "6356f3240e37638afd92c516")
		};
                
		var query = QueryBuilder.And(expressions);
		var query2 = QueryHelper.EnsureObjectIdsAndISODates(query.ToString());
		
		Assert.That(
			"{ \"sys.published_at\": { \"$gt\": ISODate(\"2022-12-09T21:00:00Z\") }, \"organization_id\": \"6356f3240e37638afd92c516\" }"
				.Replace("\n", string.Empty)
				.Replace(" ", string.Empty)
				.Trim() == 
			query2
				.Replace("\n", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
		);
	}
	
	#endregion
}