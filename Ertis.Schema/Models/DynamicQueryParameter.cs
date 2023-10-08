using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Models
{
	public class DynamicQueryParameter
	{
		#region Properties
		
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("slug")]
		public string Slug { get; set; }
		
		[JsonProperty("description")]
		public string Description { get; set; }
		
		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public DynamicQueryParameterType Type { get; set; }
		
		[JsonProperty("defaultValue")]
		public object DefaultValue { get; set; }
		
		[JsonProperty("isRequired")]
		public bool IsRequired { get; set; }

		#endregion
	}
    
	public enum DynamicQueryParameterType
	{
		@string,
		number,
		date,
		boolean,
	}
}