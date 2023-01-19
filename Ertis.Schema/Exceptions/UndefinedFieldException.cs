using System;

namespace Ertis.Schema.Exceptions;

public class UndefinedFieldException : Exception
{
	#region Constructors

	public UndefinedFieldException(string path) : base($"'{path}': undefined")
	{ }

	#endregion
}