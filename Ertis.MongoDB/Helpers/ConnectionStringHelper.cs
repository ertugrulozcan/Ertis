using System.Collections.Generic;
using System.Linq;
using Ertis.MongoDB.Configuration;

namespace Ertis.MongoDB.Helpers
{
	public static class ConnectionStringHelper
	{
		#region Methods

		public static string GenerateConnectionString(IDatabaseSettings settings)
		{
			// mongodb://[username:password@]host1[:port1][,...hostN[:portN]][/[database_name][?options]]

			string credentialsSection = null;
			if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
			{
				credentialsSection = $"{settings.Username}:{settings.Password}@";
			}
			
			string serverSection = null;
			if (!string.IsNullOrEmpty(settings.Host))
			{
				serverSection = settings.Port > 0 ? $"{settings.Host}:{settings.Port}" : $"{settings.Host}";
			}
			
			string databaseSection = null;
			if (!string.IsNullOrEmpty(settings.DatabaseName))
			{
				databaseSection = $"/{settings.DatabaseName}";
			}

			var queryParams = new Dictionary<string, object> {{"authSource", "admin"}};
			string queryString = $"?{string.Join('&', queryParams.Select(x => $"{x.Key}={x.Value}"))}";

			return $"mongodb://{credentialsSection}{serverSection}{databaseSection}{queryString}";
		}

		#endregion
	}
}