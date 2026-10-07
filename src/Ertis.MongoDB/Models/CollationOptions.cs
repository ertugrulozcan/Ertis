using Ertis.MongoDB.Helpers;
using MongoDB.Driver;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.MongoDB.Models;

public class CollationOptions
{
	#region Properties
	
	public Locale? Locale { get; init; }
	
	public bool CaseInsensitive { get; init; }
	
	#endregion
	
	#region Methods
	
	internal Collation? GetCollation()
	{
		if (this.Locale == null)
		{
			return null;
		}
		
		return new Collation(
			LocaleHelper.GetLanguageCode(this.Locale.Value),
			strength: this.CaseInsensitive ? CollationStrength.Primary : null
		);
	}
	
	#endregion
}
