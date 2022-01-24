using Ertis.MongoDB.Queries;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Queries.Tests
{
    public class QueryBuilderTests
    {
        #region Basic Where Methods

        [Test]
        public void WhereTest1()
        {
            QueryBuilder.Where("username", "ertugrul.ozcan");
            Assert.Pass();
        }
        
        [Test]
        public void WhereTest2()
        {
            QueryBuilder.Where(QueryBuilder.Equals("username", "ertugrul.ozcan"));
            Assert.Pass();
        }
        
        [Test]
        public void WhereTest3()
        {
            QueryBuilder.Where(QueryBuilder.Equals("first_name", "Ertuğrul"), QueryBuilder.Equals("last_name", "Özcan"));
            Assert.Pass();
        }
        
        [Test]
        public void WhereTest4()
        {
            var queries = new[]
            {
                QueryBuilder.Equals("first_name", "Ertuğrul"),
                QueryBuilder.Equals("last_name", "Özcan")
            };
            
            QueryBuilder.Where(queries);
            
            Assert.Pass();
        }
        
        [Test]
        public void WhereTest5()
        {
            QueryBuilder.Where(QueryBuilder.And(QueryBuilder.Equals("first_name", "Ertuğrul"), QueryBuilder.Equals("last_name", "Özcan")));
            Assert.Pass();
        }
        
        [Test]
        public void WhereTest6()
        {
            QueryBuilder.Where(QueryBuilder.And(new []
            {
                QueryBuilder.Equals("first_name", "Ertuğrul"), 
                QueryBuilder.Equals("last_name", "Özcan")
            }));
            
            Assert.Pass();
        }

        #endregion

        #region Equals & NotEquals Tests

        [Test]
        public void EqualsTest()
        {
            var query = QueryBuilder.Equals("username", "ertugrul.ozcan");
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"username\": \"ertugrul.ozcan\" }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void NotEqualsTest()
        {
            var query = QueryBuilder.NotEquals("username", "ertugrul.ozcan");
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"username\": { $ne: \"ertugrul.ozcan\" } }".Trim(), queryJson.Trim());
        }

        #endregion

        #region GreaterThan & LessThan Tests

        [Test]
        public void GreaterThanTest()
        {
            var query = QueryBuilder.GreaterThan("age", 34);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $gt: 34 } }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void GreaterThanOrEqualTest()
        {
            var query = QueryBuilder.GreaterThanOrEqual("age", 34);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $gte: 34 } }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void LessThanTest()
        {
            var query = QueryBuilder.LessThan("age", 34);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $lt: 34 } }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void LessThanOrEqualTest()
        {
            var query = QueryBuilder.LessThanOrEqual("age", 34);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $lte: 34 } }".Trim(), queryJson.Trim());
        }

        #endregion

        #region Contains & NotContains Tests

        [Test]
        public void ContainsTest()
        {
            var query = QueryBuilder.Contains("name", new [] { "ahmet", "ertugrul" });
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"name\": { $in: [ \"ahmet\", \"ertugrul\" ] } }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void NotContainsTest()
        {
            var query = QueryBuilder.NotContains("age", new [] { 7, 13, 48 });
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $nin: [ 7, 13, 48 ] } }".Trim(), queryJson.Trim());
        }

        #endregion
        
        #region Logical Queries Tests
        
        [Test]
        public void AndTest1()
        {
            var query = QueryBuilder.And(QueryBuilder.GreaterThanOrEqual("age", 30), QueryBuilder.Equals("gender", "female"));
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ $and: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void AndTest2()
        {
            var expressions = new[]
            {
                QueryBuilder.GreaterThanOrEqual("age", 30),
                QueryBuilder.Equals("gender", "female")
            };
            
            var query = QueryBuilder.And(expressions);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ $and: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void OrTest1()
        {
            var query = QueryBuilder.Or(QueryBuilder.GreaterThanOrEqual("age", 30), QueryBuilder.Equals("gender", "female"));
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ $or: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void OrTest2()
        {
            var expressions = new[]
            {
                QueryBuilder.GreaterThanOrEqual("age", 30),
                QueryBuilder.Equals("gender", "female")
            };
            
            var query = QueryBuilder.Or(expressions);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ $or: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void NorTest1()
        {
            var query = QueryBuilder.Nor(QueryBuilder.GreaterThanOrEqual("age", 30), QueryBuilder.Equals("gender", "female"));
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ $nor: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void NorTest2()
        {
            var expressions = new[]
            {
                QueryBuilder.GreaterThanOrEqual("age", 30),
                QueryBuilder.Equals("gender", "female")
            };
            
            var query = QueryBuilder.Nor(expressions);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ $nor: [ { \"age\": { $gte: 30 } }, { \"gender\": \"female\" } ] }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void NotTest1()
        {
            var query = QueryBuilder.Not("age", 1.99);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $not: { $eq: 1.99 } } }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void NotTest2()
        {
            var query = QueryBuilder.Not(QueryBuilder.GreaterThanOrEqual("age", 30));
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"age\": { $not: { $gte: 30 } } }".Trim(), queryJson.Trim());
        }
        
        #endregion
        
        #region Element Queries Tests
        
        [Test]
        public void ExistTest()
        {
            var query = QueryBuilder.Exist("qty", true);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"qty\": { $exists: true } }".Trim(), queryJson.Trim());
        }
        
        [Test]
        public void TypeOfTest()
        {
            var query = QueryBuilder.TypeOf("qty", BsonType.MaxKey);
            var queryJson = query.ToString();
            Assert.NotNull(queryJson);
            Assert.AreEqual("{ \"qty\": { $type: \"maxKey\" } }".Trim(), queryJson.Trim());
        }
        
        #endregion
    }
}