using System;

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
}