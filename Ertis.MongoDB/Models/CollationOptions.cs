// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.MongoDB.Models;

public class CollationOptions
{
	#region Properties
	
	public Locale? Locale { get; init; }
	
	public bool CaseInsensitive { get; init; }
	
	#endregion
}