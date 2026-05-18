// ReSharper disable UnusedMember.Global
namespace Ertis.MongoDB.Models;

// ReSharper disable once UnusedType.Global
public class CollationOptions
{
	#region Properties
	
	public Locale? Locale { get; init; }
	
	public bool CaseInsensitive { get; init; }
	
	#endregion
}