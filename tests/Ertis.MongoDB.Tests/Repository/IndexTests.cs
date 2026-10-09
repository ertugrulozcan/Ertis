using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Models;
using Ertis.MongoDB.Tests.TestHelpers;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using SortDirection = Ertis.Core.Collections.SortDirection;

namespace Ertis.MongoDB.Tests.Repository;

public class IndexTests(MongoDbContainerFixture fixture) : MongoTestBase(fixture)
{
	#region Methods
	
	private TestRepository CreateRepository()
	{
		return new TestRepository(this.ClientProvider, this.Settings);
	}
	
	private TestDynamicRepository CreateDynamicRepository()
	{
		return new TestDynamicRepository(this.ClientProvider, this.Settings);
	}
	
	/// <summary>
	/// A text index created with the driver (by hand), named by MongoDB (f1_text_f2_text)
	/// </summary>
	private async Task CreateTextIndexByHandAsync(string collectionName, params string[] fields)
	{
		var collection = this.Database.GetCollection<BsonDocument>(collectionName);
		var keys = Builders<BsonDocument>.IndexKeys.Combine(fields.Select(x => Builders<BsonDocument>.IndexKeys.Text(x)));
		await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(keys), cancellationToken: CancellationToken);
	}
	
	#endregion
	
	#region Definition Methods
	
	[Fact]
	public void Definitions_HaveTheirKeys()
	{
		Assert.Equal("age_1", new SingleIndexDefinition("age").Key);
		Assert.Equal("age_-1", new SingleIndexDefinition("age", SortDirection.Descending).ToString());
		Assert.Equal("name_1_age_-1", new CompoundIndexDefinition(new SingleIndexDefinition("name"), new SingleIndexDefinition("age", SortDirection.Descending)).Key);
		Assert.Equal("a_1_b_1", new CompoundIndexDefinition("a", "b").Key);
		Assert.Equal("a_1", new CompoundIndexDefinition(new List<SingleIndexDefinition> { new("a") }).Key);
		Assert.Equal(IndexType.TTL, new TTLIndexDefinition("created_at", SortDirection.Ascending, TimeSpan.FromHours(1)).Type);
		Assert.Throws<ArgumentException>(() => new CompoundIndexDefinition(Array.Empty<SingleIndexDefinition>()));
		Assert.Throws<ArgumentException>(() => new CompoundIndexDefinition(Array.Empty<string>()));
	}
	
	[Fact]
	public void TextIndexDefinition_HasItsFieldsAndWeights()
	{
		var single = new TextIndexDefinition("name", IndexLocale.turkish);
		var weighted = new TextIndexDefinition(new Dictionary<string, int> { ["name"] = 5, ["tags"] = 1 });
		
		Assert.Equal(["name"], single.Fields);
		Assert.Equal(new Dictionary<string, int> { ["name"] = 1 }, single.WeightedFields);
		Assert.Equal(IndexLocale.turkish, single.Locale);
		Assert.Equal(["name", "tags"], weighted.Fields);
		Assert.Equal(IndexType.Text, weighted.Type);
	}
	
	[Fact]
	public void TextIndexDefinition_KeyIsTheDefaultIndexNameOfTheSortedFields()
	{
		Assert.Equal("name_text_tags_text", new TextIndexDefinition(["name", "tags"]).Key);
		Assert.Equal("email_address_text_firstname_text_username_text", new TextIndexDefinition(["username", "firstname", "email_address"]).Key);
	}
	
	#endregion
	
	#region Create And List Methods
	
	[Fact]
	public async Task CreateAndGetIndexes_RoundTripsTheDefinitions()
	{
		var repository = this.CreateRepository();
		var definitions = new IIndexDefinition[]
		{
			new SingleIndexDefinition("age", SortDirection.Descending),
			new CompoundIndexDefinition(new SingleIndexDefinition("name"), new SingleIndexDefinition("age", SortDirection.Descending)),
			new TextIndexDefinition(["name", "tags"])
		};
		
		var names = await repository.CreateManyIndexAsync(definitions, CancellationToken);
		var indexes = (await repository.GetIndexesAsync(CancellationToken)).ToArray();
		
		Assert.Equal(["age_-1", "name_1_age_-1", "name_text_tags_text"], names);
		Assert.Equal(["_id_1", "age_-1", "name_1_age_-1", "name_text_tags_text"], indexes.Select(x => x.Key));
		var textIndex = Assert.IsType<TextIndexDefinition>(indexes[3]);
		Assert.Equal(["name", "tags"], textIndex.Fields);
	}
	
	[Fact]
	public async Task CreateIndexes_WithTheOtherOverloads_CreatesThem()
	{
		var repository = this.CreateRepository();
		
		await repository.CreateSingleIndexAsync("name", cancellationToken: CancellationToken);
		await repository.CreateSingleIndexAsync(x => x.CreatedAt, SortDirection.Descending, CancellationToken);
		await repository.CreateCompoundIndexAsync(new Dictionary<string, SortDirection> { ["a"] = SortDirection.Ascending, ["b"] = SortDirection.Descending }, CancellationToken);
		await repository.CreateCompoundIndexAsync(new Dictionary<System.Linq.Expressions.Expression<Func<TestEntity, object>>, SortDirection> { [x => x.Age] = SortDirection.Ascending, [x => x.Name!] = SortDirection.Ascending }, CancellationToken);
		await repository.CreateIndexAsync(new TTLIndexDefinition("expires_at", SortDirection.Ascending, TimeSpan.FromHours(1)), CancellationToken);
		await repository.CreateIndexAsync(new SingleIndexDefinition("slug") { IsUnique = true }, CancellationToken);
		
		var keys = (await repository.GetIndexesAsync(CancellationToken)).Select(x => x.Key).ToArray();
		
		Assert.Equal(["_id_1", "name_1", "created_at_-1", "a_1_b_-1", "age_1_name_1", "expires_at_1", "slug_1"], keys);
	}
	
	[Fact]
	public async Task CreateTextIndex_WithWeights_CreatesAWeightedIndex()
	{
		var repository = this.CreateRepository();
		
		await repository.CreateTextIndexAsync(new TextIndexDefinition(new Dictionary<string, int> { ["name"] = 5, ["tags"] = 1 }, IndexLocale.english), CancellationToken);
		
		var textIndex = Assert.IsType<TextIndexDefinition>((await repository.GetIndexesAsync(CancellationToken)).Last());
		Assert.Equal(new Dictionary<string, int> { ["name"] = 5, ["tags"] = 1 }, textIndex.WeightedFields);
		Assert.Equal(IndexLocale.english, textIndex.Locale);
	}
	
	[Fact]
	public async Task CreateTextIndex_WithoutFields_ThrowsIndexException()
	{
		await Assert.ThrowsAsync<IndexException>(() => this.CreateRepository().CreateTextIndexAsync(new TextIndexDefinition(Array.Empty<string>()), CancellationToken));
		await Assert.ThrowsAsync<IndexException>(() => this.CreateDynamicRepository().CreateTextIndexAsync(new TextIndexDefinition(Array.Empty<string>()), CancellationToken));
	}
	
	[Fact]
	public async Task GetIndexes_WithoutACollection_ReturnsNothing()
	{
		Assert.Empty(await this.CreateRepository().GetIndexesAsync(CancellationToken));
	}
	
	[Fact]
	public async Task GetIndexes_WithATextIndexCreatedByHand_ReadsItsFields()
	{
		await this.CreateTextIndexByHandAsync("entities", "name", "tags");
		
		var textIndex = Assert.IsType<TextIndexDefinition>((await this.CreateRepository().GetIndexesAsync(CancellationToken)).Last());
		
		Assert.Equal(["name", "tags"], textIndex.Fields.Order());
	}
	
	[Fact]
	public async Task GetIndexes_WithATextIndexOfSeveralFieldsCreatedByHand_DoesNotThrow()
	{
		await this.CreateTextIndexByHandAsync("entities", "username", "firstname", "lastname", "email_address");
		await this.CreateTextIndexByHandAsync("documents", "username", "firstname", "lastname", "email_address");
		
		var textIndex = Assert.IsType<TextIndexDefinition>((await this.CreateRepository().GetIndexesAsync(CancellationToken)).Last());
		var dynamicTextIndex = Assert.IsType<TextIndexDefinition>((await this.CreateDynamicRepository().GetIndexesAsync(CancellationToken)).Last());
		
		Assert.Equal(["email_address", "firstname", "lastname", "username"], textIndex.Fields.Order());
		Assert.Equal(["email_address", "firstname", "lastname", "username"], dynamicTextIndex.Fields.Order());
	}
	
	[Fact]
	public async Task GetIndexes_WithATextIndexFieldWithAnUnderscore_ReadsTheField()
	{
		var repository = this.CreateRepository();
		await repository.CreateTextIndexAsync(new TextIndexDefinition(["first_name", "email_address"]), CancellationToken);
		
		var textIndex = Assert.IsType<TextIndexDefinition>((await repository.GetIndexesAsync(CancellationToken)).Last());
		
		Assert.Equal(["email_address", "first_name"], textIndex.Fields.Order());
	}
	
	[Fact]
	public async Task GetIndexes_ReadsTheUniqueAndTtlIndexes()
	{
		var repository = this.CreateRepository();
		await repository.CreateIndexAsync(new SingleIndexDefinition("slug") { IsUnique = true }, CancellationToken);
		await repository.CreateIndexAsync(new TTLIndexDefinition("expires_at", SortDirection.Ascending, TimeSpan.FromHours(1)), CancellationToken);
		
		var indexes = (await repository.GetIndexesAsync(CancellationToken)).ToArray();
		
		Assert.True(indexes[1].IsUnique);
		var ttlIndex = Assert.IsType<TTLIndexDefinition>(indexes[2]);
		Assert.Equal(TimeSpan.FromHours(1), ttlIndex.ExpireAfter);
	}
	
	[Fact]
	public async Task GetIndexes_KeepsTheFieldOrderOfATextIndex()
	{
		var repository = this.CreateRepository();
		var definition = new TextIndexDefinition(["tags", "name"]);
		await repository.CreateIndexAsync(definition, CancellationToken);
		
		var textIndex = Assert.IsType<TextIndexDefinition>((await repository.GetIndexesAsync(CancellationToken)).Last());
		
		Assert.Equal(definition.Key, textIndex.Key);
	}
	
	[Fact]
	public async Task GetIndexes_ReadsTheOtherIndexKinds()
	{
		var collection = this.Database.GetCollection<BsonDocument>("entities");
		await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Hashed("slug")), cancellationToken: CancellationToken);
		await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("a").Descending("b"), new CreateIndexOptions { Unique = true }), cancellationToken: CancellationToken);
		await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(new BsonDocumentIndexKeysDefinition<BsonDocument>(new BsonDocument("c", -1.0))), cancellationToken: CancellationToken);
		
		var indexes = (await this.CreateRepository().GetIndexesAsync(CancellationToken)).ToArray();
		
		var hashed = Assert.IsType<SingleIndexDefinition>(indexes[1]);
		Assert.Null(hashed.Direction);
		Assert.True(Assert.IsType<CompoundIndexDefinition>(indexes[2]).IsUnique);
		Assert.Equal("c_-1", indexes[3].Key);
	}
	
	[Fact]
	public async Task CreateIndex_WithAnUnsupportedType_Throws()
	{
		var definition = Substitute.For<IIndexDefinition>();
		definition.Type.Returns(IndexType.Hashed);
		
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.CreateRepository().CreateIndexAsync(definition, CancellationToken));
	}
	
	#endregion
	
	#region Dynamic Repository Methods
	
	[Fact]
	public async Task DynamicRepository_CreateAndGetIndexes_RoundTripsTheDefinitions()
	{
		var repository = this.CreateDynamicRepository();
		
		await repository.CreateManyIndexAsync(
		[
			new SingleIndexDefinition("age"),
			new TTLIndexDefinition("expires_at", SortDirection.Descending, TimeSpan.FromMinutes(5)),
			new CompoundIndexDefinition("a", "b"),
			new TextIndexDefinition("name")
		], CancellationToken);
		await repository.CreateSingleIndexAsync("x", SortDirection.Descending, CancellationToken);
		await repository.CreateCompoundIndexAsync(new Dictionary<string, SortDirection> { ["c"] = SortDirection.Descending, ["d"] = SortDirection.Ascending }, CancellationToken);
		
		var keys = (await repository.GetIndexesAsync(CancellationToken)).Select(x => x.Key).ToArray();
		
		Assert.Equal(["_id_1", "age_1", "expires_at_-1", "a_1_b_1", "name_text", "x_-1", "c_-1_d_1"], keys);
	}
	
	#endregion
}
