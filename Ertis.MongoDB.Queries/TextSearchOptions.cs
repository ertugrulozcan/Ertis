// ReSharper disable UnusedMember.Global
namespace Ertis.MongoDB.Queries;

// ReSharper disable once UnusedType.Global
public class TextSearchOptions
{
	#region Properties
	
	public TextSearchLanguage Language { get; set; }
	
	public bool IsCaseSensitive { get; set; }
	
	public bool IsDiacriticSensitive { get; set; }
	
	#endregion
}