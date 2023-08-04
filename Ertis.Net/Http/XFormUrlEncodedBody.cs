namespace Ertis.Net.Http;

public class XFormUrlEncodedBody : IRequestBody
{
	#region Properties

	private IDictionary<string, string> Dictionary { get; }

	public object Payload => this.Dictionary;
	
	public BodyTypes Type => BodyTypes.UrlEncoded;

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="dictionary"></param>
	public XFormUrlEncodedBody(IDictionary<string, string> dictionary)
	{
		this.Dictionary = dictionary;
	}

	#endregion

	#region Methods

	public HttpContent GetHttpContent()
	{
		return new FormUrlEncodedContent(this.Dictionary.ToArray());
	}

	#endregion
}