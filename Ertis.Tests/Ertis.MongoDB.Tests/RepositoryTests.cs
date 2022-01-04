using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ertis.MongoDB.Attributes;
using Ertis.Tests.Ertis.MongoDB.Tests.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Tests
{
    public class RepositoryTests
    {
        #region Initialize

        [SetUp]
        public void Setup()
        {
            
        }

        #endregion

        #region Methods
        
        [Test]
		public void CreateSearchIndexesTest()
		{
			var connectionString = "mongodb://localhost:27017/ertisauth?authSource=admin";
			var client = new MongoClient(connectionString);
			var database = client.GetDatabase("ertisauth");

			var collection = database.GetCollection<UserDto>("users");
			this.CreateSearchIndexesAsync(collection).ConfigureAwait(false).GetAwaiter().GetResult();
			
			Assert.Pass();
		}
		
		private async Task CreateSearchIndexesAsync<TEntity>(IMongoCollection<TEntity> collection)
		{
			try
			{
				var currentIndexesCursor = await collection.Indexes.ListAsync();
				var currentIndexes = await currentIndexesCursor.ToListAsync();
				var currentTextIndexes = currentIndexes.Where(x =>
					x.Contains("key") &&
					x["key"].IsBsonDocument &&
					x["key"].AsBsonDocument.Contains("_fts") &&
					x["key"].AsBsonDocument["_fts"].IsString &&
					x["key"].AsBsonDocument["_fts"].AsString == "text");

				var indexedPropertyNames = currentTextIndexes.SelectMany(x => x["weights"].AsBsonDocument.Names).ToArray();
				var nonIndexedPropertyNames = new List<string>();
			
				var propertyInfos = typeof(TEntity).GetProperties();
				foreach (var propertyInfo in propertyInfos)
				{
					var searchableAttribute = propertyInfo.GetCustomAttribute(typeof(SearchableAttribute), true);
					if (searchableAttribute is SearchableAttribute)
					{
						var attribute = propertyInfo.GetCustomAttribute(typeof(BsonElementAttribute), true);
						if (attribute is BsonElementAttribute bsonElementAttribute)
						{
							if (!indexedPropertyNames.Contains(bsonElementAttribute.ElementName))
							{
								nonIndexedPropertyNames.Add(bsonElementAttribute.ElementName);
							}
						}
					}
				}

				var combinedTextIndexDefinition = Builders<TEntity>.IndexKeys.Combine(
					nonIndexedPropertyNames.Select(x => Builders<TEntity>.IndexKeys.Text(x)));
				
				await collection.Indexes.CreateOneAsync(new CreateIndexModel<TEntity>(combinedTextIndexDefinition));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occured while creating search indexes for '{typeof(TEntity).Name}' entity type;");
				Console.WriteLine(ex);
			}
		}
        
        #endregion
    }
}