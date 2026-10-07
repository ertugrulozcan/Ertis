using Ertis.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable UnusedMember.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Ertis.MongoDB.Tests.TestHelpers;

[BsonIgnoreExtraElements]
public sealed class TestEntity : IEntity<string>
{
	#region Properties
	
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
	
	[BsonElement("name")]
	public string? Name { get; set; }
	
	[BsonElement("age")]
	public int Age { get; set; }
	
	[BsonElement("created_at")]
	public DateTime CreatedAt { get; set; }
	
	[BsonElement("tags")]
	public string[]? Tags { get; set; }
	
	[BsonElement("owner_id")]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? OwnerId { get; set; }
	
	#endregion
}
