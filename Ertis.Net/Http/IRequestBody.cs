using System.Net.Http;

namespace Ertis.Net.Http
{
	public interface IRequestBody
	{
		BodyTypes Type { get; }
		
		object Payload { get; }

		HttpContent GetHttpContent();
	}
}