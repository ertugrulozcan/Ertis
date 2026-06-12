using System.Dynamic;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Schema.Extensions;

// ReSharper disable once UnusedType.Global
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
    
    extension(ExpandoObject expandoObject)
    {
        public T? GetProperty<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "DynamicExtensions.GetProperty(path) path can not be null!");
            }
            
            var segments = path.Split('.');
            if (segments.Length == 1)
            {
                var propertyName = path;
                var expandoDictionary = expandoObject as IDictionary<string, object>;
                if (expandoDictionary.TryGetValue(propertyName, out var value))
                {
                    return (T) value;
                }
            }
            else
            {
                var expandoDictionary = expandoObject as IDictionary<string, object>;
                if (expandoDictionary.ContainsKey(segments[0]))
                {
                    var subPath = path[(segments[0].Length + 1)..];
                    return expandoDictionary[segments[0]].ToExpandoObject().GetProperty<T>(subPath);
                }
            }
            
            return default;
        }
        
        public ExpandoObject SetProperty(string path, object value)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "DynamicExtensions.SetProperty(path) path can not be null!");
            }
            
            var segments = path.Split('.');
            if (segments.Length == 1)
            {
                var propertyName = path;
                var expandoDictionary = expandoObject as IDictionary<string, object?>;
                expandoDictionary[propertyName] = value;
                return expandoDictionary.ToDynamic();
            }
            else
            {
                var expandoDictionary = expandoObject as IDictionary<string, object?>;
                if (expandoDictionary.ContainsKey(segments[0]))
                {
                    var subPath = path[(segments[0].Length + 1)..];
                    var newValue = expandoDictionary[segments[0]]?.ToExpandoObject().SetProperty(subPath, value);
                    expandoDictionary[segments[0]] = newValue;
                }
                else
                {
                    expandoDictionary.Add(segments[0], value);
                }
                
                return expandoDictionary.ToDynamic();
            }
        }
        
        public ExpandoObject RemoveProperty(string propertyName)
        {
            var expandoDictionary = expandoObject as IDictionary<string, object?>;
            expandoDictionary.Remove(propertyName);
            return expandoDictionary.ToDynamic();
        }
        
        public ExpandoObject Clone()
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
    }
    
    #endregion
}