using Ertis.Schema.Types;

namespace Ertis.Schema.Exceptions
{
    public class FieldValidationException : ErtisSchemaValidationException
    {
        #region Fields

        private readonly string fieldPath;

        #endregion
        
        #region Properties

        private IFieldInfo FieldInfo { get; }

        public string FieldName => this.FieldInfo.Name;

        public string FieldPath
        {
            get => string.IsNullOrEmpty(this.fieldPath) ? this.FieldInfo.Path : this.fieldPath;
            private init => this.fieldPath = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="fieldPath"></param>
        public FieldValidationException(string message, IFieldInfo fieldInfo, string fieldPath = null) : base(message)
        {
            this.FieldInfo = fieldInfo;
            this.FieldPath = fieldPath;
        }

        #endregion
    }
}