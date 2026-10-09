namespace Ertis.Schema.Exceptions;

public class SchemaValidationException : ErtisSchemaValidationException
{
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	public SchemaValidationException(string message, Exception? innerException = null) : base(message, innerException)
	{ }
	
	#endregion
}