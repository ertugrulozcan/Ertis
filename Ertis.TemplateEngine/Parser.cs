using System;
using System.Collections.Generic;

namespace Ertis.TemplateEngine
{
    public class Parser
    {
        #region Constants

        private const string DEFAULT_OPEN_BRACKETS = "{{";
        private const string DEFAULT_CLOSE_BRACKETS = "}}";

        #endregion

        #region Properties

        internal ParserOptions Options { get; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public Parser(ParserOptions options = null)
        {
            if (options == null)
            {
                this.Options = new ParserOptions
                {
                    OpenBrackets = DEFAULT_OPEN_BRACKETS,
                    CloseBrackets = DEFAULT_CLOSE_BRACKETS
                };
            }
            else
            {
                if (string.IsNullOrEmpty(options.OpenBrackets) || string.IsNullOrEmpty(options.CloseBrackets))
                {
                    throw new ArgumentException("Open&Close brackets could not be null or empty!");
                }
                
                this.Options = options;
            }
        }

        #endregion

        #region Methods

        public IEnumerable<ITemplateSegment> Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield return new RawPart { RawValue = text };
                yield break;
            }

            var template = text;
            while (TryFindPlaceHolder(template, out var placeHolder))
            {
                if (placeHolder == null)
                {
                    break;
                }

                if (placeHolder.StartIndex > 0)
                {
                    var rawText = template.Substring(0, placeHolder.StartIndex);
                    yield return new RawPart { RawValue = rawText };
                }
                
                template = template.Substring(placeHolder.StartIndex + placeHolder.Outer.Length);
                
                yield return placeHolder;
            }

            if (!string.IsNullOrEmpty(template))
            {
                yield return new RawPart { RawValue = template };
            }
        }

        private bool TryFindPlaceHolder(string template, out PlaceHolder placeHolder)
        {
            placeHolder = this.FindPlaceHolder(template);
            return placeHolder != null;
        }
        
        private PlaceHolder FindPlaceHolder(string template)
        {
            var startIndex = template.IndexOf(this.Options.OpenBrackets, StringComparison.Ordinal);
            var endIndex = template.IndexOf(this.Options.CloseBrackets, StringComparison.Ordinal);
            if (startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
            {
                return null;
            }
            
            var placeholder = template.Substring(startIndex + this.Options.OpenBrackets.Length, endIndex - startIndex - this.Options.OpenBrackets.Length);
            return new PlaceHolder
            {
                Inner = placeholder,
                Outer = $"{this.Options.OpenBrackets}{placeholder}{this.Options.CloseBrackets}",
                Value = placeholder.Trim(),
                StartIndex = startIndex,
                Length = endIndex - startIndex + this.Options.CloseBrackets.Length,
                OpenBrackets = this.Options.OpenBrackets,
                CloseBrackets = this.Options.CloseBrackets,
            };
        }

        #endregion
    }
}