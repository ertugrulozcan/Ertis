using Ertis.Schema.Types;

namespace Ertis.Schema.Exceptions
{
    public class FieldValidationException : ErtisSchemaValidationException
    {
        #region Properties

        private IFieldInfo FieldInfo { get; }

        public string FieldName => this.FieldInfo.Name;

        public string FieldPath => this.FieldInfo.Path;
        
        public bool ThrowEvenOnCreate { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="fieldInfo"></param>
        public FieldValidationException(string message, IFieldInfo fieldInfo) : base(message)
        {
            this.FieldInfo = fieldInfo;
        }

        #endregion
    }
}