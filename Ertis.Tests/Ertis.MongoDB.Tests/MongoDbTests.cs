using System;
using System.Linq;
using Ertis.MongoDB.Configuration;
using Ertis.Tests.Ertis.MongoDB.Tests.Configuration;
using Ertis.Tests.Ertis.MongoDB.Tests.Models;
using Ertis.Tests.Ertis.MongoDB.Tests.Services;
using Ertis.Tests.Ertis.MongoDB.Tests.Services.Interfaces;
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
				Username = "",
				Password = "",
				Host = "172.17.0.2",
				Port = 27017,
				DatabaseName = "test_db"
			};
			
			this.mockDatabaseRepository = new MockDatabaseRepository(databaseSettings);
		}

		#endregion

		#region Methods

		[Test]
		public void FindTest()
		{
			var items = this.mockDatabaseRepository.Find();
			foreach (var item in items)
			{
				Console.WriteLine($"{item.Id} - {item.Text}");
			}
			
			Assert.Pass();
		}
		
		[Test]
		public void FindByIdTest()
		{
			const string id = "5fc65c27dd7283e0b912f660";
			var item = this.mockDatabaseRepository.Find(id);
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
			var mockList= TestModel.GenerateRandom(20);
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
			var item = this.mockDatabaseRepository.Find(id);
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
			const string query = "{'int_field': {'$gte': 10}}";
			var paginationResult = this.mockDatabaseRepository.Query(query);
			if (paginationResult.Items.Any())
			{
				foreach (var item in paginationResult.Items)
				{
					Console.WriteLine($"{item.Id} - {item.Text}");
				}	
			}
			else
			{
				Console.WriteLine("Collection is empty.");
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