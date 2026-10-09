using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.Core.Tests.Exceptions;

public class ExceptionTests
{
	#region Test Types
	
	private sealed class TestException : ErtisException<int>
	{
		public TestException() : base(HttpStatusCode.Conflict, "Code")
		{ }
		
		public TestException(string message) : base(HttpStatusCode.Conflict, message, "Code")
		{ }
		
		public TestException(string message, Exception innerException) : base(HttpStatusCode.Conflict, message, "Code", innerException)
		{ }
	}
	
	#endregion
	
	#region Methods
	
	[Fact]
	public void ErtisException_HasItsErrorModel()
	{
		var inner = new InvalidOperationException();
		var exception = new TestException("m", inner) { Payload = 5 };
		
		Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
		Assert.Equal("Code", exception.ErrorCode);
		Assert.Equal(5, exception.Payload);
		Assert.Same(inner, exception.InnerException);
		Assert.Equal("m", exception.Error.Message);
		Assert.Equal("Code", exception.Error.ErrorCode);
		Assert.Equal(409, exception.Error.StatusCode);
		Assert.Equal("m", new TestException("m").Message);
		Assert.Equal(HttpStatusCode.Conflict, new TestException().StatusCode);
	}
	
	[Fact]
	public void ValidationException_HasItsErrors()
	{
		var exception = new ValidationException(HttpStatusCode.BadRequest, "invalid", "ValidationError") { Errors = ["a", "b"] };
		
		Assert.Equal(["a", "b"], exception.Errors);
		Assert.Equal(400, exception.Error.StatusCode);
		Assert.Equal("ValidationError", new ValidationException(HttpStatusCode.BadRequest, "ValidationError").ErrorCode);
		Assert.NotNull(new ValidationException(HttpStatusCode.BadRequest, "m", "c", new Exception()).InnerException);
	}
	
	#endregion
}
