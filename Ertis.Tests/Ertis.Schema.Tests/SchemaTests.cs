using System;
using System.IO;
using Ertis.Schema.Dynamics;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Schema.Tests
{
    public class SchemaTests
    {
        #region Methods

        [Test]
        public void SchemaValidationTest1()
        {
            var json = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "event-test.json"));
            var dynamicObject = DynamicObject.Parse(json);
            if (dynamicObject != null)
            {
                
            }
        }
        
        #endregion
    }
}