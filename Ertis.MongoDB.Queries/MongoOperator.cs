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
        Exists,
        TypeOf,
        Regex,
        Text,
        RegexOptions,
        TextSearch,
        TextSearchLanguage,
        TextSearchCaseSensitive,
        TextSearchDiacriticSensitive
    }
}