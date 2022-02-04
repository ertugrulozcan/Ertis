namespace Ertis.MongoDB.Queries
{
    public class ObjectId : IQuery, IQueryExpression
    {
        #region Properties

        private string Id { get; }

        public string Field => "_id";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ObjectId</param>
        public ObjectId(string id)
        {
            this.Id = id;
        }

        #endregion
        
        #region Methods

        public override string ToString()
        {
            return "{ \"" + this.Field + "\": " + $"ObjectId(\"{this.Id}\")" + " }";
        }

        #endregion
    }
}