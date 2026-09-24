using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Ertis.Schema.Types;

namespace Ertis.Schema.Serialization;

public sealed class FieldInfoCollectionJsonConverterFactory : JsonConverterFactory
{
    #region Methods
    
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(IEnumerable<IFieldInfo>).IsAssignableFrom(typeToConvert))
        {
            return false;
        }
        
        if (typeToConvert.IsArray)
        {
            return true;
        }
        
        // IEnumerable, IReadOnlyCollection, IReadOnlyList, ICollection, IList, List<IFieldInfo>
        return typeToConvert.IsAssignableFrom(typeof(List<IFieldInfo>));
    }
    
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(FieldInfoCollectionJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter) Activator.CreateInstance(converterType)!;
    }
    
    #endregion
}

public sealed class FieldInfoCollectionJsonConverter<TCollection> : JsonConverter<TCollection> where TCollection : IEnumerable<IFieldInfo>
{
    #region Methods
    
    public override void Write(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options)
    {
        var writtenNames = new HashSet<string>(StringComparer.Ordinal);
        var nameKey = GetNameKey(options);
        
        writer.WriteStartObject();
        
        foreach (var field in value)
        {
            if (!writtenNames.Add(field.Name))
            {
                throw new JsonException($"Duplicate field name: '{field.Name}'.");
            }
            
            writer.WritePropertyName(field.Name);
            
            var node = JsonSerializer.SerializeToNode(field, options)!.AsObject();
            node.Remove(nameKey);
            node.WriteTo(writer, options);
        }
        
        writer.WriteEndObject();
    }

    public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected StartObject but got {reader.TokenType}.");
        }
        
        var fields = new List<IFieldInfo>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (typeof(TCollection).IsArray)
                {
                    return (TCollection)(object) fields.ToArray();
                }
                
                return (TCollection)(object) fields;
            }
            
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected PropertyName but got {reader.TokenType}.");
            }
            
            var nameKey = GetNameKey(options);
            var name = reader.GetString();
            
            reader.Read();
            
            var node = JsonNode.Parse(ref reader)!.AsObject();
            node[nameKey] = name;
            
            if (node["type"]?.GetValue<string>() == FieldType.array.ToString())
            {
                node["itemSchema"]?[nameKey] = "$schema";
            }
            
            var field = node.Deserialize<IFieldInfo>(options);
            if (field is null)
            {
                throw new JsonException("Field value cannot be null.");
            }
            
            fields.Add(field);
        }
        
        throw new JsonException("Unexpected end of JSON.");
    }
    
    private static string GetNameKey(JsonSerializerOptions options)
    {
        return options.PropertyNamingPolicy?.ConvertName(nameof(IFieldInfo.Name)) ?? nameof(IFieldInfo.Name);
    }
    
    #endregion
}