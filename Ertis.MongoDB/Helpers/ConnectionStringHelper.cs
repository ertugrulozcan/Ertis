using System;
using System.Collections.Generic;
using System.Linq;
using Ertis.MongoDB.Configuration;
using MongoDB.Driver;

namespace Ertis.MongoDB.Helpers
{
	public static class ConnectionStringHelper
	{
		#region Methods

		public static string GenerateConnectionString(IDatabaseSettings settings, bool setAuthSourceAsAdmin = true)
		{
			// mongodb://[username:password@]host1[:port1][,...hostN[:portN]][/[database_name][?options]]

			var scheme = string.IsNullOrEmpty(settings.Scheme) ? "mongodb" : settings.Scheme;
			
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
			if (!string.IsNullOrEmpty(settings.DefaultAuthDatabase))
			{
				databaseSection = $"/{settings.DefaultAuthDatabase}";
			}

			var queryParams = new Dictionary<string, object>();
			if (setAuthSourceAsAdmin)
			{
				queryParams.Add("authSource", "admin");
			}

			if (settings.QueryString != null)
			{
				foreach (var (key, value) in settings.QueryString)
				{
					queryParams.Add(key, value);	
				}
			}

			string queryString = null;
			if (queryParams.Any())
			{
				queryString = $"?{string.Join('&', queryParams.Select(x => $"{x.Key}={x.Value}"))}";
			}
			
			return $"{scheme}://{credentialsSection}{serverSection}{databaseSection}{queryString}";
		}

		public static IDatabaseSettings ParseConnectionString(string connectionString)
		{
			// mongodb://[username:password@]host1[:port1][,...hostN[:portN]][/[database_name][?options]]
            
            if (string.IsNullOrEmpty(connectionString))
            {
            	throw new ArgumentNullException();
            }

			var databaseSettings = new DatabaseSettings();
			
            if (SplitBy(connectionString, "://", out var schemePart, out var withoutSchemePart))
            {
            	string scheme = schemePart;
				databaseSettings.Scheme = scheme;
            	
            	connectionString = withoutSchemePart;
            }
            else
            {
            	throw new MongoException("The scheme was not specified in connection string");
            }
    
            if (SplitBy(connectionString, "?", out var withoutQueryStringPart, out var queryStringPart))
            {
            	string queryString = queryStringPart;
				databaseSettings.QueryString = queryString.Split('&').ToDictionary(x => x.Split('=')[0], y => y.Split('=')[1] as object);
				
            	connectionString = withoutQueryStringPart;
            }
            
            if (SplitBy(connectionString, "@", out var authenticationCredentialsSegment, out var withoutAuthenticationPart))
            {
            	if (authenticationCredentialsSegment.Contains(":"))
            	{
            		var authenticationCredentials = authenticationCredentialsSegment.Split(':');
            		
					var username = authenticationCredentials[0];
					databaseSettings.Username = username;
					
            		var password = authenticationCredentials[1];
					databaseSettings.Password = password;
				}
            	
            	connectionString = withoutAuthenticationPart;
            }
            
            if (SplitBy(connectionString, "/", out var withoutAdminDbPart, out var adminDbPart))
            {
            	string adminDatabase = adminDbPart;
				databaseSettings.DefaultAuthDatabase = adminDatabase;
				
            	connectionString = withoutAdminDbPart;
            }
            
            if (connectionString.Contains(":"))
            {
            	var hostAndPort = connectionString.Split(':');
            	
				var host = hostAndPort[0];
				databaseSettings.Host = host;
				
            	var port = hostAndPort[1];
				databaseSettings.Port = int.Parse(port);
            }
            else
            {
            	var host = connectionString;
				databaseSettings.Host = host;
            }

			return databaseSettings;
		}

		private static bool SplitBy(string text, string splitBy, out string part1, out string part2)
		{
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(splitBy))
				throw new ArgumentNullException();
			
			if (text.Contains(splitBy))
			{
				int index = text.IndexOf(splitBy, StringComparison.Ordinal);
				part1 = text.Substring(0, index);
				part2 = text.Substring(index + splitBy.Length);
				return true;
			}
			else
			{
				part1 = text;
				part2 = null;
				return false;
			}
		}

		#endregion
	}
}