namespace Ertis.Schema.Exceptions;

public abstract class ErtisSchemaValidationException : Exception
{
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message"></param>
	/// <param name="innerException"></param>
	protected ErtisSchemaValidationException(string message, Exception? innerException = null) : base(message, innerException)
	{ }
	
	#endregion
}