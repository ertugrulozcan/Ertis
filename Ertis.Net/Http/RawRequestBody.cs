using System.Net.Http.Headers;

namespace Ertis.Net.Http;

public class RawRequestBody : IRequestBody
{
    #region Properties
    
    public object? Payload { get; }
    
    public BodyTypes Type { get; }
    
    private string ContentType { get; }
    
    private string? Encoding { get; }
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="bodyType"></param>
    /// <param name="contentType"></param>
    /// <param name="encoding"></param>
    public RawRequestBody(byte[] payload, BodyTypes bodyType, string contentType, string? encoding = null)
    {
        this.Payload = payload;
        this.Type = bodyType;
        this.ContentType = contentType;
        this.Encoding = encoding;
    }
    
    #endregion
    
    #region Methods
    
    public HttpContent GetHttpContent()
    {
        if (this.Payload is byte[] buffer)
        {
            var content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType)
            {
                CharSet = this.Encoding
            };
            
            return content;	
        }
        else
        {
            return new StringContent(string.Empty);
        }
    }
    
    #endregion
}