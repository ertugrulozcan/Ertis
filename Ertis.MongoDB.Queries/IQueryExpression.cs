namespace Ertis.MongoDB.Queries
{
    public interface IQueryExpression : IQuery
    {
        string Field { get; }
    }
}