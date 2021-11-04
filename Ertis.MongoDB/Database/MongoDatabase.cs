using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Helpers;
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
		/// <param name="settings"></param>
		public MongoDatabase(IDatabaseSettings settings)
		{
			string connectionString = ConnectionStringHelper.GenerateConnectionString(settings);
			var client = new MongoClient(connectionString);
			this.Database = client.GetDatabase(settings.DefaultAuthDatabase);
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
			var resultDocument = await this.GetDatabaseStatisticsDocumentAsync(cancellationToken);
			var json = resultDocument.ToJson(new JsonWriterSettings
			{
				OutputMode = JsonOutputMode.RelaxedExtendedJson
			});
			
			return Newtonsoft.Json.JsonConvert.DeserializeObject<MongoDbStatistics>(json);
		}

		public BsonDocument GetDatabaseStatisticsDocument()
		{
			var command = new BsonDocument { { "dbstats", 1 } };
			return this.Database.RunCommand<BsonDocument>(command);
		}

		public async ValueTask<BsonDocument> GetDatabaseStatisticsDocumentAsync(CancellationToken cancellationToken = default)
		{
			var command = new BsonDocument { { "dbstats", 1 } };
			return await this.Database.RunCommandAsync<BsonDocument>(command, cancellationToken:cancellationToken);
		}

		#endregion
	}
}