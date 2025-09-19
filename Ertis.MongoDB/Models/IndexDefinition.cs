using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ertis.Core.Collections;

namespace Ertis.MongoDB.Models;

public enum IndexType
{
	Single,
	Compound,
	Multikey,
	Text,
	Clustered,
	Geospatial,
	Hashed
}

public interface IIndexDefinition
{
	IndexType Type { get; }
	
	string Key { get; }
}

public enum IndexLocale
{
	none, 
	danish, 
	dutch, 
	english, 
	finnish, 
	french, 
	german, 
	hungarian, 
	italian, 
	norwegian, 
	portuguese, 
	romanian, 
	russian, 
	spanish, 
	swedish, 
	turkish, 
}

public abstract class IndexDefinitionBase : IIndexDefinition
{
	#region Properties

	public abstract IndexType Type { get; }
	
	public abstract string Key { get; }

	#endregion

	#region Methods

	public override string ToString()
	{
		return this.Key;
	}

	#endregion
}

public class SingleIndexDefinition : IndexDefinitionBase
{
	#region Properties

	public string Field { get; }
	
	public SortDirection? Direction { get; }

	public override IndexType Type => IndexType.Single;
	
	public override string Key => $"{this.Field}_{(this.Direction is SortDirection.Descending ? "-1" : "1")}";

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="field"></param>
	/// <param name="direction"></param>
	public SingleIndexDefinition(string field, SortDirection? direction = null)
	{
		this.Field = field;
		this.Direction = direction;
	}

	#endregion
}

public class CompoundIndexDefinition : IndexDefinitionBase
{
	#region Properties

	public ReadOnlyCollection<SingleIndexDefinition> Indexes { get; }

	public override IndexType Type => IndexType.Compound;

	public override string Key => string.Join('_',
		this.Indexes.Select(index => $"{index.Field}_{(index.Direction is SortDirection.Descending ? "-1" : "1")}"));

	#endregion

	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="indexes"></param>
	public CompoundIndexDefinition(IEnumerable<SingleIndexDefinition> indexes)
	{
		this.Indexes = new ReadOnlyCollection<SingleIndexDefinition>(indexes.ToList());
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="indexes"></param>
	public CompoundIndexDefinition(params SingleIndexDefinition[] indexes)
	{
		if (!indexes.Any())
		{
			throw new ArgumentException("Compound indexes must be included at least one single index");
		}
		
		this.Indexes = new ReadOnlyCollection<SingleIndexDefinition>(indexes.ToList());
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="fieldNames"></param>
	public CompoundIndexDefinition(params string[] fieldNames)
	{
		if (!fieldNames.Any())
		{
			throw new ArgumentException("Compound indexes must be included at least one single index");
		}
		
		this.Indexes = new ReadOnlyCollection<SingleIndexDefinition>(fieldNames.Select(x => new SingleIndexDefinition(x)).ToList());
	}

	#endregion
}

public class TextIndexDefinition : IndexDefinitionBase
{
	#region Fields
	
	private readonly string[] fields;
	private readonly Dictionary<string, int> weightedFields;
	
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
			
			return this.fields;
		}
		private init => this.fields = value;
	}
	
	// ReSharper disable once MemberCanBePrivate.Global
	public Dictionary<string, int> WeightedFields
	{
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