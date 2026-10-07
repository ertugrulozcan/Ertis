namespace Ertis.MongoDB.Queries;

internal class QueryExpression : IQueryExpression, IHasChildren
{
	#region Properties
	
	/// <summary>
	/// A key starting with '$' is read by MongoDB as an operator, never as a field (e.g. "$where" runs JavaScript): it is rejected.
	/// The other segments of a path may start with '$' (e.g. the DBRef fields "owner.$id", "owner.$ref").
	/// </summary>
	public required string Field
	{
		get;
		init => field = value.StartsWith('$') ? throw new ArgumentException($"'{value}' is not a field name: a key starting with '$' is an operator (use the operator methods of the QueryBuilder)", nameof(value)) : value;
	}
	
	internal required IQuery Value
	{
		get;
		init
		{
			field = value;
			this.Children.Insert(0, value);
		}
	}
	
	public List<IQuery> Children { get; } = new();
	
	#endregion
	
	#region Methods
	
	public void AddQuery(IQuery query)
	{
		this.Children.Add(query);
	}
	
	public override string ToString()
	{
		if (this.Children.Count == 1)
		{
			var expressionJson = this.Value.ToString();
			return "{ " + QueryHelper.ToJsonString(this.Field) + ": " + expressionJson + " }";
		}
		else
		{
			var expressionJsons = new List<string>();
			foreach (var query in this.Children)
			{
				var expressionJson = QueryHelper.GetInnerQuery(query);
				if (!string.IsNullOrEmpty(expressionJson))
				{
					expressionJsons.Add(expressionJson);
				}
			}
			
			return "{ " + QueryHelper.ToJsonString(this.Field) + ": { " + string.Join(", ", expressionJsons) + " } }";
		}
	}
	
	#endregion
}