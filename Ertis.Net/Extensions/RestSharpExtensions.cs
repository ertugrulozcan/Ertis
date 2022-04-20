using System.Net.Http;
using Ertis.Net.Http;
using RestSharp;

namespace Ertis.Net.Extensions
{
	public static class RestSharpExtensions
	{
		#region Methods

		public static Method ConvertToRestSharpMethod(this HttpMethod httpMethod)
		{
			if (httpMethod != null)
			{
				if (httpMethod == HttpMethod.Get)
					return Method.Get;
				else if (httpMethod == HttpMethod.Post)
					return Method.Post;
				else if (httpMethod == HttpMethod.Put)
					return Method.Put;
				else if (httpMethod == HttpMethod.Delete)
					return Method.Delete;
				else if (httpMethod == HttpMethod.Head)
					return Method.Head;
				else if (httpMethod == HttpMethod.Options)
					return Method.Options;
				else if (httpMethod == HttpMethod.Patch)
					return Method.Patch;
				else
					return Method.Get;
			}
			else
			{
				return Method.Get;
			}
		}
		
		public static DataFormat ConvertToRestSharpDataFormat(this IRequestBody body)
		{
			if (body == null)
			{
				return DataFormat.None;
			}

			switch (body.Type)
			{
				case BodyTypes.Javascript:
				case BodyTypes.Json:
				case BodyTypes.MongoQuery:
					return DataFormat.Json;
				case BodyTypes.Xml:
					return DataFormat.Xml;
				case BodyTypes.None:
				case BodyTypes.FormData:
				case BodyTypes.UrlEncoded:
				case BodyTypes.Text:
				case BodyTypes.Binary:
				case BodyTypes.Html:
				case BodyTypes.GraphQL:
					return DataFormat.None;
				default:
					return DataFormat.None;
			}
		}

		#endregion
	}
}