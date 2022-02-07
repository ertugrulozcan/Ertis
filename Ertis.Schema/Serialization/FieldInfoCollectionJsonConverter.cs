using System;
using System.Collections.Generic;
using Ertis.Schema.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Serialization
{
    public class FieldInfoCollectionJsonConverter : JsonConverter<IEnumerable<IFieldInfo>>
    {
        public override void WriteJson(JsonWriter writer, IEnumerable<IFieldInfo> value, JsonSerializer serializer)
        {
            ToJsonObject(value).WriteTo(writer);
        }

        public override IEnumerable<IFieldInfo> ReadJson(JsonReader reader, Type objectType, IEnumerable<IFieldInfo> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            return Deserialize(jObject);
        }

        private static IEnumerable<IFieldInfo> Deserialize(JObject rootNode)
        {
            var fieldInfoList = new List<IFieldInfo>();
            
            foreach (var (name, jToken) in rootNode)
            {
                if (jToken is JObject jObject)
                {
                    fieldInfoList.Add(FieldInfoJsonConverter.Deserialize(jObject, name));   
                }
            }

            return fieldInfoList;
        }
        
        private static JObject ToJsonObject(IEnumerable<IFieldInfo> properties)
        {
            var rootNode = new JObject();
            foreach (var fieldInfo in properties)
            {
                if (!string.IsNullOrEmpty(fieldInfo.Name))
                {
                    var jObject = JObject.FromObject(fieldInfo);
                    jObject.Remove("name");
                    rootNode.Add(fieldInfo.Name, jObject);   
                }
            }

            return rootNode;
        }

        public static string Serialize(IEnumerable<IFieldInfo> properties)
        {
            return ToJsonObject(properties).ToString();
        }
        
        public static IEnumerable<IFieldInfo> Deserialize(string json)
        {
            return string.IsNullOrEmpty(json) ? null : Deserialize(JObject.Parse(json));
        }
    }
}