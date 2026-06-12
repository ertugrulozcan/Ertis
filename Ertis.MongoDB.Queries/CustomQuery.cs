using System.Diagnostics.CodeAnalysis;

namespace Ertis.MongoDB.Queries;

internal class CustomQuery : IQuery, IHasChildren
{
    #region Properties
    
    internal string? Operator { get; init; }
    
    [field: AllowNull, MaybeNull]
    // ReSharper disable once UnusedMember.Global
    internal IQuery Value
    {
        get;
        init
        {
            field = value;
            this.Children.Insert(0, value);
        }
    }
    
    public List<IQuery> Children { get; init; } = new();
    
    public bool ShowOperatorTag { get; init; } = true;
    
    #endregion
    
    #region Methods
    
    public void AddQuery(IQuery query)
    {
        this.Children.Add(query);
    }
    
    public override string ToString()
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
        
        if (this.ShowOperatorTag && !string.IsNullOrEmpty(this.Operator))
        {
            return "{ \"" + this.Operator + "\": { " + string.Join(", ", expressionJsons) + " } }";
        }
        else
        {
            return "{ " + string.Join(", ", expressionJsons) + " }";
        }
    }
    
    #endregion
}