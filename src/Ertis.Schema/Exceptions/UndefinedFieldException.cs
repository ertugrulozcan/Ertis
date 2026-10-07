namespace Ertis.Schema.Exceptions;

public class UndefinedFieldException(string path) : Exception($"'{path}': undefined");