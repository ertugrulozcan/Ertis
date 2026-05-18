namespace Ertis.MongoDB.Models;

public class TextIndexDefinition : IndexDefinitionBase
{
	#region Fields
	
	private readonly string[]? fields;
	private readonly Dictionary<string, int>? weightedFields;
	
	#endregion
	
	#region Properties
	
	// ReSharper disable once MemberCanBePrivate.Global
	public string[] Fields
	{
		get
		{
			if (this.fields == null && this.weightedFields != null)
			{
				return this.weightedFields.Keys.ToArray();
			}
			
			return this.fields ?? Array.Empty<string>();
		}
		private init => this.fields = value;
	}
	
	// ReSharper disable once MemberCanBePrivate.Global
	public Dictionary<string, int>? WeightedFields
	{
		// ReSharper disable once UnusedMember.Global
		get
		{
			if (this.weightedFields == null && this.fields != null)
			{
				return this.fields.ToDictionary(x => x, _ => 1);
			}
			
			return this.weightedFields;
		}
		private init => this.weightedFields = value;
	}
	
	// ReSharper disable once UnusedAutoPropertyAccessor.Global
	public IndexLocale Locale { get; }
	
	public override IndexType Type => IndexType.Text;
	
	public override string Key => $"{string.Join('_', this.Fields)}_text";
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="field"></param>
	/// <param name="locale"></param>
	public TextIndexDefinition(string field, IndexLocale locale = IndexLocale.none)
	{
		this.Fields = new []
		{
			field
		};
		
		this.Locale = locale;
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="fields"></param>
	/// <param name="locale"></param>
	public TextIndexDefinition(IEnumerable<string> fields, IndexLocale locale = IndexLocale.none)
	{
		this.Fields = fields.ToArray();
		this.Locale = locale;
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="weightedFields"></param>
	/// <param name="locale"></param>
	public TextIndexDefinition(Dictionary<string, int> weightedFields, IndexLocale locale = IndexLocale.none)
	{
		this.WeightedFields = weightedFields;
		this.Locale = locale;
	}
	
	#endregion
}