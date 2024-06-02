using System;
using System.Collections.Generic;
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
            Assert.That(testTemplate == parsed);
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
            
            Assert.That("bla bla bla Ertuğrul foo bar foo bar Özcan kara uzum habbesi Ahmetle le le le canim{{  last name}} pof. (Created at: 1.01.2022 23:59:00, Created by: migration)" == formatted);
        }
        
        [Test]
        public void TemplateEngineFormatter_RouteValues_Test()
        {
            var testTemplate = "{id}";
            var testData = new Dictionary<string, object>
            {
                { "action", "Get" },
                { "controller", "Organizations" },
                { "id", "62b8d1e023af61a96846d4f3" },
                { "v", 1 }
            };
            
            var formatter = new Formatter(new ParserOptions
            {
                OpenBrackets = "{",
                CloseBrackets = "}"
            });
            
            var formatted = formatter.Format(testTemplate, testData);
            
            Assert.That("62b8d1e023af61a96846d4f3" == formatted);
        }
        
        [Test]
        public void TemplateEngineFormatter_UrlPattern_Test()
        {
            const string defaultUrlPattern = "{category.slug}/{slug}-{cid}";
            var testData = new Dictionary<string, object>
            {
                { "_id", "665c604238278d825ebcd90c" },
                { "contentType", "TextNews" },
                { "title", "Content Test" },
                { "slug", "content-test" },
                { "tags", new [] { "tag-1", "tag-2" } },
                { "category", new Dictionary<string, object> {
                    { "title", "Beşiktaş" },
                    { "slug", "besiktas" }
                }},
                { "cid", "10015" }
            };
            
            var formatter = new Formatter(new ParserOptions
            {
                OpenBrackets = "{",
                CloseBrackets = "}"
            });
            
            var formatted = formatter.Format(defaultUrlPattern, testData);
            
            Assert.That("besiktas/content-test-10015" == formatted);
        }
        
        [Test]
        public void TemplateEngineFormatter_Array_Test()
        {
            const string defaultUrlPattern = "{categories[0].slug}/{slug}-{cid}";
            var testData = new Dictionary<string, object>
            {
                { "_id", "665c604238278d825ebcd90c" },
                { "contentType", "TextNews" },
                { "title", "Content Test" },
                { "slug", "content-test" },
                { "tags", new [] { "tag-1", "tag-2" } },
                { "categories", new []
                {
                    new Dictionary<string, object> {
                        { "title", "Beşiktaş" },
                        { "slug", "besiktas" }
                    }
                }},
                { "cid", "10015" }
            };
            
            var formatter = new Formatter(new ParserOptions
            {
                OpenBrackets = "{",
                CloseBrackets = "}"
            });
            
            var formatted = formatter.Format(defaultUrlPattern, testData);
            
            Assert.That("besiktas/content-test-10015" == formatted);
        }

        #endregion
    }
}