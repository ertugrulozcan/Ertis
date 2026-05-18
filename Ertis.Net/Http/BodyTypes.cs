using System.Diagnostics.CodeAnalysis;

namespace Ertis.Net.Http;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum BodyTypes
{
	None,
	FormData,
	UrlEncoded,
	Text,
	Javascript,
	Json,
	Html,
	Xml,
	Binary,
	MongoQuery,
	GraphQL
}