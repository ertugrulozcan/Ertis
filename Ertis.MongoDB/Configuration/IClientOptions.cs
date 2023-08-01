namespace Ertis.MongoDB.Configuration;

public interface IClientOptions
{
	#region Properties

	int? MaxConnectionLifeTime { get; }
	
	int? SocketTimeout { get; }
	
	int? MaxConnectionIdleTime { get; }
	
	int? ConnectTimeout { get; }
	
	int? ServerSelectionTimeout { get; }
	
	int? HeartbeatInterval { get; }
	
	int? HeartbeatTimeout { get; }
	
	int? MinConnectionPoolSize { get; }
	
	int? MaxConnectionPoolSize { get; }
	
	int? MaxConnecting { get; }
	
	#endregion
}

public class ClientOptions : IClientOptions
{
	#region Properties

	public int? MaxConnectionLifeTime { get; set; }
	
	public int? SocketTimeout { get; set; }
	
	public int? MaxConnectionIdleTime { get; set; }
	
	public int? ConnectTimeout { get; set; }
	
	public int? ServerSelectionTimeout { get; set; }
	
	public int? HeartbeatInterval { get; set; }
	
	public int? HeartbeatTimeout { get; set; }
	
	public int? MinConnectionPoolSize { get; set; }
	
	public int? MaxConnectionPoolSize { get; set; }
	
	public int? MaxConnecting { get; set; }
	
	#endregion
}