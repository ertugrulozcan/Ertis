using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ertis.MongoDB.Models;
using MongoDB.Bson;

namespace Ertis.MongoDB.Database
{
	public interface IMongoDatabase
	{
		void CreateCollection(string name);

		Task CreateCollectionAsync(string name, CancellationToken cancellationToken = default);

		void DropCollection(string name);

		Task DropCollectionAsync(string name, CancellationToken cancellationToken = default);

		void RenameCollection(string oldName, string newName);

		Task RenameCollectionAsync(string oldName, string newName, CancellationToken cancellationToken = default);

		IEnumerable<string> ListCollections(Expression<Func<BsonDocument, bool>> filterExpression = null);

		Task<IEnumerable<string>> ListCollectionsAsync(Expression<Func<BsonDocument, bool>> filterExpression = null, CancellationToken cancellationToken = default);

		MongoDbStatistics GetDatabaseStatistics();

		Task<MongoDbStatistics> GetDatabaseStatisticsAsync(CancellationToken cancellationToken = default);
		
		BsonDocument GetDatabaseStatisticsDocument();

		Task<BsonDocument> GetDatabaseStatisticsDocumentAsync(CancellationToken cancellationToken = default);
	}
}