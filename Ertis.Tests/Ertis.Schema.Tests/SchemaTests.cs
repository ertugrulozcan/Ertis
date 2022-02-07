using System;
using System.Collections.Generic;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Schema.Tests
{
    public class MockContentType : ISchema
    {
        #region Properties

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("properties")]
        public IEnumerable<IFieldInfo> Properties { get; set; }

        [JsonProperty("allowAdditionalProperties")]
        public bool AllowAdditionalProperties { get; init; }

        #endregion

        #region Methods
        
        public bool IsValid(object obj, out Exception exception)
        {
            if (obj == null)
            {
                exception = null;
                return true;
            }

            var objectFieldInfo = new ObjectFieldInfo(this.Name)
            {
                Properties = this.Properties,
                AllowAdditionalProperties = this.AllowAdditionalProperties
            };

            var isValid = objectFieldInfo.IsValid(obj, out var ex);
            exception = ex;

            return isValid;
        }

        #endregion
    }
    
    public class SchemaTests
    {
        #region Methods

        [Test]
        public void SchemaValidationTest1()
        {
            var mockContentType = new MockContentType
            {
                Name = "MockContentType",
                Properties = new[]
                {
                    new StringFieldInfo("firstname"),
                    new StringFieldInfo("lastname"),
                }
            };

            var mock = new
            {
                firstname = "Ertuğrul",
                lastname = "Özcan"
            };

            Assert.IsTrue(mockContentType.IsValid(mock, out _));
        }
        
        [Test]
        public void SchemaValidationTest2()
        {
            var mockContentType = new MockContentType
            {
                Name = "MockContentType",
                Properties = new[]
                {
                    new StringFieldInfo("firstname"),
                    new StringFieldInfo("lastname"),
                },
                AllowAdditionalProperties = true
            };

            var mock = new
            {
                firstname = "Ertuğrul",
                lastname = "Özcan",
                extra = "bla bla"
            };

            Assert.IsTrue(mockContentType.IsValid(mock, out _));
        }
        
        [Test]
        public void SchemaValidationTest3()
        {
            var mockContentType = new MockContentType
            {
                Name = "MockContentType",
                Properties = new[]
                {
                    new StringFieldInfo("firstname"),
                    new StringFieldInfo("lastname"),
                }
            };

            var mock = new
            {
                firstname = "Ertuğrul",
                lastname = "Özcan",
                extra = "bla bla"
            };

            Assert.IsFalse(mockContentType.IsValid(mock, out var exception));
            Assert.AreEqual("Additional properties not allowed in this object schema. (extra)", exception.Message);
        }
        
        #endregion
    }
}