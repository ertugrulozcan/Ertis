using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Ertis.Core.Dynamics.Primitives;

namespace Ertis.Core.Dynamics
{
	public class DynamicObjectSchema
	{
		#region Properties

		public IDynamicObjectField[] Fields { get; private set; }

		#endregion

		#region Operator Overloading

		public IDynamicObjectField this[string key] => this.Fields?.FirstOrDefault(x => x.Name == key);

		#endregion

		#region Methods

		public static DynamicObjectSchema Parse(string json)
		{
			var deserializedObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
			if (deserializedObject is JToken jToken)
			{
				var fields = new List<IDynamicObjectField>();
				
				foreach (var propertyToken in jToken)
				{
					if (propertyToken is JProperty jProperty)
					{
						if (jProperty.Value is JObject jObject)
						{
							var fieldType = DynamicObjectField.ParseFieldType(jObject["type"]?.ToString());

							var defaultField = jObject["default"];
							bool isRequired = jObject.Value<bool>("is_required");

							DynamicObjectSchema objectSchema = null;
							if (jObject.ContainsKey("schema"))
							{
								objectSchema = Parse(jObject["schema"]?.ToString());	
							}
							
							IDynamicObjectField field = fieldType switch
							{
								DynamicObjectFieldType.Object => new DynamicObject(propertyToken.Path, objectSchema, isRequired, ParseNestedObject(defaultField)),
								DynamicObjectFieldType.Array => new DynamicArray(propertyToken.Path, objectSchema, isRequired, ParseArray(defaultField as JArray)),
								DynamicObjectFieldType.String => new DynamicString(propertyToken.Path, isRequired, defaultField?.Value<string>()),
								DynamicObjectFieldType.Integer => new DynamicInteger(propertyToken.Path, isRequired, defaultField?.Value<int>()),
								DynamicObjectFieldType.Double => new DynamicDouble(propertyToken.Path, isRequired, defaultField?.Value<double>()),
								DynamicObjectFieldType.Boolean => new DynamicBoolean(propertyToken.Path, isRequired, defaultField?.Value<bool>()),
								DynamicObjectFieldType.Date => new DynamicDate(propertyToken.Path, isRequired, ParseToDateTime(defaultField)),
								_ => throw new ArgumentOutOfRangeException()
							};

							fields.Add(field);
						}
					}
				}
				
				return new DynamicObjectSchema
				{
					Fields = fields.ToArray()
				};
			}

			throw new Exception("Json could not deserialized!");
		}
		
		private static dynamic ParseNestedObject(JToken defaultField)
		{
			if (defaultField == null)
			{
				return null;
			}

			switch (defaultField.Type)
			{
				case JTokenType.Object:
				case JTokenType.Constructor:
				case JTokenType.Property:
					var converter = new ExpandoObjectConverter();
					var json = defaultField.ToString();
					var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json, converter);
					return obj;	
				case JTokenType.Array:
					return ParseArray(defaultField as JArray);
				case JTokenType.Integer:
					return defaultField.Value<int>();
				case JTokenType.Float:
					return defaultField.Value<double>();
				case JTokenType.Boolean:
					return defaultField.Value<bool>();
				case JTokenType.String:
				case JTokenType.Date:
				case JTokenType.TimeSpan:
				case JTokenType.Raw:
				case JTokenType.Bytes:
				case JTokenType.Guid:
				case JTokenType.Uri:
				case JTokenType.Comment:
					return defaultField.ToString();
				case JTokenType.None:
				case JTokenType.Null:
				case JTokenType.Undefined:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private static dynamic[] ParseArray(JArray defaultField)
		{
			return defaultField?.Select(ParseNestedObject).ToArray();
		}

		private static DateTime? ParseToDateTime(JToken jToken)
		{
			if (jToken != null)
			{
				var str = jToken.Value<string>();
				if (!string.IsNullOrEmpty(str))
				{
					if (DateTime.TryParse(str, out var date))
					{
						return date;
					}
				}
			}

			return null;
		}

		#endregion
	}
}