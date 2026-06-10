namespace Ertis.MongoDB.Queries;

internal class QueryExpression : IQueryExpression, IHasChildren
{
    #region Fields
    
    private readonly IQuery _query;
    
    #endregion
    
    #region Properties
    
    public string Field { get; init; }
    
    internal IQuery Value 
    {
        get => this._query;
        init
        {
            this._query = value;
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
            return "{ \"" + this.Field + "\": " + expressionJson + " }";
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
            
            return "{ \"" + this.Field + "\": { " + string.Join(", ", expressionJsons) + " } }";
        }
    }
    
    #endregion
}