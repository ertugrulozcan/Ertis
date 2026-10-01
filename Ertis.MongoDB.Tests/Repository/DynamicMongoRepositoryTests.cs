using System.Dynamic;
using Ertis.Core.Collections;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Models;
using Ertis.MongoDB.Tests.TestHelpers;
using MongoDB.Bson;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;

namespace Ertis.MongoDB.Tests.Repository;

public class DynamicMongoRepositoryTests(MongoDbContainerFixture fixture) : MongoTestBase(fixture)
{
	#region Fields
	
	private static readonly DateTime Date = new(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
	
	#endregion
	
	#region Methods
	
	private TestDynamicRepository CreateRepository(RecordingActionBinder? actionBinder = null)
	{
		return new TestDynamicRepository(this.ClientProvider, this.Settings, actionBinder: actionBinder);
	}
	
	private async Task<(TestDynamicRepository Repository, ObjectId[] Ids)> SeedAsync()
	{
		var repository = this.CreateRepository();
		var documents = new[]
		{
			new BsonDocument { { "_id", ObjectId.GenerateNewId() }, { "name", "Jane" }, { "age", 30 }, { "created_at", Date }, { "tags", new BsonArray { "a", "b" } } },
			new BsonDocument { { "_id", ObjectId.GenerateNewId() }, { "name", "John" }, { "age", 20 }, { "created_at", Date.AddDays(1) }, { "tags", new BsonArray { "b" } } },
			new BsonDocument { { "_id", ObjectId.GenerateNewId() }, { "name", "jane" }, { "age", 40 }, { "created_at", Date.AddDays(2) }, { "tags", new BsonArray() } }
		};
		
		foreach (var document in documents)
		{
			await repository.InsertAsync(document, cancellationToken: CancellationToken);
		}
		
		return (repository, documents.Select(x => x["_id"].AsObjectId).ToArray());
	}
	
	#endregion
	
	#region Crud Methods
	
	[Fact]
	public async Task InsertAndFindOne_RoundTripsTheDocument()
	{
		var (repository, ids) = await this.SeedAsync();
		
		IDictionary<string, object?>? document = await repository.FindOneAsync(ids[0].ToString(), CancellationToken);
		IDictionary<string, object?>? syncDocument = repository.FindOne(ids[1].ToString());
		
		Assert.IsType<ExpandoObject>(document);
		Assert.Equal("Jane", document["name"]);
		Assert.Equal(ids[0], document["_id"]);
		Assert.Equal(Date, document["created_at"]);
		Assert.Equal("John", syncDocument!["name"]);
		Assert.Null(await repository.FindOneAsync(ObjectId.GenerateNewId().ToString(), CancellationToken));
	}
	
	[Fact]
	public async Task Insert_WithAnExpandoObject_InsertsIt()
	{
		var binder = new RecordingActionBinder();
		var repository = this.CreateRepository(binder);
		dynamic expando = new ExpandoObject();
		expando.name = "Jane";
		dynamic syncExpando = new ExpandoObject();
		syncExpando.name = "John";
		
		await repository.InsertAsync(expando, cancellationToken: CancellationToken);
		repository.Insert(syncExpando);
		repository.Insert(new BsonDocument("name", "Doe"));
		
		Assert.Equal(3, await repository.CountAsync(CancellationToken));
		Assert.Equal(["BeforeInsert", "AfterInsert", "BeforeInsert", "AfterInsert", "BeforeInsert", "AfterInsert"], binder.Calls.Select(x => x.Action));
	}
	
	[Fact]
	public async Task UpdateAndUpsert_ReplaceTheDocument()
	{
		var (repository, ids) = await this.SeedAsync();
		
		await repository.UpdateAsync(new BsonDocument { { "_id", ids[0] }, { "name", "Updated" } }, ids[0].ToString(), cancellationToken: CancellationToken);
		repository.Update(new BsonDocument { { "_id", ids[1] }, { "name", "Updated2" } }, ids[1].ToString());
		dynamic expando = new ExpandoObject();
		expando._id = ids[2];
		expando.name = "Updated3";
		await repository.UpsertAsync(expando, ids[2].ToString(), CancellationToken);
		await repository.UpsertAsync(new BsonDocument("name", "New"), cancellationToken: CancellationToken);
		repository.Upsert(new BsonDocument("name", "New2"), ObjectId.GenerateNewId().ToString());
		
		var names = (await repository.DistinctAsync<string>("name", (string?) null, CancellationToken)).Order();
		Assert.Equal(["New", "New2", "Updated", "Updated2", "Updated3"], names);
	}
	
	[Fact]
	public async Task Delete_RemovesTheDocuments()
	{
		var (repository, ids) = await this.SeedAsync();
		
		Assert.True(await repository.DeleteAsync(ids[0].ToString(), CancellationToken));
		Assert.True(repository.Delete(ids[1].ToString()));
		Assert.False(repository.Delete(ids[1].ToString()));
		Assert.True(await repository.BulkDeleteAsync([ids[2].ToString()], CancellationToken));
		Assert.False(repository.BulkDelete([ids[2].ToString()]));
		Assert.Equal(0, repository.Count());
	}
	
	[Fact]
	public async Task DeleteMany_WithOneMatch_ReturnsTrue()
	{
		var (repository, ids) = await this.SeedAsync();
		
		Assert.True(await repository.DeleteManyAsync($$"""{ "_id": "{{ids[0]}}" }""", CancellationToken));
		Assert.True(repository.DeleteMany("""{ "age": 20 }"""));
		Assert.Equal(1, repository.Count());
	}
	
	[Fact]
	public async Task DeleteManyAndClear_WithSeveralMatches_ReturnTrue()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.True(repository.DeleteMany("""{ "age": { "$gt": 25 } }"""));
		await this.SeedAsync();
		Assert.True(await repository.ClearAsync(CancellationToken));
		await this.SeedAsync();
		Assert.True(repository.Clear());
		Assert.Equal(0, repository.Count());
	}
	
	[Fact]
	public async Task BulkInsertAndInsertMany_InsertTheDocuments()
	{
		var binder = new RecordingActionBinder();
		var repository = this.CreateRepository(binder);
		
		await repository.BulkInsertAsync([new BsonDocument("n", 1), new BsonDocument("n", 2)], cancellationToken: CancellationToken);
		repository.BulkInsert([new BsonDocument("n", 3)]);
		await repository.InsertManyAsync([new BsonDocument("n", 4)], cancellationToken: CancellationToken);
		repository.InsertMany([new BsonDocument("n", 5)]);
		
		Assert.Equal(5, await repository.EstimatedCountAsync(CancellationToken));
		Assert.Equal(5, repository.EstimatedCount());
		Assert.Equal(10, binder.Calls.Count);
	}
	
	[Fact]
	public async Task BulkInsertAndInsertMany_StoreTheDocumentsAsTheyAre()
	{
		var repository = this.CreateRepository();
		dynamic expando = new ExpandoObject();
		expando.name = "expando";
		
		await repository.BulkInsertAsync([new BsonDocument("name", "a"), expando], cancellationToken: CancellationToken);
		repository.InsertMany([new BsonDocument("name", "b")]);
		
		var documents = await repository.DocumentCollection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync(CancellationToken);
		Assert.Equal(["a", "b", "expando"], documents.Select(x => x["name"].AsString).Order());
		Assert.All(documents, x => Assert.Equal(["_id", "name"], x.Names.Order()));
	}
	
	[Fact]
	public async Task BulkInsert_InsertsTheEntitiesReturnedByTheActionBinder()
	{
		var binder = new RecordingActionBinder { BeforeInsertReplacement = _ => new BsonDocument("replaced", true) };
		var repository = this.CreateRepository(binder);
		
		await repository.BulkInsertAsync([new BsonDocument("n", 1)], cancellationToken: CancellationToken);
		
		Assert.Equal(1, await repository.CountAsync("""{ "replaced": true }""", CancellationToken));
	}
	
	#endregion
	
	#region Find And Query Methods
	
	[Fact]
	public async Task Find_ReturnsTheMatchesAndTheCount()
	{
		var (repository, ids) = await this.SeedAsync();
		
		var result = await repository.FindAsync("""{ "age": { "$gte": 30 } }""", skip: 0, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Descending, cancellationToken: CancellationToken);
		var byId = repository.Find($$"""{ "_id": "{{ids[1]}}" }""", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null);
		var all = repository.Find(skip: 1, limit: null, withCount: true, sorting: new Sorting(new SortField("age", SortDirection.Ascending)));
		
		Assert.Equal(2, result.Count);
		Assert.Equal("jane", ((IDictionary<string, object?>) Assert.Single(result.Items))["name"]);
		Assert.Equal("John", ((IDictionary<string, object?>) Assert.Single(byId.Items))["name"]);
		Assert.Equal(3, all.Count);
		Assert.Equal(2, all.Items.Count());
	}
	
	[Fact]
	public async Task Find_WithACaseInsensitiveCollation_MatchesTheCase()
	{
		var (repository, _) = await this.SeedAsync();
		
		var result = await repository.FindAsync("""{ "name": "JANE" }""", skip: null, limit: null, withCount: null, sorting: null, indexOptions: null, collationOptions: new CollationOptions { Locale = Locale.English, CaseInsensitive = true }, CancellationToken);
		
		Assert.Equal(2, result.Items.Count());
	}
	
	[Fact]
	public async Task Query_WithACaseInsensitiveCollation_CountsWithIt()
	{
		var (repository, _) = await this.SeedAsync();
		var collation = new CollationOptions { Locale = Locale.English, CaseInsensitive = true };
		
		var result = await repository.QueryAsync("""{ "name": "JANE" }""", skip: null, limit: null, withCount: true, sorting: null, selectFields: null, indexOptions: null, collationOptions: collation, CancellationToken);
		
		Assert.Equal(2, result.Count);
	}
	
	[Fact]
	public async Task Query_ReturnsTheDocumentsAsDictionaries()
	{
		var (repository, ids) = await this.SeedAsync();
		
		var result = await repository.QueryAsync("""{ "age": 30 }""", skip: null, limit: null, withCount: true, orderBy: null, sortDirection: null, selectFields: new Dictionary<string, bool> { ["name"] = true }, cancellationToken: CancellationToken);
		var syncResult = repository.Query("""{ "age": { "$lt": 35 } }""", skip: null, limit: null, withCount: true, orderBy: "age", sortDirection: null);
		
		Assert.Equal(1, result.Count);
		var document = Assert.IsType<Dictionary<string, object>>((object) Assert.Single(result.Items));
		Assert.Equal(["_id", "name"], document.Keys.Order());
		Assert.Equal(ids[0], document["_id"]);
		Assert.Equal(2, syncResult.Count);
	}
	
	[Fact]
	public async Task Query_WithAMixedProjection_ThrowsSelectQueryInclusionException()
	{
		var (repository, _) = await this.SeedAsync();
		
		await Assert.ThrowsAsync<SelectQueryInclusionException>(() => repository.QueryAsync("{}", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, selectFields: new Dictionary<string, bool> { ["name"] = true, ["age"] = false }, cancellationToken: CancellationToken));
		Assert.Throws<SelectQueryPathCollisionException>(() => repository.Query("{}", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, selectFields: new Dictionary<string, bool> { ["tags"] = true, ["tags.x"] = true }));
	}
	
	[Fact]
	public async Task Count_CountsTheMatches()
	{
		var (repository, ids) = await this.SeedAsync();
		
		Assert.Equal(3, await repository.CountAsync(CancellationToken));
		Assert.Equal(1, repository.Count($$"""{ "_id": "{{ids[0]}}" }"""));
		Assert.Equal(2, await repository.CountAsync("""{ "age": { "$gt": 25 } }""", CancellationToken));
		Assert.Equal(3, repository.Count((IndexOptions?) null));
	}
	
	[Fact]
	public async Task Distinct_ReturnsTheDistinctValues()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.Equal(["b"], await repository.DistinctAsync<string>("tags", """{ "age": 20 }""", CancellationToken));
		Assert.Equal(["a", "b"], (await repository.DistinctAsync<string>("tags", (System.Linq.Expressions.Expression<Func<dynamic, bool>>?) null, CancellationToken)).Order());
	}
	
	[Fact]
	public async Task Distinct_Synchronous_ReturnsTheDistinctValues()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.Equal(["b"], repository.Distinct<string>("tags", """{ "age": 20 }"""));
	}
	
	[Fact]
	public async Task Aggregate_RunsThePipeline()
	{
		var (repository, ids) = await this.SeedAsync();
		
		IEnumerable<object> result = await repository.AggregateAsync($$"""[{ "$match": { "_id": "{{ids[0]}}" } }, { "$project": { "name": 1 } }]""", cancellationToken: CancellationToken);
		IEnumerable<object> syncResult = repository.Aggregate("""[{ $group: { _id: null, total: { $sum: "$age" } } }]""");
		
		Assert.Equal("Jane", ((Dictionary<string, object>) Assert.Single(result))["name"]);
		Assert.Equal(90, ((Dictionary<string, object>) Assert.Single(syncResult))["total"]);
	}
	
	[Fact]
	public async Task Aggregate_WithAnInvalidProjection_ThrowsTheErtisException()
	{
		var (repository, _) = await this.SeedAsync();
		
		await Assert.ThrowsAsync<SelectQueryInclusionException>(() => repository.AggregateAsync("""[{ "$project": { "name": 1, "age": 0 } }]""", cancellationToken: CancellationToken));
	}
	
	#endregion
}
