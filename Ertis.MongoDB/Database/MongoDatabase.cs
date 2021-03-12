using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.MongoDB.Configuration;
using Ertis.MongoDB.Helpers;
using Ertis.MongoDB.Models;
using MongoDB.Bson;
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
		protected MongoDatabase(IDatabaseSettings settings)
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
		
		public async Task CreateCollectionAsync(string name, CancellationToken cancellationToken = default)
		{
			await this.Database.CreateCollectionAsync(name, cancellationToken: cancellationToken);
		}
		
		public void DropCollection(string name)
		{
			this.Database.DropCollection(name);
		}
		
		public async Task DropCollectionAsync(string name, CancellationToken cancellationToken = default)
		{
			await this.Database.DropCollectionAsync(name, cancellationToken: cancellationToken);
		}
		
		public void RenameCollection(string oldName, string newName)
		{
			this.Database.RenameCollection(oldName, newName);
		}
		
		public async Task RenameCollectionAsync(string oldName, string newName, CancellationToken cancellationToken = default)
		{
			await this.Database.RenameCollectionAsync(oldName, newName, cancellationToken: cancellationToken);
		}
		
		public IEnumerable<string> ListCollections(string oldName, string newName, Expression<Func<BsonDocument, bool>> filterExpression = null)
		{
			if (filterExpression != null)
			{
				var result = this.Database.ListCollectionNames(new ListCollectionNamesOptions
				{
					Filter = new ExpressionFilterDefinition<BsonDocument>(filterExpression)
				});

				return result.Current;
			}
			else
			{
				var result = this.Database.ListCollectionNames();
				return result.Current;
			}
		}
		
		public async Task<IEnumerable<string>> ListCollectionsAsync(
			string oldName, 
			string newName, 
			Expression<Func<BsonDocument, bool>> filterExpression = null, 
			CancellationToken cancellationToken = default)
		{
			if (filterExpression != null)
			{
				var result = await this.Database.ListCollectionNamesAsync(new ListCollectionNamesOptions
				{
					Filter = new ExpressionFilterDefinition<BsonDocument>(filterExpression)
				}, cancellationToken: cancellationToken);

				return result.Current;
			}
			else
			{
				var result = await this.Database.ListCollectionNamesAsync(cancellationToken: cancellationToken);
				return result.Current;
			}
		}
		
		public MongoDbStatistics GetDatabaseStatistics()
		{
			var command = new BsonDocument { { "dbstats", 1 } };
			var resultDocument = this.Database.RunCommand<BsonDocument>(command);
			var json = resultDocument.ToString();
			return Newtonsoft.Json.JsonConvert.DeserializeObject<MongoDbStatistics>(json);
		}
		
		public async Task<MongoDbStatistics> GetDatabaseStatisticsAsync(CancellationToken cancellationToken = default)
		{
			var command = new BsonDocument { { "dbstats", 1 } };
			var resultDocument = await this.Database.RunCommandAsync<BsonDocument>(command, cancellationToken:cancellationToken);
			var json = resultDocument.ToString();
			return Newtonsoft.Json.JsonConvert.DeserializeObject<MongoDbStatistics>(json);
		}

		#endregion
	}
}