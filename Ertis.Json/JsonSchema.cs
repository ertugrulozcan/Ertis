using System;
using System.Collections.Generic;
using System.Linq;
using Ertis.Json.Extensions;
using Newtonsoft.Json.Schema;

namespace Ertis.Json
{
	public static class JsonSchema
	{
		#region Methods

		public static JSchema Parse(string json, out IList<string> validationErrors)
		{
			try
			{
				var schema = JSchema.Parse(json);
				
				var isValidDefaults = schema.IsValidDefaults(out var validationErrors_);
				if (isValidDefaults != null && !isValidDefaults.Value)
				{
					validationErrors = validationErrors_.Select(x => $"{x.Message} (Line: {x.LineNumber}, Position: {x.LinePosition})").ToList();
					return null;
				}

				if (schema.IsContainsAdditionalProperties(out var additionalPropertyErrors))
				{
					validationErrors = additionalPropertyErrors;
					return null;
				}

				validationErrors = null;
				return schema;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		#endregion
	}
}