namespace Ertis.Schema.Exceptions;

// ReSharper disable once UnusedType.Global
public class SchemaValidationException : ErtisSchemaValidationException
{
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	public SchemaValidationException(string message) : base(message)
	{
		
	}
	
	#endregion
}