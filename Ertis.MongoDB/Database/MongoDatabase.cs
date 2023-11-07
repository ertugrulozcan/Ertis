using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.MongoDB.Client;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDriver = MongoDB.Driver;

namespace Ertis.MongoDB.Database
{
	public class MongoDatabase : IMongoDatabase
	{
		#region Properties

		private MongoDriver.IMongoDatabase Database { get; }

		#endregion	
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="clientProvider"></param>
		/// <param name="settings"></param>
		public MongoDatabase(IMongoClientProvider clientProvider, IDatabaseSettings settings)
		{
			this.Database = clientProvider.Client.GetDatabase(settings.DefaultAuthDatabase);
		}

		#endregion

		#region Methods

		public void CreateCollection(string name)
		{
			this.Database.CreateCollection(name);
		}
		
		public async ValueTask CreateCollectionAsync(string name, CancellationToken cancellationToken = default)
		{
			await this.Database.CreateCollectionAsync(name, cancellationToken: cancellationToken);
		}
		
		public void DropCollection(string name)
		{
			this.Database.DropCollection(name);
		}
		
		public async ValueTask DropCollectionAsync(string name, CancellationToken cancellationToken = default)
		{
			await this.Database.DropCollectionAsync(name, cancellationToken: cancellationToken);
		}
		
		public void RenameCollection(string oldName, string newName)
		{
			this.Database.RenameCollection(oldName, newName);
		}
		
		public async ValueTask RenameCollectionAsync(string oldName, string newName, CancellationToken cancellationToken = default)
		{
			await this.Database.RenameCollectionAsync(oldName, newName, cancellationToken: cancellationToken);
		}
		
		public IEnumerable<string> ListCollections(Expression<Func<BsonDocument, bool>> filterExpression = null)
		{
			if (filterExpression != null)
			{
				var result = this.Database.ListCollectionNames(new ListCollectionNamesOptions
				{
					Filter = new ExpressionFilterDefinition<BsonDocument>(filterExpression)
				});

				var collectionNames = new List<string>();
				result.ForEachAsync(collectionName => collectionNames.Add(collectionName));
				return collectionNames;
			}
			else
			{
				var result = this.Database.ListCollectionNames();
				var collectionNames = new List<string>();
				result.ForEachAsync(collectionName => collectionNames.Add(collectionName));
				return collectionNames;
			}
		}
		
		public async ValueTask<IEnumerable<string>> ListCollectionsAsync( 
			Expression<Func<BsonDocument, bool>> filterExpression = null, 
			CancellationToken cancellationToken = default)
		{
			if (filterExpression != null)
			{
				var result = await this.Database.ListCollectionNamesAsync(new ListCollectionNamesOptions
				{
					Filter = new ExpressionFilterDefinition<BsonDocument>(filterExpression)
				}, cancellationToken: cancellationToken);

				var collectionNames = new List<string>();
				await result.ForEachAsync(collectionName => collectionNames.Add(collectionName), cancellationToken: cancellationToken);
				return collectionNames;
			}
			else
			{
				var result = await this.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken);
				var collectionNames = new List<string>();
				await result.ForEachAsync(collectionName => collectionNames.Add(collectionName), cancellationToken: cancellationToken);
				return collectionNames;
			}
		}
		
		public MongoDbStatistics GetDatabaseStatistics()
		{
			var resultDocument = this.GetDatabaseStatisticsDocument();
			var json = resultDocument.ToJson(new JsonWriterSettings
			{
				OutputMode = JsonOutputMode.RelaxedExtendedJson
			});
			
			return Newtonsoft.Json.JsonConvert.DeserializeObject<MongoDbStatistics>(json);
		}
		
		public async ValueTask<MongoDbStatistics> GetDatabaseStatisticsAsync(CancellationToken cancellationToken = default)
		{
			var resultDocument = await this.GetDatabaseStatisticsDocumentAsync(cancellationToken: cancellationToken);
			var json = resultDocument.ToJson(new JsonWriterSettings
			{
				OutputMode = JsonOutputMode.RelaxedExtendedJson
			});
			
			return Newtonsoft.Json.JsonConvert.DeserializeObject<MongoDbStatistics>(json);
		}

		public BsonDocument GetDatabaseStatisticsDocument()
		{
			// ReSharper disable once StringLiteralTypo
			var command = new BsonDocument { { "dbstats", 1 } };
			return this.Database.RunCommand<BsonDocument>(command);
		}

		public async ValueTask<BsonDocument> GetDatabaseStatisticsDocumentAsync(CancellationToken cancellationToken = default)
		{
			// ReSharper disable once StringLiteralTypo
			var command = new BsonDocument { { "dbstats", 1 } };
			return await this.Database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
		}
		
		public async Task CopyOneAsync(string documentId, string sourceCollectionName, string destinationCollectionName)
		{
			var sourceCollection = this.Database.GetCollection<BsonDocument>(sourceCollectionName);
			if (sourceCollection == null)
			{
				throw new MongoException($"There is no collection named '{sourceCollectionName}'");
			}
			
			var destinationCollection = this.Database.GetCollection<BsonDocument>(destinationCollectionName);
			if (destinationCollection == null)
			{
				throw new MongoException($"There is no collection named '{destinationCollectionName}'");
			}
			
			using (var cursor = await sourceCollection.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId))).ToCursorAsync())
			{
				while (await cursor.MoveNextAsync())
				{
					var batch = cursor.Current;
					foreach (var document in batch)
					{
						await destinationCollection.BulkWriteAsync(new WriteModel<BsonDocument>[]
						{
							new InsertOneModel<BsonDocument>(document)
						});
					}
				}
			}
		}
		
		public async Task ReplaceOneAsync(string documentId, string sourceCollectionName, string destinationCollectionName)
		{
			var sourceCollection = this.Database.GetCollection<BsonDocument>(sourceCollectionName);
			if (sourceCollection == null)
			{
				throw new MongoException($"There is no collection named '{sourceCollectionName}'");
			}
			
			var destinationCollection = this.Database.GetCollection<BsonDocument>(destinationCollectionName);
			if (destinationCollection == null)
			{
				throw new MongoException($"There is no collection named '{destinationCollectionName}'");
			}
			
			using (var cursor = await sourceCollection.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId))).ToCursorAsync())
			{
				while (await cursor.MoveNextAsync())
				{
					var batch = cursor.Current;
					foreach (var document in batch)
					{
						await destinationCollection.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId)), document);
					}
				}
			}
		}
		
		public async Task CopyAllAsync(string sourceCollectionName, string destinationCollectionName)
		{
			var sourceCollection = this.Database.GetCollection<BsonDocument>(sourceCollectionName);
			if (sourceCollection == null)
			{
				throw new MongoException($"There is no collection named '{sourceCollectionName}'");
			}
			
			var destinationCollection = this.Database.GetCollection<BsonDocument>(destinationCollectionName);
			if (destinationCollection == null)
			{
				throw new MongoException($"There is no collection named '{destinationCollectionName}'");
			}
			
			using (var cursor = await sourceCollection.FindAsync(_ => true))
			{
				while (await cursor.MoveNextAsync())
				{
					var batch = cursor.Current;
					foreach (var document in batch)
					{
						await destinationCollection.BulkWriteAsync(new WriteModel<BsonDocument>[]
						{
							new InsertOneModel<BsonDocument>(document)
						});
					}
				}
			}
		}

		#endregion
	}
}