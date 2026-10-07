using Ertis.Data.Repository;
using Ertis.MongoDB.Client;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Repository;

namespace Ertis.MongoDB.Tests.TestHelpers;

public sealed class TestRepository(IMongoClientProvider clientProvider, IDatabaseSettings settings, string collectionName = "entities", IRepositoryActionBinder? actionBinder = null)
	: MongoRepositoryBase<TestEntity>(clientProvider, settings, collectionName, actionBinder);

public sealed class TestDynamicRepository(IMongoClientProvider clientProvider, IDatabaseSettings settings, string collectionName = "documents", IRepositoryActionBinder? actionBinder = null)
	: DynamicMongoRepository(clientProvider, settings, collectionName, actionBinder)
{
	#region Properties
	
	public new global::MongoDB.Driver.IMongoCollection<global::MongoDB.Bson.BsonDocument> DocumentCollection => base.DocumentCollection;
	
	#endregion
}

/// <summary>
/// Records the entities passed to the action binder and returns the replacement given for them
/// </summary>
public sealed class RecordingActionBinder : IRepositoryActionBinder
{
	#region Properties
	
	public List<(string Action, object? Entity)> Calls { get; } = [];
	
	public Func<object?, object?>? BeforeInsertReplacement { get; init; }
	
	#endregion
	
	#region Methods
	
	public TEntity BeforeInsert<TEntity>(TEntity entity)
	{
		this.Calls.Add((nameof(this.BeforeInsert), entity));
		return this.BeforeInsertReplacement != null ? (TEntity) this.BeforeInsertReplacement(entity)! : entity;
	}
	
	public TEntity AfterInsert<TEntity>(TEntity entity)
	{
		this.Calls.Add((nameof(this.AfterInsert), entity));
		return entity;
	}
	
	public TEntity BeforeUpdate<TEntity>(TEntity entity)
	{
		this.Calls.Add((nameof(this.BeforeUpdate), entity));
		return entity;
	}
	
	public TEntity AfterUpdate<TEntity>(TEntity entity)
	{
		this.Calls.Add((nameof(this.AfterUpdate), entity));
		return entity;
	}
	
	#endregion
}
