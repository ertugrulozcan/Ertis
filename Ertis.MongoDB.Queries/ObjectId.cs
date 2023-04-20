namespace Ertis.MongoDB.Queries
{
    public class ObjectId : QueryValue<string>, IQueryExpression
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
        public ObjectId(string id) : base(id)
        {
            this.Id = id;
        }

        #endregion
        
        #region Methods

        public override string ToString()
        {
            return $"ObjectId(\"{this.Id}\")";
        }

        #endregion
    }
}