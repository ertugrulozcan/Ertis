using System;
using Ertis.TemplateEngine;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.TemplateEngine.Tests
{
    public class ParserTests
    {
        #region Methods

        [Test]
        public void TemplateEngineParserTest()
        {
            var testTemplate = "bla bla bla {{name}} foo bar foo bar {{surname}} kara uzum habbesi {{ first_name  }}le le le le canim{{  last name}} pof.";
            var parser = new Parser();
            var segments = parser.Parse(testTemplate);
            var parsed = string.Join(string.Empty, segments);
            Assert.AreEqual(testTemplate, parsed);
        }
        
        [Test]
        public void TemplateEngineFormatterTest()
        {
            var testTemplate = "bla bla bla {{name}} foo bar foo bar {{surname}} kara uzum habbesi {{ first_name  }}le le le le canim{{  last name}} pof. (Created at: {{sys.created_at}}, Created by: {{sys.created_by}})";
            var testData = new
            {
                first_name = "Ahmet",
                name = "Ertuğrul",
                surname = "Özcan",
                sys = new
                {
                    created_at = new DateTime(2022, 01, 01, 23, 59, 00),
                    created_by = "migration"
                }
            };
            
            var formatter = new Formatter();
            var formatted = formatter.Format(testTemplate, testData);
            
            Assert.AreEqual("bla bla bla Ertuğrul foo bar foo bar Özcan kara uzum habbesi Ahmetle le le le canim{{  last name}} pof. (Created at: 01/01/2022 23:59:00, Created by: migration)", formatted);
        }

        #endregion
    }
}