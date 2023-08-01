using Ertis.MongoDB.Configuration;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Configuration
{
	public class MockDatabaseSettings : IDatabaseSettings
	{
		#region Properties
		
		public string ConnectionString { get; set; }
		
		public string DefaultAuthDatabase { get; set; }
		
		public bool? AllowDiskUse { get; set; }

		#endregion
	}
}