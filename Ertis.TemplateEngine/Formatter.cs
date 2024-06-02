using System;
using System.Collections;
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
                        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                        switch (this.Parser.Options.UndefinedStrategy)
                        {
                            case UndefinedStrategy.Ignore:
                                stringBuilder.Append(segment);
                                break;
                            case UndefinedStrategy.Remove:
                                break;
                            case UndefinedStrategy.Throw:
                                throw new ArgumentException($"{placeHolder.Value} is undefined");
                            case UndefinedStrategy.Swap:
                                if (!string.IsNullOrEmpty(this.Parser.Options.Fallback))
                                {
                                    stringBuilder.Append(this.Parser.Options.Fallback);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    stringBuilder.Append(segment);
                }
            }

            return stringBuilder.ToString();
        }

        public ITemplateSegment[] LookUp(string template)
        {
            return this.Parser.Parse(template).ToArray();
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
            }
            else if (key.Contains('[') && key.Contains(']'))
            {
                var arrayStartIndex = key.IndexOf('[');
                var arrayEndIndex = key.IndexOf(']');
                var indexLength = arrayEndIndex - arrayStartIndex;
                if (arrayStartIndex > 0 && indexLength > 1)
                {
                    if (int.TryParse(key.AsSpan(arrayStartIndex + 1, indexLength - 1), out var index))
                    {
                        var fieldKey = key[..arrayStartIndex];
                        if (dictionary.TryGetValue(fieldKey, out var obj) && obj is IList array)
                        {
                            if (index < 0)
                            {
                                index = array.Count - index;
                            }

                            if (index > array.Count)
                            {
                                throw new ArgumentOutOfRangeException(fieldKey, array, "The index parameter was greater than array length");
                            }
                            
                            if (pathParts.Length == 1)
                            {
                                return array[index];
                            }
                            else if (array[index] is IDictionary<string, object> subDictionary)
                            {
                                return ExtractData(string.Join(".", pathParts.Skip(1)), subDictionary);
                            }
                        }
                    }
                }
            }
            
            return null;
        }
        
        #endregion
    }
}