namespace Ertis.MongoDB.Queries
{
    internal enum MongoOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        NotContains,
        And,
        Or,
        Nor,
        Not,
        Exist,
        TypeOf,
        Regex,
        Text
    }
}