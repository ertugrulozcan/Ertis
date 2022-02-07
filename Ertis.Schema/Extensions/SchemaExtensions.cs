using System;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;

namespace Ertis.Schema.Extensions
{
    public static class SchemaExtensions
    {
        #region Methods

        public static bool CheckPropertiesUniqueness(this ISchema schema, out Exception exception)
        {
            var fieldInfos = schema.Properties;
            var distinctCount = fieldInfos.Select(x => x.Name).Distinct().Count();
            if (fieldInfos.Count != distinctCount)
            {
                exception = new ErtisSchemaValidationException("Duplicate property declaration. Property names are must be unique.");
                return false;
            }
            
            exception = null;
            return true;
        }

        #endregion
    }
}