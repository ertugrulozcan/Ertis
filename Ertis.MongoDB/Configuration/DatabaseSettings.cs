// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.MongoDB.Configuration
{
	public class DatabaseSettings : IDatabaseSettings
	{
		#region Properties
		
		public string ConnectionString { get; set; }
		
		public string DefaultAuthDatabase { get; set; }
		
		public bool? AllowDiskUse { get; set; }
		
		#endregion
	}
}