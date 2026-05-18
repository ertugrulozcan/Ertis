using System.Diagnostics.CodeAnalysis;

namespace Ertis.MongoDB.Queries;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum BsonType
{
	Double,
	String,
	Object,
	Array,
	BinData,
	Undefined,
	ObjectId,
	Bool,
	Date,
	Null,
	Regex,
	DbPointer,
	Javascript,
	Symbol,
	JavascriptWithScope,
	Int,
	TimeStamp,
	Long,
	Decimal,
	MinKey,
	MaxKey
}