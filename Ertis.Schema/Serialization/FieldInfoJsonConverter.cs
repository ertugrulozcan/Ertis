using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Serialization
{
    public class FieldInfoJsonConverter : JsonConverter<IFieldInfo>
    {
        public override void WriteJson(JsonWriter writer, IFieldInfo value, JsonSerializer serializer)
        {
            var jToken = JToken.FromObject(value);
            jToken.WriteTo(writer);
        }

        public override IFieldInfo ReadJson(JsonReader reader, Type objectType, IFieldInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            return Deserialize(jObject, null);
        }

        public static IFieldInfo Deserialize(JObject jObject, string fieldName)
        {
            try
            {
                if (jObject.ContainsKey("type"))
                {
                    var type = jObject["type"]?.Value<string>();
                    
                    /*
                    if (type == "array" && jObject.ContainsKey("itemSchema") && jObject["itemSchema"] is JObject itemSchemaNode)
                    {
                        if (itemSchemaNode.ContainsKey("name"))
                        {
                            itemSchemaNode["name"] = $"{fieldName}_item";
                        }
                        else
                        {
                            itemSchemaNode.Add("name", $"{fieldName}_item");
                        }
                    }
                    */

                    var json = jObject.ToString(Formatting.None);
                    IFieldInfo fieldInfo = type switch
                    {
                        "object" => JsonConvert.DeserializeObject<ObjectFieldInfo>(json, new FieldInfoCollectionJsonConverter()),
                        "string" => JsonConvert.DeserializeObject<StringFieldInfo>(json),
                        "integer" => JsonConvert.DeserializeObject<IntegerFieldInfo>(json),
                        "float" => JsonConvert.DeserializeObject<FloatFieldInfo>(json),
                        "boolean" => JsonConvert.DeserializeObject<BooleanFieldInfo>(json),
                        "array" => JsonConvert.DeserializeObject<ArrayFieldInfo>(json),
                        _ => throw new ErtisSchemaValidationException($"Unknown field type : '{type}' ({fieldName})")
                    };

                    if (fieldInfo != null)
                    {
                        fieldInfo.Name = fieldName;
                    }
                    
                    return fieldInfo;
                }
                else
                {
                    throw new ErtisSchemaValidationException($"Field type is required ({fieldName})");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is FieldValidationException)
                {
                    throw ex.InnerException;
                }
                
                throw;
            }
        }
    }
}