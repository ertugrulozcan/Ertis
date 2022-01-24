namespace Ertis.MongoDB.Queries
{
    public interface IQuery
    {
        string ToQuery(bool addFieldName = true, bool simplifyEqualsQueries = true);
    }
}