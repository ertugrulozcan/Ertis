using System.Net.Http;
using System.Net.Http.Headers;

namespace Ertis.Net.Http
{
    public class XmlRequestBody : IRequestBody
    {
        #region Properties

        public object Payload { get; }
		
        public BodyTypes Type => BodyTypes.Xml;

        public string Xml
        {
            get
            {
                if (this.Payload == null)
                    return null;
				
                var serializer = new RestSharp.Serializers.DotNetXmlSerializer();
                return serializer.Serialize(this.Payload);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload"></param>
        public XmlRequestBody(object payload)
        {
            this.Payload = payload;
        }

        #endregion

        #region Methods

        public HttpContent GetHttpContent()
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(this.Xml);
            var content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            return content;
        }

        #endregion
    }
}