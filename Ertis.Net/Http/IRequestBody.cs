// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Net.Http;

public interface IRequestBody
{
	BodyTypes Type { get; }
	
	object? Payload { get; }
	
	HttpContent GetHttpContent();
}