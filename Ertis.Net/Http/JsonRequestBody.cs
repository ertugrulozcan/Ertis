using System.Net.Http.Headers;

namespace Ertis.Net.Http
{
	public class JsonRequestBody : IRequestBody
	{
		#region Properties

		public object? Payload { get; }
		
		public BodyTypes Type => BodyTypes.Json;

		// ReSharper disable once MemberCanBePrivate.Global
		public string? Json => this.Payload == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(this.Payload);

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="payload"></param>
		public JsonRequestBody(object payload)
		{
			this.Payload = payload;
		}

		#endregion

		#region Methods

		public HttpContent GetHttpContent()
		{
			if (this.Json != null)
			{
				var buffer = System.Text.Encoding.UTF8.GetBytes(this.Json);
				var content = new ByteArrayContent(buffer);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				return content;	
			}
			else
			{
				return new StringContent(string.Empty);
			}
		}

		#endregion
	}
}