using System.Net.Http.Headers;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.Net.Http;

public class TextRequestBody : IRequestBody
{
    #region Properties
    
    public BodyTypes Type => BodyTypes.Text;
    
    public string? Body { get; }
    
    public object? Payload => this.Body;
    
    public string? ContentType { get; init; }
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="body"></param>
    public TextRequestBody(string body)
    {
        this.Body = body;
    }
    
    #endregion
    
    #region Methods
    
    public HttpContent GetHttpContent()
    {
        if (!string.IsNullOrEmpty(this.Body))
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(this.Body);
            var content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(this.ContentType) ? "text/plain" : this.ContentType);
            return content;	
        }
        else
        {
            return new StringContent(string.Empty);
        }
    }
    
    #endregion
}