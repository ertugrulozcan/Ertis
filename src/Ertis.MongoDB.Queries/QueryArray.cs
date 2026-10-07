namespace Ertis.MongoDB.Queries;

internal class QueryArray : List<IQuery>, IQuery
{
	#region Properties
	
	internal MongoOperator? Operator { get; init; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="queries"></param>
	public QueryArray(IEnumerable<IQuery> queries)
	{
		this.AddRange(queries);
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// An $and of field expressions with distinct fields is written as one object of the fields ({ "a": 1, "b": 2 })
	/// </summary>
	internal bool IsFlattened => this.Operator == MongoOperator.And && this.All(x => x is IQueryExpression) && this.Cast<IQueryExpression>().Select(x => x.Field).Distinct().Count() == this.Count;
	
	public override string ToString()
	{
		if (this.Operator != null)
		{
			// Simplify $and operators
			// The field expressions are merged into one object, unless a field is repeated (a duplicate key would drop a condition)
			if (this.IsFlattened)
			{
				return "{ " + string.Join(", ", this.Select(x => x.ToString().Trim().Trim('{').Trim('}').Trim())) + " }";
			}
			
			var operatorTag = QueryHelper.GetOperatorTag(this.Operator.Value);
			return "{ \"$" + operatorTag + "\": [ " + string.Join(", ", this.Select(x => x.ToString())) + " ] }";
		}
		else
		{
			return "[ " + string.Join(", ", this.Select(x => x.ToString())) + " ]";   
		}
	}
	
	#endregion
}