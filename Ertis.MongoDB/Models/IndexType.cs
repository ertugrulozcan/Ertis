using System.Diagnostics.CodeAnalysis;

namespace Ertis.MongoDB.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum IndexType
{
	Single,
	Compound,
	Multikey,
	Text,
	Clustered,
	Geospatial,
	Hashed,
	TTL
}