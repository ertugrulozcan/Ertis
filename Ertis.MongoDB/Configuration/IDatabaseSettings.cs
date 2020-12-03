namespace Ertis.MongoDB.Configuration
{
	public interface IDatabaseSettings
	{
		#region Properties

		string Username { get; }
		
		string Password { get; }
		
		string Host { get; }
		
		int Port { get; }
		
		string DatabaseName { get; }

		#endregion
	}
}