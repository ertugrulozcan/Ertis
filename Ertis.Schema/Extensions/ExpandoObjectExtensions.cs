using System.Dynamic;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Schema.Extensions;

public static class ExpandoObjectExtensions
{
    #region Methods
    
    public static ExpandoObject ToExpandoObject(this object obj)
    {
        var dictionary = obj.ToDictionary();
        IDictionary<string, object?> expando = new ExpandoObject();
        foreach (var pair in dictionary)
        {
            if (pair.Value is IDictionary<string, object?> childDictionary)
            {
                expando.Add(new KeyValuePair<string, object?>(pair.Key, childDictionary.ToDynamic()));
            }
            else
            {
                expando.Add(pair);   
            }
        }
        
        return (ExpandoObject) expando;
    }
    
    public static T? GetProperty<T>(this ExpandoObject expandoObject, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path), "DynamicExtensions.GetProperty(path) path can not be null!");
        }
        
        var segments = path.Split('.');
        if (segments.Length == 1)
        {
            var propertyName = path; 
            IDictionary<string, object?> expandoDictionary = expandoObject;
            if (expandoDictionary.TryGetValue(propertyName, out var value))
            {
                return (T?) value;    
            }
        }
        else
        {
            IDictionary<string, object?> expandoDictionary = expandoObject;
            if (expandoDictionary.ContainsKey(segments[0]))
            {
                var subPath = path[(segments[0].Length + 1)..];
                var segmentValue = expandoDictionary[segments[0]];
                if (segmentValue == null)
                {
                    return (T?) segmentValue;
                }
                
                return segmentValue.ToExpandoObject().GetProperty<T>(subPath);
            }
        }
        
        return default;
    }
    
    public static ExpandoObject SetProperty(this ExpandoObject expandoObject, string path, object value)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path), "DynamicExtensions.SetProperty(path) path can not be null!");
        }
        
        var segments = path.Split('.');
        if (segments.Length == 1)
        {
            var propertyName = path; 
            IDictionary<string, object?> expandoDictionary = expandoObject;
            
            // ReSharper disable once RedundantDictionaryContainsKeyBeforeAdding
            if (expandoDictionary.ContainsKey(propertyName))
            {
                expandoDictionary[propertyName] = value;    
            }
            else
            {
                expandoDictionary.Add(propertyName, value);
            }
            
            return expandoDictionary.ToDynamic();   
        }
        else
        {
            IDictionary<string, object?> expandoDictionary = expandoObject;
            if (expandoDictionary.ContainsKey(segments[0]))
            {
                var subPath = path[(segments[0].Length + 1)..];
                var segmentValue = expandoDictionary[segments[0]];
                if (segmentValue == null)
                {
                    return expandoObject;
                }
                
                var newValue = segmentValue.ToExpandoObject().SetProperty(subPath, value);
                expandoDictionary[segments[0]] = newValue;
            }
            else
            {
                expandoDictionary.Add(segments[0], value);
            }
            
            return expandoDictionary.ToDynamic();
        }
    }
    
    public static ExpandoObject RemoveProperty(this ExpandoObject expandoObject, string propertyName)
    {
        IDictionary<string, object?> expandoDictionary = expandoObject;
        expandoDictionary.Remove(propertyName);
        return expandoDictionary.ToDynamic();
    }
    
    public static ExpandoObject Clone(this ExpandoObject expandoObject)
    {
        var dictionary = expandoObject.ToDictionary();
        IDictionary<string, object?> expando = new ExpandoObject();
        foreach (var pair in dictionary)
        {
            if (pair.Value is IDictionary<string, object?> childDictionary)
            {
                expando.Add(new KeyValuePair<string, object?>(pair.Key, childDictionary.ToDynamic()));
            }
            else
            {
                expando.Add(pair);   
            }
        }
        
        return (ExpandoObject) expando;
    }
    
    #endregion
}