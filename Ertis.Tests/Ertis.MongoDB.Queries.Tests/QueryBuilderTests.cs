using System.Collections.Generic;
using Ertis.MongoDB.Queries;
using NUnit.Framework;

// ReSharper disable StringLiteralTypo
namespace Ertis.Tests.Ertis.MongoDB.Queries.Tests
{
    public class QueryBuilderTests
    {
        #region ObjectId Tests

        [Test]
        public void ObjectIdTest1()
        {
            const string membershipId = "61b7bfa7fa20a74224879e13";
            const string id = "61fdb0eb923408d9b5d7f14a";
            var query = QueryBuilder.Where(QueryBuilder.Equals("membership_id", membershipId), QueryBuilder.Equals("_id", QueryBuilder.ObjectId(id)));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"membership_id\": \"61b7bfa7fa20a74224879e13\", \"_id\": ObjectId(\"61fdb0eb923408d9b5d7f14a\") }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void ObjectIdTest2()
        {
            const string organizationId = "6356f3240e37638afd92c516";
            const string url = "/foo/bar";
            var query = QueryBuilder.Where(QueryBuilder.Equals("organization_id", organizationId), QueryBuilder.Equals("url", url));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"organization_id\": \"6356f3240e37638afd92c516\", \"url\": \"/foo/bar\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void ObjectIdTest3()
        {
            const string organizationId = "6356f3240e37638afd92c516";
            const string id = "6439f2d9e41d91644e8b8126";
            const string url = "/foo/bar";
            var query = QueryBuilder.Where(QueryBuilder.Equals("organization_id", organizationId), QueryBuilder.Equals("url", url), QueryBuilder.NotEquals("_id", QueryBuilder.ObjectId(id)));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"organization_id\": \"6356f3240e37638afd92c516\", \"url\": \"/foo/bar\", \"_id\": { $ne: ObjectId(\"6439f2d9e41d91644e8b8126\") } }".Trim() == queryJson?.Trim());
        }

        #endregion
        
        #region Basic Where Methods

        [Test]
        public void WhereTest1()
        {
            var query = QueryBuilder.Where("username", "ertugrul.ozcan");
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"username\": \"ertugrul.ozcan\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void WhereTest2()
        {
            var query = QueryBuilder.Where(QueryBuilder.Equals("username", "ertugrul.ozcan"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"username\": \"ertugrul.ozcan\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void WhereTest3()
        {
            var query = QueryBuilder.Where(QueryBuilder.Equals("first_name", "Ertuğrul"), QueryBuilder.Equals("last_name", "Özcan"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"first_name\": \"Ertuğrul\", \"last_name\": \"Özcan\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void WhereTest4()
        {
            IEnumerable<IQuery> queries = new []
            {
                QueryBuilder.Equals("first_name", "Ertuğrul"),
                QueryBuilder.Equals("last_name", "Özcan")
            };
            
            var query = QueryBuilder.Where(queries);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"first_name\": \"Ertuğrul\", \"last_name\": \"Özcan\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void WhereTest5()
        {
            var query = QueryBuilder.Where(QueryBuilder.And(QueryBuilder.Equals("first_name", "Ertuğrul"), QueryBuilder.Equals("last_name", "Özcan")));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"first_name\": \"Ertuğrul\", \"last_name\": \"Özcan\" }".Trim() == queryJson?.Trim()); // { $and: [ { "first_name": "Ertuğrul" }, { "last_name": "Özcan" } ] }
        }
        
        [Test]
        public void WhereTest6()
        {
            IQuery[] queries =
            {
                QueryBuilder.Equals("first_name", "Ertuğrul"),
                QueryBuilder.Equals("last_name", "Özcan")
            };
            
            var query = QueryBuilder.Where(QueryBuilder.And(queries));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"first_name\": \"Ertuğrul\", \"last_name\": \"Özcan\" }".Trim() == queryJson?.Trim()); // { $and: [ { "first_name": "Ertuğrul" }, { "last_name": "Özcan" } ] }
        }
        
        [Test]
        public void WhereTest7()
        {
            IQuery[] queries =
            {
                QueryBuilder.Equals("first_name", "Ertuğrul"),
                QueryBuilder.Equals("last_name", "Özcan")
            };
            
            var query = QueryBuilder.Where(QueryBuilder.Or(queries));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $or: [ { \"first_name\": \"Ertuğrul\" }, { \"last_name\": \"Özcan\" } ] }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void WhereTest8()
        {
            var startTime = System.DateTime.Now;
            var endTime = System.DateTime.Now.AddDays(1);
            
            IQuery[] queries =
            {
                QueryBuilder.GreaterThanOrEqual(startTime),
                QueryBuilder.LessThanOrEqual(endTime)
            };
            
            var query = QueryBuilder.Where("event_time", queries);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"event_time\": { $gte: \"" + startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\", $lte: \"" + endTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\" } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void WhereTest9()
        {
            var startTime = System.DateTime.Now;
            var endTime = System.DateTime.Now.AddDays(1);
            
            IQuery[] eventTimeQueries =
            {
                QueryBuilder.GreaterThanOrEqual(startTime),
                QueryBuilder.LessThanOrEqual(endTime)
            };
            
            var eventTimeQuery = QueryBuilder.Where("event_time", eventTimeQueries);
            var eventTypeQuery = QueryBuilder.Equals("event_type", "TokenVerified");
            
            var query = QueryBuilder.Where(eventTypeQuery, eventTimeQuery);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"event_type\": \"TokenVerified\", \"event_time\": { $gte: \"" + startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\", $lte: \"" + endTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "\" } }".Trim() == queryJson?.Trim());
        }

        #endregion

        #region Projection Tests

        [Test]
        public void SelectTest()
        {
            var query = QueryBuilder.Select(new Dictionary<string, bool>
            {
                { "first_name", false },
                { "last_name", true },
            });
            
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"select\": { \"first_name\": 0, \"last_name\": 1 } }".Trim() == queryJson?.Trim());
        }

        #endregion
        
        #region Combine Queries Tests

        [Test]
        public void CombineTest()
        {
            IEnumerable<IQuery> whereQueries = new []
            {
                QueryBuilder.Equals("first_name", "Ertuğrul"),
                QueryBuilder.Equals("last_name", "Özcan")
            };
            
            var whereQuery = QueryBuilder.WhereOut(whereQueries);
            var whereQueryJson = whereQuery.ToString();
            Assert.That(whereQueryJson != null);
            Assert.That("{ \"where\": { \"first_name\": \"Ertuğrul\", \"last_name\": \"Özcan\" } }".Trim() == whereQueryJson?.Trim());
            
            var selectQuery = QueryBuilder.Select(new Dictionary<string, bool>
            {
                { "first_name", false },
                { "last_name", true },
            });
            
            var selectQueryJson = selectQuery.ToString();
            Assert.That(selectQueryJson != null);
            Assert.That("{ \"select\": { \"first_name\": 0, \"last_name\": 1 } }".Trim() == selectQueryJson?.Trim());

            var combinedQuery = QueryBuilder.Combine(whereQuery, selectQuery);
            var combinedQueryJson = combinedQuery.ToString();
            Assert.That(combinedQueryJson != null);
            Assert.That("{ \"where\": { \"first_name\": \"Ertuğrul\", \"last_name\": \"Özcan\" }, \"select\": { \"first_name\": 0, \"last_name\": 1 } }".Trim() == combinedQueryJson?.Trim());
        }

        [Test]
        public void NotNullTest()
        {
            var query = QueryBuilder.Combine(QueryBuilder.NotEquals<string>("tvSeriesId", null), QueryBuilder.Equals("sys.state", "published"));
            Assert.That("{ \"tvSeriesId\": { $ne: null }, \"sys.state\": \"published\" }".Trim() == query.ToString()?.Trim());
        }

        #endregion

        #region Equals & NotEquals Tests

        [Test]
        public void EqualsTest()
        {
            var query = QueryBuilder.Equals("username", "ertugrul.ozcan");
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"username\": \"ertugrul.ozcan\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void EqualsTest2()
        {
            object value = "ertugrul.ozcan";
            var query = QueryBuilder.Equals("username", value);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"username\": \"ertugrul.ozcan\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void NotEqualsTest()
        {
            var query = QueryBuilder.NotEquals("username", "ertugrul.ozcan");
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"username\": { $ne: \"ertugrul.ozcan\" } }".Trim() == queryJson?.Trim());
        }

        #endregion

        #region GreaterThan & LessThan Tests

        [Test]
        public void GreaterThanTest()
        {
            var query = QueryBuilder.GreaterThan("age", 34);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $gt: 34 } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void GreaterThanOrEqualTest()
        {
            var query = QueryBuilder.GreaterThanOrEqual("age", 34);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $gte: 34 } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void LessThanTest()
        {
            var query = QueryBuilder.LessThan("age", 34);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $lt: 34 } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void LessThanOrEqualTest()
        {
            var query = QueryBuilder.LessThanOrEqual("age", 34);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $lte: 34 } }".Trim() == queryJson?.Trim());
        }

        #endregion

        #region Contains & NotContains Tests

        [Test]
        public void ContainsTest()
        {
            var query = QueryBuilder.Contains("name", new [] { "ahmet", "ertugrul" });
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"name\": { $in: [ \"ahmet\", \"ertugrul\" ] } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void NotContainsTest()
        {
            var query = QueryBuilder.NotContains("age", new [] { 7, 13, 48 });
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $nin: [ 7, 13, 48 ] } }".Trim() == queryJson?.Trim());
        }

        #endregion
        
        #region Logical Queries Tests
        
        [Test]
        public void AndTest1()
        {
            var query = QueryBuilder.And(QueryBuilder.GreaterThanOrEqual("age", 30), QueryBuilder.Equals("gender", "female"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $gte: 30 }, \"gender\": \"female\" }".Trim() == queryJson?.Trim()); // { $and: [ { "age": { $gte: 30 } }, { "gender": "female" } ] }
        }
        
        [Test]
        public void AndTest2()
        {
            IQuery[] expressions = 
            {
                QueryBuilder.GreaterThanOrEqual("age", 30),
                QueryBuilder.Equals("gender", "female")
            };
            
            var query = QueryBuilder.And(expressions);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $gte: 30 }, \"gender\": \"female\" }".Trim() == queryJson?.Trim()); // { $and: [ { "age": { $gte: 30 } }, { "gender": "female" } ] }
        }
        
        [Test]
        public void AndTest3()
        {
            var query = QueryBuilder.And(QueryBuilder.NotEquals("_id", QueryBuilder.ObjectId("66c8c0a9a54a186fb2e0515b")), QueryBuilder.Equals("categories.slug", "futbol"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"_id\": { $ne: ObjectId(\"66c8c0a9a54a186fb2e0515b\") }, \"categories.slug\": \"futbol\" }".Trim() == queryJson?.Trim()); // { $and: [ { "_id": { $ne: ObjectId("66c8c0a9a54a186fb2e0515b") } }, { "categories.slug": "futbol" } ] }
        }
        
        [Test]
        public void AndTest4()
        {
            var query = QueryBuilder.And(QueryBuilder.NotEquals("_id", QueryBuilder.ObjectId("66c8c0a9a54a186fb2e0515b")), QueryBuilder.Equals("categories.slug", "futbol"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"_id\": { $ne: ObjectId(\"66c8c0a9a54a186fb2e0515b\") }, \"categories.slug\": \"futbol\" }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void OrTest1()
        {
            var query = QueryBuilder.Or(QueryBuilder.GreaterThanOrEqual("age", 30), QueryBuilder.Equals("gender", "female"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $or: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void OrTest2()
        {
            IQuery[] expressions =
            {
                QueryBuilder.GreaterThanOrEqual("age", 30),
                QueryBuilder.Equals("gender", "female")
            };
            
            var query = QueryBuilder.Or(expressions);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $or: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void NorTest1()
        {
            var query = QueryBuilder.Nor(QueryBuilder.GreaterThanOrEqual("age", 30), QueryBuilder.Equals("gender", "female"));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $nor: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void NorTest2()
        {
            IQuery[] expressions =
            {
                QueryBuilder.GreaterThanOrEqual("age", 30),
                QueryBuilder.Equals("gender", "female")
            };
            
            var query = QueryBuilder.Nor(expressions);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $nor: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void NotTest1()
        {
            var query = QueryBuilder.Not("age", 1.99);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $not: { $eq: 1.99 } } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void NotTest2()
        {
            var query = QueryBuilder.Not(QueryBuilder.GreaterThanOrEqual("age", 30));
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"age\": { $not: { $gte: 30 } } }".Trim() == queryJson?.Trim());
        }
        
        #endregion
        
        #region Element Queries Tests
        
        [Test]
        public void ExistTest()
        {
            var query = QueryBuilder.Exist("qty", true);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"qty\": { $exists: true } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void TypeOfTest()
        {
            var query = QueryBuilder.TypeOf("qty", BsonType.MaxKey);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"qty\": { $type: \"maxKey\" } }".Trim() == queryJson?.Trim());
        }
        
        #endregion
        
        #region Evaluation Queries

        [Test]
        public void RegexTest()
        {
            var query = QueryBuilder.Regex("name", "/acme.*corp/", RegexOptions.AllowDot | RegexOptions.CaseInsensitivity);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ \"name\": { $regex: \"acme.*corp\", $options: \"si\" } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void FullTextSearchTest1()
        {
            var query = QueryBuilder.FullTextSearch("Test Keyword", "tr", true, true);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $text: { $search: \"Test Keyword\", $language: \"tr\", $caseSensitive: true, $diacriticSensitive: true } }".Trim() == queryJson?.Trim());
        }
        
        [Test]
        public void FullTextSearchTest2()
        {
            var query = QueryBuilder.FullTextSearch("Test Keyword", isCaseSensitive: true);
            var queryJson = query.ToString();
            Assert.That(queryJson != null);
            Assert.That("{ $text: { $search: \"Test Keyword\", $caseSensitive: true } }".Trim() == queryJson?.Trim());
        }

        #endregion
    }
}