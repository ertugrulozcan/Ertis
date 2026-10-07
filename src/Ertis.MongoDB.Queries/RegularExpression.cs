namespace Ertis.MongoDB.Queries;

/// <summary>
/// A regular expression value (extended json v2): unlike the legacy { "$regex": ..., "$options": ... } pair, MongoDB's json reader
/// reads it in every place ($elemMatch, next to the other operators of a field)
/// </summary>
internal sealed class RegularExpression : QueryValue<string>
{
	#region Properties
	
	private string Pattern { get; }
	
	private string Options { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="pattern">Pattern</param>
	/// <param name="options">Options</param>
	public RegularExpression(string pattern, RegexOptions? options) : base(pattern)
	{
		this.Pattern = pattern;
		// The options are in alphabetical order in extended json
		this.Options = string.Concat((QueryHelper.ConvertRegexOptions(options) ?? string.Empty).Order());
	}
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		return "{ \"$regularExpression\": { \"pattern\": " + QueryHelper.ToJsonString(this.Pattern) + ", \"options\": " + QueryHelper.ToJsonString(this.Options) + " } }";
	}
	
	#endregion
}
