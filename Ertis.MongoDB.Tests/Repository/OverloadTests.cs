using Ertis.Core.Collections;
using Ertis.MongoDB.Exceptions;
using Ertis.MongoDB.Models;
using Ertis.MongoDB.Tests.TestHelpers;
using MongoDB.Bson;
using MongoDB.Driver;
using SortDirection = Ertis.Core.Collections.SortDirection;

// ReSharper disable MethodHasAsyncOverload
namespace Ertis.MongoDB.Tests.Repository;

/// <summary>
/// The overloads delegate to the same implementations; every overload is called once with the same data
/// </summary>
public class OverloadTests(MongoDbContainerFixture fixture) : MongoTestBase(fixture)
{
	#region Fields
	
	private static readonly Sorting AgeSorting = new(new SortField("age", SortDirection.Ascending));
	
	private static readonly IndexOptions NoHint = new();
	
	private static readonly Dictionary<string, bool> MixedProjection = new() { ["name"] = true, ["age"] = false };
	
	#endregion
	
	#region Methods
	
	private async Task<TestRepository> SeedRepositoryAsync()
	{
		var repository = new TestRepository(this.ClientProvider, this.Settings);
		await repository.BulkInsertAsync(
		[
			new TestEntity { Name = "a", Age = 1 },
			new TestEntity { Name = "b", Age = 2 },
			new TestEntity { Name = "c", Age = 3 }
		], cancellationToken: CancellationToken);
		
		return repository;
	}
	
	private async Task<TestDynamicRepository> SeedDynamicRepositoryAsync()
	{
		var repository = new TestDynamicRepository(this.ClientProvider, this.Settings);
		await repository.BulkInsertAsync(
		[
			new BsonDocument { { "name", "a" }, { "age", 1 } },
			new BsonDocument { { "name", "b" }, { "age", 2 } },
			new BsonDocument { { "name", "c" }, { "age", 3 } }
		], cancellationToken: CancellationToken);
		
		return repository;
	}
	
	#endregion
	
	#region Repository Methods
	
	[Fact]
	public async Task Find_Overloads_ReturnTheSameResults()
	{
		var repository = await this.SeedRepositoryAsync();
		
		var results = new[]
		{
			await repository.FindAsync(skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken),
			repository.Find(x => x.Age > 0, skip: 1, limit: 1, withCount: true, sorting: AgeSorting),
			await repository.FindAsync(x => x.Age > 0, skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken),
			await repository.FindAsync(x => x.Age > 0, skip: 1, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken),
			repository.Find("{}", skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending),
			repository.Find("{}", skip: 1, limit: 1, withCount: true, sorting: AgeSorting),
			await repository.FindAsync("{}", skip: 1, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken),
			repository.Find("{}", skip: 1, limit: 1, withCount: true, sorting: AgeSorting, indexOptions: NoHint, collationOptions: null),
			repository.Find(skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, indexOptions: NoHint, collationOptions: null),
			await repository.FindAsync(skip: 1, limit: 1, withCount: true, sorting: AgeSorting, indexOptions: NoHint, collationOptions: null, cancellationToken: CancellationToken)
		};
		
		Assert.All(results, x =>
		{
			Assert.Equal(3, x.Count);
			Assert.Equal("b", Assert.Single(x.Items).Name);
		});
	}
	
	[Fact]
	public async Task Query_Overloads_ReturnTheSameResults()
	{
		var repository = await this.SeedRepositoryAsync();
		
		var dynamicResults = new[]
		{
			repository.Query(x => x.Age > 1, skip: 0, limit: 1, withCount: true, sorting: AgeSorting),
			repository.Query(x => x.Age > 1, skip: 0, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending),
			repository.Query("""{ "age": { "$gt": 1 } }""", skip: 0, limit: 1, withCount: true, sorting: AgeSorting),
			await repository.QueryAsync(x => x.Age > 1, skip: 0, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken)
		};
		var typedResults = new[]
		{
			repository.Query<TestEntity>(x => x.Age > 1, skip: 0, limit: 1, withCount: true, sorting: AgeSorting),
			repository.Query<TestEntity>("""{ "age": { "$gt": 1 } }""", skip: 0, limit: 1, withCount: true, sorting: AgeSorting),
			await repository.QueryAsync<TestEntity>(x => x.Age > 1, skip: 0, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken),
			await repository.QueryAsync<TestEntity>(x => x.Age > 1, skip: 0, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken)
		};
		
		Assert.All(dynamicResults, x => Assert.Equal("b", ((Dictionary<string, object>) Assert.Single(x.Items))["name"]));
		Assert.All(typedResults, x => Assert.Equal("b", Assert.Single(x.Items).Name));
		Assert.All(dynamicResults.Select(x => x.Count).Concat(typedResults.Select(x => x.Count)), x => Assert.Equal(2, x));
	}
	
	[Fact]
	public async Task Query_Overloads_MapTheProjectionErrors()
	{
		var repository = await this.SeedRepositoryAsync();
		
		Assert.Throws<SelectQueryInclusionException>(() => repository.Query<TestEntity>("{}", selectFields: MixedProjection, skip: null, limit: null, withCount: null, sorting: null));
		await Assert.ThrowsAsync<SelectQueryInclusionException>(() => repository.QueryAsync<TestEntity>("{}", selectFields: MixedProjection, skip: null, limit: null, withCount: null, sorting: null, cancellationToken: CancellationToken));
		Assert.Throws<SelectQueryInclusionException>(() => repository.Query(x => true, selectFields: MixedProjection, skip: null, limit: null, withCount: null, sorting: null));
		await Assert.ThrowsAsync<SelectQueryInclusionException>(() => repository.QueryAsync(x => true, selectFields: MixedProjection, skip: null, limit: null, withCount: null, sorting: null, cancellationToken: CancellationToken));
		Assert.Throws<SelectQueryInclusionException>(() => repository.Aggregate("""[{ "$project": { "name": 1, "age": 0 } }]"""));
		await Assert.ThrowsAsync<SelectQueryInclusionException>(() => repository.AggregateAsync("""[{ "$project": { "name": 1, "age": 0 } }]""", cancellationToken: CancellationToken));
	}
	
	[Fact]
	public async Task CountAndUpsert_Overloads_Work()
	{
		var repository = await this.SeedRepositoryAsync();
		var entity = repository.Find(x => x.Name == "a", skip: null, limit: null, withCount: null, sorting: null).Items.Single();
		
		entity.Age = 10;
		repository.Upsert(entity);
		
		Assert.Equal(3, await repository.CountAsync(NoHint, CancellationToken));
		Assert.Equal(1, await repository.CountAsync(x => x.Age == 10, NoHint, CancellationToken));
		Assert.Equal(1, repository.Count(x => x.Age == 10, NoHint));
		Assert.Equal(1, repository.Count("""{ "age": 10 }""", NoHint));
		Assert.Equal(1, await repository.CountAsync("""{ "age": 10 }""", CancellationToken));
	}
	
	[Fact]
	public async Task Increment_Overloads_IncrementTheField()
	{
		var repository = await this.SeedRepositoryAsync();
		var id = repository.Find(x => x.Name == "a", skip: null, limit: null, withCount: null, sorting: null).Items.Single().Id;
		
		repository.Increment(id, "age", 1);
		repository.Increment(id, "age", 1L);
		await repository.IncrementAsync(id, "age", 1L, CancellationToken);
		repository.Increment(id, x => x.Age, 1L);
		await repository.IncrementAsync(id, x => x.Age, 1, CancellationToken);
		await repository.IncrementAsync(id, x => x.Age, 1L, CancellationToken);
		repository.Increment(x => x.Name == "a", "age", 1);
		await repository.IncrementAsync(x => x.Name == "a", "age", 1, CancellationToken);
		await repository.IncrementAsync(x => x.Name == "a", "age", 1L, CancellationToken);
		repository.Increment(x => x.Name == "a", x => x.Age);
		repository.Increment(x => x.Name == "a", x => x.Age, 1L);
		await repository.IncrementAsync(x => x.Name == "a", x => x.Age, 1L, CancellationToken);
		
		Assert.Equal(13, (await repository.FindOneAsync(id, CancellationToken))?.Age);
	}
	
	[Fact]
	public async Task CreateIndex_Overloads_CreateTheIndexes()
	{
		var repository = new TestRepository(this.ClientProvider, this.Settings);
		var dynamicRepository = new TestDynamicRepository(this.ClientProvider, this.Settings);
		
		await repository.CreateSingleIndexAsync(new SingleIndexDefinition("a"), CancellationToken);
		await repository.CreateTTLIndexAsync(new TTLIndexDefinition("b", SortDirection.Ascending, TimeSpan.FromHours(1)), CancellationToken);
		await repository.CreateCompoundIndexAsync(new CompoundIndexDefinition("c", "d"), CancellationToken);
		await dynamicRepository.CreateSingleIndexAsync(new SingleIndexDefinition("a"), CancellationToken);
		await dynamicRepository.CreateTTLIndexAsync(new TTLIndexDefinition("b", SortDirection.Ascending, TimeSpan.FromHours(1)), CancellationToken);
		await dynamicRepository.CreateCompoundIndexAsync(new CompoundIndexDefinition("c", "d"), CancellationToken);
		await dynamicRepository.CreateTextIndexAsync(new TextIndexDefinition("e"), CancellationToken);
		
		Assert.Equal(["_id_1", "a_1", "b_1", "c_1_d_1"], (await repository.GetIndexesAsync(CancellationToken)).Select(x => x.Key));
		Assert.Equal(["_id_1", "a_1", "b_1", "c_1_d_1", "e_text"], (await dynamicRepository.GetIndexesAsync(CancellationToken)).Select(x => x.Key));
	}
	
	#endregion
	
	#region Dynamic Repository Methods
	
	[Fact]
	public async Task DynamicFind_Overloads_ReturnTheSameResults()
	{
		var repository = await this.SeedDynamicRepositoryAsync();
		
		var results = new[]
		{
			repository.Find(skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending),
			await repository.FindAsync(skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken),
			await repository.FindAsync(skip: 1, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken),
			repository.Find(x => true, skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending),
			repository.Find(x => true, skip: 1, limit: 1, withCount: true, sorting: AgeSorting),
			await repository.FindAsync(x => true, skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken),
			await repository.FindAsync(x => true, skip: 1, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken),
			repository.Find("{}", skip: 1, limit: 1, withCount: true, sorting: AgeSorting),
			await repository.FindAsync("{}", skip: 1, limit: 1, withCount: true, sorting: AgeSorting, cancellationToken: CancellationToken),
			await repository.FindAsync("{}", skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken),
			repository.Find(skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, indexOptions: NoHint, collationOptions: null),
			await repository.FindAsync(skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, indexOptions: NoHint, collationOptions: null, cancellationToken: CancellationToken),
			await repository.FindAsync(skip: 1, limit: null, withCount: true, sorting: AgeSorting, indexOptions: NoHint, collationOptions: null, cancellationToken: CancellationToken),
			repository.Find("{}", skip: null, limit: 1, withCount: true, sorting: AgeSorting, indexOptions: NoHint, collationOptions: null)
		};
		
		Assert.All(results, x => Assert.Equal(3, x.Count));
		Assert.Equal("b", ((IDictionary<string, object?>) results[0].Items.Single())["name"]);
		Assert.Equal(2, results[12].Items.Count());
		Assert.Equal("a", ((IDictionary<string, object?>) results[13].Items.Single())["name"]);
	}
	
	[Fact]
	public async Task DynamicQueryAndCount_Overloads_Work()
	{
		var repository = await this.SeedDynamicRepositoryAsync();
		
		var results = new[]
		{
			repository.Query(x => true, skip: 1, limit: 1, withCount: true, sorting: AgeSorting),
			repository.Query(x => true, skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending),
			await repository.QueryAsync(x => true, skip: 1, limit: 1, withCount: true, orderBy: "age", sortDirection: SortDirection.Ascending, cancellationToken: CancellationToken)
		};
		
		Assert.All(results, x => Assert.Equal("b", ((Dictionary<string, object>) Assert.Single(x.Items))["name"]));
		Assert.Throws<SelectQueryInclusionException>(() => repository.Query(x => true, selectFields: MixedProjection, skip: null, limit: null, withCount: null, sorting: null));
		Assert.Throws<SelectQueryInclusionException>(() => repository.Aggregate("""[{ "$project": { "name": 1, "age": 0 } }]"""));
		Assert.Equal(3, await repository.CountAsync(NoHint, CancellationToken));
		Assert.Equal(3, await repository.CountAsync(x => true, NoHint, CancellationToken));
		Assert.Equal(3, repository.Count(x => true));
		Assert.Equal(1, repository.Count("""{ "age": 1 }""", NoHint));
		Assert.Equal(1, await repository.CountAsync("""{ "age": 1 }""", NoHint, CancellationToken));
		Assert.Equal([1, 2, 3], repository.Distinct<int>("age", x => true).Order());
	}
	
	[Fact]
	public async Task DynamicFindOneAndDeleteMany_Overloads_Work()
	{
		var repository = await this.SeedDynamicRepositoryAsync();
		
		Assert.NotNull(repository.FindOne(x => true));
		Assert.NotNull(await repository.FindOneAsync(x => true, CancellationToken));
		Assert.True(await repository.DeleteManyAsync(x => true, CancellationToken));
		Assert.True(repository.DeleteMany(x => true));
		Assert.Equal(0, repository.Count());
	}
	
	[Fact]
	public async Task DynamicUpdate_WithAnExpandoObject_ReplacesTheDocument()
	{
		var repository = await this.SeedDynamicRepositoryAsync();
		var id = repository.DocumentCollection.Find(new BsonDocument("name", "a")).Single(CancellationToken)["_id"].AsObjectId;
		dynamic expando = new System.Dynamic.ExpandoObject();
		expando._id = id;
		expando.name = "x";
		dynamic syncExpando = new System.Dynamic.ExpandoObject();
		syncExpando._id = id;
		syncExpando.name = "y";
		
		await repository.UpdateAsync(expando, id.ToString(), cancellationToken: CancellationToken);
		Assert.Equal("x", (string) (await repository.FindOneAsync(id.ToString(), CancellationToken))!.name);
		repository.Update(syncExpando, id.ToString());
		repository.Upsert(new BsonDocument("name", "z"));
		
		Assert.Equal("y", (string) repository.FindOne(id.ToString())!.name);
		Assert.Equal(4, repository.Count());
	}
	
	#endregion
}
