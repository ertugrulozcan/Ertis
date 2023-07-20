using System.Collections.Generic;
using Ertis.MongoDB.Configuration;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Configuration
{
	public class MockDatabaseSettings : IDatabaseSettings
	{
		#region Properties
		
		public string Scheme { get; set; }
		
		public string Username { get; set; }
		
		public string Password { get; set; }
		
		public string Host { get; set; }
		
		public int Port { get; set; }
		
		public string DefaultAuthDatabase { get; set; }
		
		public bool? AllowDiskUse { get; set; }
		
		public IDictionary<string, object> QueryString { get; set; }

		#endregion
	}
}