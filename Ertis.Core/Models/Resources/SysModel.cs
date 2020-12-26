using System;
using Newtonsoft.Json;

namespace Ertis.Core.Models.Resources
{
	public class SysModel
	{
		#region Properties

		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? CreatedAt { get; set; }
		
		[JsonProperty("created_by", NullValueHandling = NullValueHandling.Ignore)]
		public string CreatedBy { get; set; }
		
		[JsonProperty("modified_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? ModifiedAt { get; set; }
		
		[JsonProperty("modified_by", NullValueHandling = NullValueHandling.Ignore)]
		public string ModifiedBy { get; set; }

		#endregion
	}
}