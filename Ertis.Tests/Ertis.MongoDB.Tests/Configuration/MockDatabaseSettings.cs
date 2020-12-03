using Ertis.MongoDB.Configuration;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Configuration
{
	public class MockDatabaseSettings : IDatabaseSettings
	{
		#region Properties
		
		public string Username { get; set; }
		
		public string Password { get; set; }
		
		public string Host { get; set; }
		
		public int Port { get; set; }
		
		public string DatabaseName { get; set; }

		#endregion
	}
}