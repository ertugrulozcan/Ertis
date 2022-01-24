using System.Collections.Generic;

namespace Ertis.MongoDB.Queries
{
    internal class CustomQuery : IQuery, IHasChildren
    {
        #region Fields

        private readonly IQuery _query;

        #endregion
        
        #region Properties

        internal string Operator { get; init; }
        
        internal IQuery Value 
        {
            get => this._query;
            init
            {
                this._query = value;
                this.Children.Insert(0, value);
            }
        }
        
        public List<IQuery> Children { get; init; }

        public bool ShowOperatorTag { get; init; } = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomQuery()
        {
            this.Children = new List<IQuery>();
        }

        #endregion
        
        #region Methods

        public void AddQuery(IQuery query)
        {
            this.Children.Add(query);
        }

        public override string ToString()
        {
            var expressionJsons = new List<string>();
            foreach (var query in this.Children)
            {
                var expressionJson = QueryHelper.GetInnerQuery(query);
                if (!string.IsNullOrEmpty(expressionJson))
                {
                    expressionJsons.Add(expressionJson);
                }
            }
                
            if (this.ShowOperatorTag && !string.IsNullOrEmpty(this.Operator))
            {
                return "{ \"" + this.Operator + "\": { " + string.Join(", ", expressionJsons) + " } }";
            }
            else
            {
                return "{ " + string.Join(", ", expressionJsons) + " }";
            }
        }

        #endregion
    }
}