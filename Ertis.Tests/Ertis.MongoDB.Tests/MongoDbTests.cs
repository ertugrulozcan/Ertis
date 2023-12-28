using System;
using System.Collections.Generic;
using Ertis.MongoDB.Configuration;
using Ertis.Tests.Ertis.MongoDB.Tests.Configuration;
using Ertis.Tests.Ertis.MongoDB.Tests.Models;
using Ertis.Tests.Ertis.MongoDB.Tests.Services;
using Ertis.Tests.Ertis.MongoDB.Tests.Services.Interfaces;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Tests
{
	public class MongoDbTests
	{
		#region Services

		private IMockDatabaseRepository mockDatabaseRepository;

		#endregion
		
		#region Initialize

		[SetUp]
		public void Setup()
		{
			IDatabaseSettings databaseSettings = new MockDatabaseSettings
			{
				DefaultAuthDatabase = "test_db"
			};
			
			this.mockDatabaseRepository = new MockDatabaseRepository(databaseSettings);
		}

		#endregion

		#region Methods

		[Test]
		public void FindTest()
		{
			var paginationCollection = this.mockDatabaseRepository.Find(null, null, null, null, null);
			foreach (var item in paginationCollection.Items)
			{
				Console.WriteLine($"{item.Id} - {item.Text}");
			}
			
			Assert.Pass();
		}
		
		[Test]
		public void FindByIdTest()
		{
			const string id = "5fc65c27dd7283e0b912f660";
			var item = this.mockDatabaseRepository.FindOne(id);
			Console.WriteLine($"{item.Id} - {item.Text}");
			
			Assert.Pass();
		}
		
		[Test]
		public void InsertTest()
		{
			var item = this.mockDatabaseRepository.Insert(TestModel.GenerateRandom(1, 3));
			Console.WriteLine($"{item.Id} - {item.Text} inserted.");
			
			Assert.Pass();
		}
		
		[Test]
		public void BulkInsertTest()
		{
			var mockList= TestModel.GenerateRandom(1000000);
			foreach (var item in mockList)
			{
				this.mockDatabaseRepository.Insert(item);
				Console.WriteLine($"{item.Id} - {item.Text} inserted.");	
			}

			Assert.Pass();
		}
		
		[Test]
		public void UpdateTest()
		{
			const string id = "5fc65c27dd7283e0b912f660";
			var item = this.mockDatabaseRepository.FindOne(id);
			item.Text += " (new)";
			var updatedItem = this.mockDatabaseRepository.Update(item);
			
			Console.WriteLine($"{updatedItem.Id} - {updatedItem.Text} updated.");
			
			Assert.Pass();
		}
		
		[Test]
		public void DeleteTest()
		{
			const string id = "5fc65c27dd7283e0b912f660";
			bool isDeleted = this.mockDatabaseRepository.Delete(id);

			if (isDeleted)
			{
				Console.WriteLine($"{id} - entity deleted.");	
				Assert.Pass();
			}
			else
			{
				Console.WriteLine($"{id} - entity could not deleted!");
				Assert.Fail();
			}
		}
		
		[Test]
		public void QueryTest()
		{
			try
			{
				const string query = "{'int_field': {'$gte': 10}}";
				Dictionary<string, bool> selectFields = new Dictionary<string, bool>
				{
					{ "_id", true },
					{ "array_field", true },
					{ "array_field.string_field", true }
				};
				
				var paginationResult = this.mockDatabaseRepository.Query(query, null, null, null, null, null, selectFields:selectFields);
				string json = JsonConvert.SerializeObject(paginationResult);
				Console.WriteLine(json);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}

			Assert.Pass();
		}
		
		[Test]
		public void QueryTest2()
		{
			try
			{
				string query = "{ \"sys.created_at\": { \"$gte\": \"2021-01-24T00:00:00.000Z\" } }";

				var paginationResult = this.mockDatabaseRepository.Query(query, null, null, null, null, null, null);
				string json = JsonConvert.SerializeObject(paginationResult);
				Console.WriteLine(json);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}

			Assert.Pass();
		}
		
		[Test]
		public void CountTest()
		{
			long count = this.mockDatabaseRepository.Count();
			Console.WriteLine($"Total count: {count}");
			
			Assert.Pass();
		}
		
		[Test]
		public void CountByQueryTest()
		{
			const string query = "{'int_field': {'$gte': 15}}";
			long count = this.mockDatabaseRepository.Count(query);
			Console.WriteLine($"Total count: {count}");
			
			Assert.Pass();
		}
		
		[Test]
		public void CountByExpressionTest()
		{
			long count = this.mockDatabaseRepository.Count(x => x.Integer >= 15);
			Console.WriteLine($"Total count: {count}");
			
			Assert.Pass();
		}

		#endregion
	}
}