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

		ValueTask CreateCollectionAsync(string name, CancellationToken cancellationToken = default);

		void DropCollection(string name);

		ValueTask DropCollectionAsync(string name, CancellationToken cancellationToken = default);

		void RenameCollection(string oldName, string newName);

		ValueTask RenameCollectionAsync(string oldName, string newName, CancellationToken cancellationToken = default);

		IEnumerable<string> ListCollections(Expression<Func<BsonDocument, bool>> filterExpression = null);

		ValueTask<IEnumerable<string>> ListCollectionsAsync(Expression<Func<BsonDocument, bool>> filterExpression = null, CancellationToken cancellationToken = default);

		MongoDbStatistics GetDatabaseStatistics();

		ValueTask<MongoDbStatistics> GetDatabaseStatisticsAsync(CancellationToken cancellationToken = default);
		
		BsonDocument GetDatabaseStatisticsDocument();

		ValueTask<BsonDocument> GetDatabaseStatisticsDocumentAsync(CancellationToken cancellationToken = default);

		Task CopyOneAsync(string documentId, string sourceCollectionName, string destinationCollectionName, bool overwriteIfExist = false);

		Task CopyAllAsync(string sourceCollectionName, string destinationCollectionName);
	}
}