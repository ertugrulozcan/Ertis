using System.Collections.Generic;

namespace Ertis.MongoDB.Configuration
{
	public interface IDatabaseSettings
	{
		#region Properties

		string Scheme { get; }
		
		string Username { get; }
		
		string Password { get; }
		
		string Host { get; }
		
		int Port { get; }
		
		string DefaultAuthDatabase { get; }
		
		bool? AllowDiskUse { get; }

		IDictionary<string, object> QueryString { get; }

		#endregion
	}
}