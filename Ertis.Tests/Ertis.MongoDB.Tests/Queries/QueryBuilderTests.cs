using System;
using System.Collections.Generic;
using Ertis.MongoDB.Queries;
using Ertis.Net.Http;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Queries
{
	public class QueryBuilderTests
	{
		#region Initialize

		[SetUp]
		public void Setup()
		{
			
		}

		#endregion

		#region Methods
		
		[Test]
		public void QueryStringTest()
		{
			var queryString = QueryString.Empty;
			queryString = queryString.Add("skip", 0);
			Assert.Pass();
		}
		
		[Test]
		public void QueryValueWithStringTest()
		{
			var queryValue = new QueryValue("value");
			Assert.AreEqual("\"value\"", queryValue.ToString());
		}
		
		[Test]
		public void QueryValueWithIntegerTest()
		{
			var queryValue = new QueryValue(9716);
			Assert.AreEqual("9716", queryValue.ToString());
		}

		[Test]
		public void QueryTest()
		{
			var query = new Query("key", new QueryValue("value"));
			Assert.AreEqual("{ \"key\": \"value\" }", query.ToString());
		}
		
		[Test]
		public void QueryGroupTest()
		{
			var queryGroup = new QueryGroup
			{
				{ "_id", "xyz" },
				{ "role", "admin" }
			};
			
			Assert.AreEqual("{ \"_id\": \"xyz\", \"role\": \"admin\" }", queryGroup.ToString());
		}
		
		[Test]
		public void QueryArrayTest()
		{
			var queryArray = new QueryArray { 1, 2, 3 };
			Assert.AreEqual("[ 1, 2, 3 ]", queryArray.ToString());
		}
		
		[Test]
		public void WhereTest1()
		{
			var whereQuery = QueryBuilder.Where("role", "admin");
			
			Assert.AreEqual("{ \"where\": { \"role\": \"admin\" } }", whereQuery.ToString());
		}
		
		[Test]
		public void WhereTest2()
		{
			var whereQuery = QueryBuilder.Where(new QueryGroup
			{
				{ "_id", "xyz" },
				{ "role", "admin" }
			});
			
			Assert.AreEqual("{ \"where\": { \"_id\": \"xyz\", \"role\": \"admin\" } }", whereQuery.ToString());
		}
		
		[Test]
		public void WhereTest3()
		{
			var now = DateTime.Now;
			var whereQuery = QueryBuilder.Where(QueryBuilder.GreaterThanOrEqual("sys.created_at", now));
			
			Assert.AreEqual("{ \"where\": { \"sys.created_at\": { \"$gte\": \"" + now + "\" } } }", whereQuery.ToString());
		}

		[Test]
		public void ContainsTest()
		{
			var containsQuery = QueryBuilder.Contains("numbers", new QueryArray { 1, 2, 3 });
			Assert.AreEqual("{ \"numbers\": { \"$in\": [ 1, 2, 3 ] } }", containsQuery.ToString());
		}

		[Test]
		public void NotContainsTest()
		{
			var ninQuery = QueryBuilder.NotContains("letters", new QueryArray { 'x', 'y', 'z' });
			Assert.AreEqual("{ \"letters\": { \"$nin\": [ \"x\", \"y\", \"z\" ] } }", ninQuery.ToString());
		}

		[Test]
		public void AndTest()
		{
			var andQuery = QueryBuilder.And(new QueryArray
			{
				QueryBuilder.Or(new QueryArray
				{
					QueryBuilder.LessThan("qty", 10),
					QueryBuilder.GreaterThan("qty", 50),
				}),
				QueryBuilder.Or(new QueryArray
				{
					new Query("sale", true),
					QueryBuilder.GreaterThanOrEqual("price", 5),
				})
			});
			
			Assert.AreEqual("{ \"$and\": [ { \"$or\": [ { \"qty\": { \"$lt\": 10 } }, { \"qty\": { \"$gt\": 50 } } ] }, { \"$or\": [ { \"sale\": true }, { \"price\": { \"$gte\": 5 } } ] } ] }", andQuery.ToString());
		}
		
		[Test]
		public void NotTest()
		{
			var notQuery = QueryBuilder.Not(QueryBuilder.GreaterThan("price", 1.99d));
			Assert.AreEqual("{ \"price\": { \"$not\": { \"$gt\": 1.99 } } }", notQuery.ToString());
		}
		
		[Test]
		public void SelectTest()
		{
			var selectQuery = QueryBuilder.Select(new Dictionary<string, bool>
			{
				{ "_id", true },
				{ "title", true },
				{ "description", false }
			});
			
			Assert.AreEqual("{ \"select\": { \"_id\": true, \"title\": true, \"description\": false } }", selectQuery.ToString());
		}
		
		[Test]
		public void SearchTest()
		{
			var searchQuery1 = QueryBuilder.Search("Ertuğrul");
			var searchQuery2 = QueryBuilder.Search("Ertuğrul", "tr");
			var searchQuery3 = QueryBuilder.Search("Ertuğrul", "tr", true);
			var searchQuery4 = QueryBuilder.Search("Ertuğrul", "tr", true, false);
			
			Assert.AreEqual("{ \"$text\": { \"$search\": \"Ertuğrul\" } }", searchQuery1.ToString());
			Assert.AreEqual("{ \"$text\": { \"$search\": \"Ertuğrul\", \"$language\": \"tr\" } }", searchQuery2.ToString());
			Assert.AreEqual("{ \"$text\": { \"$search\": \"Ertuğrul\", \"$language\": \"tr\", \"$caseSensitive\": true } }", searchQuery3.ToString());
			Assert.AreEqual("{ \"$text\": { \"$search\": \"Ertuğrul\", \"$language\": \"tr\", \"$caseSensitive\": true, \"$diacriticSensitive\": false } }", searchQuery4.ToString());
		}
		
		[Test]
		public void CombineTest()
		{
			var whereQuery = QueryBuilder.Where(new QueryGroup
			{
				{ "_id", "xyz" },
				{ "role", "admin" }
			});
			
			var selectQuery = QueryBuilder.Select(new Dictionary<string, bool>
			{
				{ "_id", true },
				{ "title", true },
				{ "description", false }
			});

			var combinedQuery = QueryBuilder.Combine(whereQuery, selectQuery);
			Assert.AreEqual("{ \"where\": { \"_id\": \"xyz\", \"role\": \"admin\" }, \"select\": { \"_id\": true, \"title\": true, \"description\": false } }", combinedQuery.ToString());
		}

		#endregion
	}
}