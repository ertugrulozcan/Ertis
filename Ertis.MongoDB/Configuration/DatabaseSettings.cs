namespace Ertis.MongoDB.Configuration
{
	public class DatabaseSettings : IDatabaseSettings
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