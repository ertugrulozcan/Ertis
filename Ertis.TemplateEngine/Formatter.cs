using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ertis.TemplateEngine
{
    public class Formatter
    {
        #region Properties

        private Parser Parser { get; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public Formatter(ParserOptions options = null)
        {
            this.Parser = new Parser(options);
        }

        #endregion

        #region Methods

        public string Format(string template, object data)
        {
            if (string.IsNullOrEmpty(template) || data == null)
            {
                return template;
            }

            var dataDictionary = data.ToDictionary();
            var segments = this.Parser.Parse(template);
            var stringBuilder = new StringBuilder();
            
            foreach (var segment in segments)
            {
                if (segment is PlaceHolder placeHolder)
                {
                    var value = ExtractData(placeHolder.Value, dataDictionary);
                    if (value != null)
                    {
                        stringBuilder.Append(value);
                    }
                    else
                    {
                        stringBuilder.Append(segment);
                    }
                }
                else
                {
                    stringBuilder.Append(segment);
                }
            }

            return stringBuilder.ToString();
        }

        private static object ExtractData(string path, IDictionary<string, object> dictionary)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            
            var pathParts = path.Split('.');
            var key = pathParts.First();
            if (dictionary.ContainsKey(key))
            {
                if (pathParts.Length == 1)
                {
                    return dictionary[key];
                }
                else if (dictionary[key] is IDictionary<string, object> subDictionary)
                {
                    return ExtractData(string.Join(".", pathParts.Skip(1)), subDictionary);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
        #endregion
    }
}