using System;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;
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
                    var fieldTypeName = jObject["type"]?.Value<string>();
                    if (Enum.TryParse(fieldTypeName, out FieldType fieldType))
                    {
                        var json = jObject.ToString(Formatting.None);
                        IFieldInfo fieldInfo = fieldType switch
                        {
                            // Primitive Types
                            FieldType.@object => JsonConvert.DeserializeObject<ObjectFieldInfo>(json, new FieldInfoCollectionJsonConverter()),
                            FieldType.@string => JsonConvert.DeserializeObject<StringFieldInfo>(json),
                            FieldType.integer => JsonConvert.DeserializeObject<IntegerFieldInfo>(json),
                            FieldType.@float => JsonConvert.DeserializeObject<FloatFieldInfo>(json),
                            FieldType.boolean => JsonConvert.DeserializeObject<BooleanFieldInfo>(json),
                            FieldType.array => JsonConvert.DeserializeObject<ArrayFieldInfo>(json),
                            FieldType.@enum => JsonConvert.DeserializeObject<EnumFieldInfo>(json),
                            FieldType.@const => JsonConvert.DeserializeObject<ConstantFieldInfo>(json),

                            // Custom Types
                            FieldType.json => JsonConvert.DeserializeObject<JsonFieldInfo>(json),
                            FieldType.date => JsonConvert.DeserializeObject<DateFieldInfo>(json),
                            FieldType.datetime => JsonConvert.DeserializeObject<DateTimeFieldInfo>(json),
                            FieldType.longtext => JsonConvert.DeserializeObject<LongTextFieldInfo>(json),
                            FieldType.richtext => JsonConvert.DeserializeObject<RichTextFieldInfo>(json),
                            FieldType.email => JsonConvert.DeserializeObject<EmailAddressFieldInfo>(json),
                            FieldType.uri => JsonConvert.DeserializeObject<UriFieldInfo>(json),
                            FieldType.hostname => JsonConvert.DeserializeObject<HostNameFieldInfo>(json),
                            FieldType.color => JsonConvert.DeserializeObject<ColorFieldInfo>(json),
                            FieldType.location => JsonConvert.DeserializeObject<LocationFieldInfo>(json),
                            FieldType.reference => JsonConvert.DeserializeObject<ReferenceFieldInfo>(json),
                            FieldType.code => JsonConvert.DeserializeObject<CodeFieldInfo>(json),

                            // Unknown Type
                            _ => throw new SchemaValidationException($"Unknown field type : '{fieldTypeName}' ({fieldName})")
                        };

                        if (fieldInfo != null)
                        {
                            fieldInfo.Name = fieldName;
                        }
                    
                        return fieldInfo;
                    }
                    else
                    {
                        throw new SchemaValidationException($"Unknown field type : '{fieldTypeName}' ({fieldName})");
                    }
                }
                else
                {
                    throw new SchemaValidationException($"Field type is required ({fieldName})");
                }
            }
            catch (Exception ex)
            {
                switch (ex.InnerException)
                {
                    case FieldValidationException:
                        throw ex.InnerException;
                    case SchemaValidationException:
                        throw ex.InnerException;
                    default:
                        throw new SchemaValidationException(ex.Message);
                }
            }
        }
    }
}