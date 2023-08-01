using System;
using MongoDB.Driver;

namespace Ertis.MongoDB.Configuration;

public interface IClientSettings
{
	#region Properties

	TimeSpan? MaxConnectionLifeTime { get; }
	
	TimeSpan? SocketTimeout { get; }
	
	TimeSpan? MaxConnectionIdleTime { get; }
	
	TimeSpan? ConnectTimeout { get; }
	
	TimeSpan? ServerSelectionTimeout { get; }
	
	TimeSpan? HeartbeatInterval { get; }
	
	TimeSpan? HeartbeatTimeout { get; }
	
	int? MinConnectionPoolSize { get; }
	
	int? MaxConnectionPoolSize { get; }
	
	int? MaxConnecting { get; }
	
	#endregion
}

public class ClientSettings : IClientSettings
{
	#region Properties

	public TimeSpan? MaxConnectionLifeTime { get; set; }
	
	public TimeSpan? SocketTimeout { get; set; }
	
	public TimeSpan? MaxConnectionIdleTime { get; set; }
	
	public TimeSpan? ConnectTimeout { get; set; }
	
	public TimeSpan? ServerSelectionTimeout { get; set; }
	
	public TimeSpan? HeartbeatInterval { get; set; }
	
	public TimeSpan? HeartbeatTimeout { get; set; }
	
	public int? MinConnectionPoolSize { get; set; }
	
	public int? MaxConnectionPoolSize { get; set; }
	
	public int? MaxConnecting { get; set; }
	
	#endregion
	
	#region Methods

	public static IClientSettings FromClientOptions(IClientOptions options)
	{
		return new ClientSettings
		{
			MaxConnectionLifeTime = options.MaxConnectionLifeTime != null
				? TimeSpan.FromSeconds(options.MaxConnectionLifeTime.Value)
				: null,
			SocketTimeout = options.SocketTimeout != null ? TimeSpan.FromSeconds(options.SocketTimeout.Value) : null,
			MaxConnectionIdleTime = options.MaxConnectionIdleTime != null
				? TimeSpan.FromSeconds(options.MaxConnectionIdleTime.Value)
				: null,
			ConnectTimeout = options.ConnectTimeout != null ? TimeSpan.FromSeconds(options.ConnectTimeout.Value) : null,
			ServerSelectionTimeout = options.ServerSelectionTimeout != null
				? TimeSpan.FromSeconds(options.ServerSelectionTimeout.Value)
				: null,
			HeartbeatInterval = options.HeartbeatInterval != null
				? TimeSpan.FromSeconds(options.HeartbeatInterval.Value)
				: null,
			HeartbeatTimeout = options.HeartbeatTimeout != null
				? TimeSpan.FromSeconds(options.HeartbeatTimeout.Value)
				: null,
			MinConnectionPoolSize = options.MinConnectionPoolSize,
			MaxConnectionPoolSize = options.MaxConnectionPoolSize,
			MaxConnecting = options.MaxConnecting,
		};
	}

	internal static MongoClientSettings GetMongoClientSettings(IClientSettings clientSettings, string connectionString)
	{
		PrintClientSettingsLogs(clientSettings);
		
		if (clientSettings != null)
		{
			var mongoClientSettings = MongoClientSettings.FromUrl(MongoUrl.Create(connectionString));
			
			if (clientSettings.MaxConnectionLifeTime != null)
			{
				mongoClientSettings.MaxConnectionLifeTime = clientSettings.MaxConnectionLifeTime.Value;
			}
			
			if (clientSettings.SocketTimeout != null)
			{
				mongoClientSettings.SocketTimeout = clientSettings.SocketTimeout.Value;
			}
			
			if (clientSettings.MaxConnectionIdleTime != null)
			{
				mongoClientSettings.MaxConnectionIdleTime = clientSettings.MaxConnectionIdleTime.Value;
			}
			
			if (clientSettings.ConnectTimeout != null)
			{
				mongoClientSettings.ConnectTimeout = clientSettings.ConnectTimeout.Value;
			}
			
			if (clientSettings.ServerSelectionTimeout != null)
			{
				mongoClientSettings.ServerSelectionTimeout = clientSettings.ServerSelectionTimeout.Value;
			}
			
			if (clientSettings.HeartbeatInterval != null)
			{
				mongoClientSettings.HeartbeatInterval = clientSettings.HeartbeatInterval.Value;
			}
			
			if (clientSettings.HeartbeatTimeout != null)
			{
				mongoClientSettings.HeartbeatTimeout = clientSettings.HeartbeatTimeout.Value;
			}
			
			if (clientSettings.MinConnectionPoolSize != null)
			{
				mongoClientSettings.MinConnectionPoolSize = clientSettings.MinConnectionPoolSize.Value;
			}
			
			if (clientSettings.MaxConnectionPoolSize != null)
			{
				mongoClientSettings.MaxConnectionPoolSize = clientSettings.MaxConnectionPoolSize.Value;
			}
			
			if (clientSettings.MaxConnecting != null)
			{
				mongoClientSettings.MaxConnecting = clientSettings.MaxConnecting.Value;
			}

			return mongoClientSettings;
		}

		return null;
	}

	private static void PrintClientSettingsLogs(IClientSettings clientSettings)
	{
		if (clientSettings == null)
		{
			Console.WriteLine("Default client settings using.");
			return;
		}
		
		if (clientSettings.MaxConnectionLifeTime != null)
		{
			Console.WriteLine($"MaxConnectionLifeTime: {clientSettings.MaxConnectionLifeTime.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.SocketTimeout != null)
		{
			Console.WriteLine($"SocketTimeout: {clientSettings.SocketTimeout.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.MaxConnectionIdleTime != null)
		{
			Console.WriteLine($"MaxConnectionIdleTime: {clientSettings.MaxConnectionIdleTime.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.ConnectTimeout != null)
		{
			Console.WriteLine($"ConnectTimeout: {clientSettings.ConnectTimeout.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.ServerSelectionTimeout != null)
		{
			Console.WriteLine($"ServerSelectionTimeout: {clientSettings.ServerSelectionTimeout.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.HeartbeatInterval != null)
		{
			Console.WriteLine($"HeartbeatInterval: {clientSettings.HeartbeatInterval.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.HeartbeatTimeout != null)
		{
			Console.WriteLine($"HeartbeatTimeout: {clientSettings.HeartbeatTimeout.Value:hh\\:mm\\:ss}");
		}
			
		if (clientSettings.MinConnectionPoolSize != null)
		{
			Console.WriteLine($"MinConnectionPoolSize: {clientSettings.MinConnectionPoolSize.Value}");
		}
			
		if (clientSettings.MaxConnectionPoolSize != null)
		{
			Console.WriteLine($"MaxConnectionPoolSize: {clientSettings.MaxConnectionPoolSize.Value}");
		}
			
		if (clientSettings.MaxConnecting != null)
		{
			Console.WriteLine($"MaxConnecting: {clientSettings.MaxConnecting.Value}");
		}
	}

	#endregion
}