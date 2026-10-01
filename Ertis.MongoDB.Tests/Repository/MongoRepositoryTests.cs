using Ertis.Core.Collections;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Models;
using Ertis.MongoDB.Tests.TestHelpers;
using MongoDB.Bson;

// ReSharper disable MethodHasAsyncOverload
namespace Ertis.MongoDB.Tests.Repository;

public class MongoRepositoryTests(MongoDbContainerFixture fixture) : MongoTestBase(fixture)
{
	#region Fields
	
	private static readonly DateTime Date = new(2026, 1, 31, 10, 0, 0, DateTimeKind.Utc);
	
	#endregion
	
	#region Methods
	
	private TestRepository CreateRepository(RecordingActionBinder? actionBinder = null)
	{
		return new TestRepository(this.ClientProvider, this.Settings, actionBinder: actionBinder);
	}
	
	private async Task<(TestRepository Repository, TestEntity[] Entities)> SeedAsync()
	{
		var repository = this.CreateRepository();
		var entities = new[]
		{
			new TestEntity { Name = "Jane", Age = 30, CreatedAt = Date, Tags = ["a", "b"] },
			new TestEntity { Name = "John", Age = 20, CreatedAt = Date.AddDays(1), Tags = ["b"] },
			new TestEntity { Name = "jane", Age = 40, CreatedAt = Date.AddDays(2), Tags = [] }
		};
		
		foreach (var entity in entities)
		{
			await repository.InsertAsync(entity, cancellationToken: CancellationToken);
		}
		
		return (repository, entities);
	}
	
	#endregion
	
	#region Crud Methods
	
	[Fact]
	public async Task InsertAndFindOne_RoundTripsTheEntity()
	{
		var (repository, entities) = await this.SeedAsync();
		
		var entity = await repository.FindOneAsync(entities[0].Id, CancellationToken);
		
		Assert.NotNull(entity);
		Assert.Equal("Jane", entity.Name);
		Assert.Equal(Date, entity.CreatedAt);
		Assert.Equal("John", repository.FindOne(entities[1].Id)?.Name);
		Assert.Equal("John", repository.FindOne(x => x.Age == 20)?.Name);
		Assert.Equal("John", (await repository.FindOneAsync(x => x.Age == 20, CancellationToken))?.Name);
		Assert.Null(await repository.FindOneAsync(ObjectId.GenerateNewId().ToString(), CancellationToken));
	}
	
	[Fact]
	public async Task Insert_CallsTheActionBinder()
	{
		var binder = new RecordingActionBinder();
		var repository = this.CreateRepository(binder);
		
		repository.Insert(new TestEntity { Name = "a" });
		await repository.InsertAsync(new TestEntity { Name = "b" }, new Data.Models.InsertOptions { TriggerBeforeActionBinder = false, TriggerAfterActionBinder = true }, CancellationToken);
		
		Assert.Equal(["BeforeInsert", "AfterInsert", "AfterInsert"], binder.Calls.Select(x => x.Action));
	}
	
	[Fact]
	public async Task UpdateAndUpsert_ReplaceTheEntity()
	{
		var binder = new RecordingActionBinder();
		var repository = this.CreateRepository(binder);
		var entity = new TestEntity { Name = "a", Age = 1 };
		await repository.InsertAsync(entity, cancellationToken: CancellationToken);
		
		entity.Age = 2;
		await repository.UpdateAsync(entity, cancellationToken: CancellationToken);
		entity.Age = 3;
		repository.Update(entity);
		Assert.Equal(3, (await repository.FindOneAsync(entity.Id, CancellationToken))?.Age);
		
		entity.Age = 4;
		await repository.UpsertAsync(entity, cancellationToken: CancellationToken);
		var newEntity = new TestEntity { Name = "new" };
		await repository.UpsertAsync(newEntity, cancellationToken: CancellationToken);
		repository.Upsert(new TestEntity { Name = "new2" });
		
		Assert.Equal(4, (await repository.FindOneAsync(entity.Id, CancellationToken))?.Age);
		Assert.Equal(3, await repository.CountAsync(CancellationToken));
		Assert.Contains("BeforeUpdate", binder.Calls.Select(x => x.Action));
		Assert.Contains("AfterUpdate", binder.Calls.Select(x => x.Action));
	}
	
	[Fact]
	public async Task Delete_RemovesTheEntity()
	{
		var (repository, entities) = await this.SeedAsync();
		
		Assert.True(await repository.DeleteAsync(entities[0].Id, CancellationToken));
		Assert.True(repository.Delete(entities[1].Id));
		Assert.False(await repository.DeleteAsync(entities[0].Id, CancellationToken));
		Assert.Equal(1, repository.Count());
	}
	
	[Fact]
	public async Task BulkDelete_RemovesTheEntities()
	{
		var (repository, entities) = await this.SeedAsync();
		
		Assert.True(await repository.BulkDeleteAsync(entities[..2], CancellationToken));
		Assert.False(repository.BulkDelete(entities));
		Assert.Equal(0, repository.Count());
	}
	
	/// <summary>
	/// Characterization: true only when exactly one document is deleted
	/// </summary>
	[Fact]
	public async Task DeleteMany_WithOneMatch_ReturnsTrue()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.True(await repository.DeleteManyAsync(x => x.Age == 20, CancellationToken));
		Assert.True(repository.DeleteMany("""{ "age": 30 }"""));
		Assert.Equal(1, repository.Count());
	}
	
	[Fact]
	public async Task DeleteManyAndClear_WithSeveralMatches_ReturnTrue()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.True(await repository.DeleteManyAsync("""{ "age": { "$gt": 25 } }""", CancellationToken));
		Assert.Equal(1, repository.Count());
		
		await this.SeedAsync();
		Assert.True(repository.Clear());
		await this.SeedAsync();
		Assert.True(await repository.ClearAsync(CancellationToken));
		Assert.True(repository.DeleteMany(x => x.Age > 0));
		Assert.Equal(0, repository.Count());
	}
	
	[Fact]
	public async Task BulkInsert_InsertsTheEntities()
	{
		var repository = this.CreateRepository();
		
		await repository.BulkInsertAsync([new TestEntity { Name = "a" }, new TestEntity { Name = "b" }], cancellationToken: CancellationToken);
		repository.BulkInsert([new TestEntity { Name = "c" }]);
		
		Assert.Equal(3, await repository.EstimatedCountAsync(CancellationToken));
		Assert.Equal(3, repository.EstimatedCount());
	}
	
	[Fact]
	public async Task BulkInsert_CallsTheActionBinder()
	{
		var binder = new RecordingActionBinder();
		var repository = this.CreateRepository(binder);
		
		await repository.BulkInsertAsync([new TestEntity { Name = "a" }, new TestEntity { Name = "b" }], cancellationToken: CancellationToken);
		
		Assert.Equal(["BeforeInsert", "BeforeInsert", "AfterInsert", "AfterInsert"], binder.Calls.Select(x => x.Action));
	}
	
	[Fact]
	public async Task Increment_IncrementsTheField()
	{
		var (repository, entities) = await this.SeedAsync();
		
		await repository.IncrementAsync(entities[0].Id, "age", 2, CancellationToken);
		repository.Increment(entities[0].Id, x => x.Age, 3);
		await repository.IncrementAsync(x => x.Name == "Jane", x => x.Age, 5, CancellationToken);
		repository.Increment(x => x.Name == "Jane", "age", 10L);
		
		Assert.Equal(50, (await repository.FindOneAsync(entities[0].Id, CancellationToken))?.Age);
	}
	
	#endregion
	
	#region Find Methods
	
	[Fact]
	public async Task Find_WithAQuery_ReturnsTheMatchesAndTheCount()
	{
		var (repository, _) = await this.SeedAsync();
		
		var result = await repository.FindAsync("""{ "age": { "$gte": 30 } }""", skip: 0, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Descending, cancellationToken: CancellationToken);
		
		Assert.Equal(2, result.Count);
		Assert.Equal(["jane"], result.Items.Select(x => x.Name));
		Assert.Equal(3, repository.Find(skip: null, limit: null, withCount: true, orderBy: null, sortDirection: null).Count);
		Assert.Equal(["John"], repository.Find(x => x.Age < 25, skip: null, limit: null, withCount: false, orderBy: null, sortDirection: null).Items.Select(x => x.Name));
	}
	
	[Fact]
	public async Task Find_WithAnIdString_MatchesTheObjectId()
	{
		var (repository, entities) = await this.SeedAsync();
		
		var result = await repository.FindAsync($$"""{ "_id": "{{entities[1].Id}}" }""", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, cancellationToken: CancellationToken);
		
		Assert.Equal("John", Assert.Single(result.Items).Name);
	}
	
	[Fact]
	public async Task Find_WithSkipAndLimit_PagesTheResults()
	{
		var (repository, _) = await this.SeedAsync();
		
		var sorting = new Sorting(new SortField("age", SortDirection.Ascending));
		Assert.Equal([30, 40], repository.Find(skip: 1, limit: null, withCount: null, sorting: sorting).Items.Select(x => x.Age));
		Assert.Equal([20], (await repository.FindAsync(skip: null, limit: 1, withCount: null, sorting: sorting, cancellationToken: CancellationToken)).Items.Select(x => x.Age));
		Assert.Equal([30], (await repository.FindAsync(skip: 1, limit: 1, withCount: null, sorting: sorting, cancellationToken: CancellationToken)).Items.Select(x => x.Age));
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
		var found = await repository.FindAsync("""{ "name": "JANE" }""", skip: null, limit: null, withCount: true, sorting: null, indexOptions: null, collationOptions: collation, CancellationToken);
		
		Assert.Equal(2, result.Count);
		Assert.Equal(2, found.Count);
	}
	
	[Fact]
	public async Task Find_WithAnIndexHint_UsesTheIndex()
	{
		var (repository, _) = await this.SeedAsync();
		await repository.CreateSingleIndexAsync("age", cancellationToken: CancellationToken);
		var indexOptions = new IndexOptions { Hint = [new SingleIndexDefinition("age")] };
		
		var result = await repository.FindAsync("""{ "age": { "$gt": 25 } }""", skip: null, limit: null, withCount: true, sorting: null, indexOptions: indexOptions, collationOptions: null, CancellationToken);
		
		Assert.Equal(2, result.Count);
		Assert.Equal(2, await repository.CountAsync("""{ "age": { "$gt": 25 } }""", indexOptions, CancellationToken));
		Assert.Equal(3, repository.Count(indexOptions));
	}
	
	[Fact]
	public async Task Find_WithAUtcDateString_MatchesTheInstant()
	{
		var (repository, _) = await this.SeedAsync();
		
		var result = await repository.FindAsync("""{ "created_at": "2026-01-31T10:00:00Z" }""", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, cancellationToken: CancellationToken);
		
		Assert.Equal("Jane", Assert.Single(result.Items).Name);
	}
	
	[Fact]
	public async Task Find_WithADateStringWithAnOffset_MatchesTheInstant()
	{
		var repository = this.CreateRepository();
		await repository.InsertAsync(new TestEntity { Name = "ms", CreatedAt = Date.AddMilliseconds(123) }, cancellationToken: CancellationToken);
		
		var withOffset = await repository.CountAsync("""{ "created_at": "2026-01-31T13:00:00.123+03:00" }""", CancellationToken);
		var withMilliseconds = await repository.CountAsync("""{ "created_at": "2026-01-31T10:00:00.123Z" }""", CancellationToken);
		
		Assert.Equal(1, withOffset);
		Assert.Equal(1, withMilliseconds);
	}
	
	#endregion
	
	#region Query Methods
	
	[Fact]
	public async Task Query_ReturnsTheDocumentsAsDictionaries()
	{
		var (repository, entities) = await this.SeedAsync();
		
		var result = await repository.QueryAsync("""{ "age": 30 }""", skip: null, limit: null, withCount: true, orderBy: null, sortDirection: null, cancellationToken: CancellationToken);
		
		Assert.Equal(1, result.Count);
		var document = Assert.IsType<Dictionary<string, object>>((object) Assert.Single(result.Items));
		Assert.Equal(ObjectId.Parse(entities[0].Id), document["_id"]);
		Assert.Equal("Jane", document["name"]);
		Assert.Equal(Date, document["created_at"]);
	}
	
	[Fact]
	public async Task Query_WithSelectFields_ProjectsTheDocuments()
	{
		var (repository, _) = await this.SeedAsync();
		
		var included = repository.Query("{}", skip: null, limit: null, withCount: null, orderBy: "age", sortDirection: null, selectFields: new Dictionary<string, bool> { ["name"] = true });
		var excluded = await repository.QueryAsync(x => x.Age == 20, skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, selectFields: new Dictionary<string, bool> { ["tags"] = false, ["created_at"] = false }, cancellationToken: CancellationToken);
		
		Assert.All(included.Items, x => Assert.Equal(["_id", "name"], ((Dictionary<string, object>) x).Keys.Order()));
		Assert.Equal(["_id", "age", "name", "owner_id"], ((Dictionary<string, object>) Assert.Single(excluded.Items)).Keys.Order());
	}
	
	[Fact]
	public async Task Query_WithAMixedProjection_ThrowsSelectQueryInclusionException()
	{
		var (repository, _) = await this.SeedAsync();
		
		await Assert.ThrowsAsync<SelectQueryInclusionException>(() => repository.QueryAsync("{}", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, selectFields: new Dictionary<string, bool> { ["name"] = true, ["age"] = false }, cancellationToken: CancellationToken));
	}
	
	[Fact]
	public async Task Query_WithAPathCollision_ThrowsSelectQueryPathCollisionException()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.Throws<SelectQueryPathCollisionException>(() => repository.Query("{}", skip: null, limit: null, withCount: null, orderBy: null, sortDirection: null, selectFields: new Dictionary<string, bool> { ["tags"] = true, ["tags.x"] = true }));
	}
	
	[Fact]
	public async Task QueryOfT_DeserializesTheDocuments()
	{
		var (repository, _) = await this.SeedAsync();
		
		var result = await repository.QueryAsync<TestEntity>("""{ "age": { "$lt": 35 } }""", skip: null, limit: null, withCount: true, orderBy: "age", sortDirection: null, cancellationToken: CancellationToken);
		var expressionResult = repository.Query<TestEntity>(x => x.Age > 35, skip: null, limit: null, withCount: true, orderBy: null, sortDirection: null);
		
		Assert.Equal(["John", "Jane"], result.Items.Select(x => x.Name));
		Assert.Equal(["jane"], expressionResult.Items.Select(x => x.Name));
	}
	
	[Fact]
	public async Task Distinct_ReturnsTheDistinctValues()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.Equal(["a", "b"], (await repository.DistinctAsync<string>("tags", (string?) null, CancellationToken)).Order());
		Assert.Equal(["b"], await repository.DistinctAsync<string>("tags", """{ "age": 20 }""", CancellationToken));
		Assert.Equal([30, 40], (await repository.DistinctAsync<int>("age", x => x.Age > 25, CancellationToken)).Order());
	}
	
	[Fact]
	public async Task Distinct_Synchronous_ReturnsTheDistinctValues()
	{
		var (repository, _) = await this.SeedAsync();
		
		Assert.Equal(["b"], repository.Distinct<string>("tags", """{ "age": 20 }"""));
		Assert.Equal([20], repository.Distinct<int>("age", x => x.Name == "John"));
	}
	
	[Fact]
	public async Task Aggregate_RunsThePipeline()
	{
		var (repository, entities) = await this.SeedAsync();
		
		IEnumerable<object> result = await repository.AggregateAsync($$"""[{ "$match": { "_id": "{{entities[0].Id}}" } }, { "$group": { "_id": null, "total": { "$sum": "$age" } } }]""", cancellationToken: CancellationToken);
		IEnumerable<object> syncResult = repository.Aggregate("""[{ "$sort": { "age": 1 } }, { "$limit": 1 }]""");
		
		Assert.Equal(30, ((Dictionary<string, object>) Assert.Single(result))["total"]);
		Assert.Equal("John", ((Dictionary<string, object>) Assert.Single(syncResult))["name"]);
	}
	
	[Fact]
	public async Task Aggregate_ConvertsTheIdsUnderOperators()
	{
		var (repository, entities) = await this.SeedAsync();
		
		IEnumerable<object> result = await repository.AggregateAsync($$"""[{ "$match": { "_id": { "$ne": "{{entities[0].Id}}" } } }, { "$group": { "_id": null, "total": { "$sum": "$age" } } }]""", cancellationToken: CancellationToken);
		
		Assert.Equal(60, ((Dictionary<string, object>) Assert.Single(result))["total"]);
	}
	
	[Fact]
	public async Task Search_FindsTheEntitiesByText()
	{
		var (repository, _) = await this.SeedAsync();
		await repository.CreateTextIndexAsync(new TextIndexDefinition("name"), CancellationToken);
		
		var result = await repository.SearchAsync("jane", withCount: true, cancellationToken: CancellationToken);
		var syncResult = repository.Search("john", new Queries.TextSearchOptions());
		
		Assert.Equal(2, result.Count);
		Assert.All(result.Items, x => Assert.Equal("jane", x.Name?.ToLowerInvariant()));
		Assert.Equal("John", Assert.Single(syncResult.Items).Name);
	}
	
	[Fact]
	public async Task Count_CountsTheMatches()
	{
		var (repository, entities) = await this.SeedAsync();
		
		Assert.Equal(3, await repository.CountAsync(CancellationToken));
		Assert.Equal(2, repository.Count(x => x.Age > 25));
		Assert.Equal(2, await repository.CountAsync(x => x.Age > 25, CancellationToken));
		Assert.Equal(1, repository.Count($$"""{ "_id": "{{entities[0].Id}}" }"""));
		Assert.Equal(1, await repository.CountAsync("""{ "age": 20 }""", CancellationToken));
	}
	
	#endregion
}
