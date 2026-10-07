using System.Net;
using System.Text.Json;
using Ertis.Core.Models;

namespace Ertis.Core.Tests.Models;

public class ModelTests
{
	#region Response Result Methods
	
	[Theory]
	[InlineData(HttpStatusCode.OK, true)]
	[InlineData(HttpStatusCode.NoContent, true)]
	[InlineData(HttpStatusCode.MultipleChoices, false)]
	[InlineData(HttpStatusCode.BadRequest, false)]
	public void ResponseResult_WithAStatusCode_IsSuccessForTheSuccessCodes(HttpStatusCode statusCode, bool expected)
	{
		Assert.Equal(expected, new ResponseResult(statusCode).IsSuccess);
		Assert.Equal(expected, new ResponseResult<int>(statusCode, "m").IsSuccess);
	}
	
	[Fact]
	public void ResponseResult_WithASuccessFlag_SetsTheStatusCode()
	{
		var success = new ResponseResult(true);
		var failure = new ResponseResult(false, "failed");
		
		Assert.True(success.IsSuccess);
		Assert.Equal(HttpStatusCode.OK, success.StatusCode);
		Assert.False(failure.IsSuccess);
		Assert.Null(failure.StatusCode);
		Assert.Equal("failed", failure.Message);
	}
	
	[Fact]
	public void ResponseResult_ToString_DescribesTheResult()
	{
		Assert.Equal("m", new ResponseResult(true, "m").ToString());
		Assert.Equal("e", new ResponseResult(false) { Exception = new Exception("e") }.ToString());
		Assert.Equal("{}", new ResponseResult(false) { Json = "{}" }.ToString());
		Assert.Equal("Success", new ResponseResult(true).ToString());
		Assert.Equal("Failure", new ResponseResult(HttpStatusCode.NotFound).ToString());
	}
	
	[Fact]
	public void ResponseResult_IsSerializedWithoutTheNullValues()
	{
		var json = JsonSerializer.Serialize(new ResponseResult<string>(HttpStatusCode.Created) { Data = "d", Headers = new Dictionary<string, string> { ["h"] = "v" } });
		
		Assert.Equal("""{"isSuccess":true,"statusCode":201,"headers":{"h":"v"},"data":"d"}""", json);
	}
	
	#endregion
	
	#region Other Model Methods
	
	[Fact]
	public void ErrorModel_IsSerializedWithTheJsonNames()
	{
		var json = JsonSerializer.Serialize(new ErrorModel<string> { Message = "m", ErrorCode = "c", StatusCode = 400, Data = "d" });

		Assert.Equal("""{"data":"d","message":"m","errorCode":"c","statusCode":400}""", json);
	}

	[Fact]
	public void ErrorModel_WithNullData_IsSerializedWithoutTheData()
	{
		var json = JsonSerializer.Serialize(new ErrorModel<string> { Message = "m", ErrorCode = "c", StatusCode = 400 });

		Assert.Equal("""{"message":"m","errorCode":"c","statusCode":400}""", json);
	}

	[Theory]
	[InlineData(0, """{"data":0,"message":"m","errorCode":"c","statusCode":400}""")]
	[InlineData(null, """{"message":"m","errorCode":"c","statusCode":400}""")]
	public void ErrorModel_WithNullableValueTypeData_WritesZeroAndSkipsNull(int? data, string expected)
	{
		var json = JsonSerializer.Serialize(new ErrorModel<int?> { Message = "m", ErrorCode = "c", StatusCode = 400, Data = data });

		Assert.Equal(expected, json);
	}

	[Fact]
	public void SysModel_IsSerializedWithoutTheNullValues()
	{
		var date = new DateTime(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
		
		Assert.Equal("""{"created_at":"2026-01-31T10:00:00Z","created_by":"a"}""", JsonSerializer.Serialize(new SysModel { CreatedAt = date, CreatedBy = "a" }));
		Assert.Equal("""{"modified_at":"2026-01-31T10:00:00Z","modified_by":"b"}""", JsonSerializer.Serialize(new SysModel { ModifiedAt = date, ModifiedBy = "b" }));
	}
	
	#endregion
}
