// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.PostgreSQL.Configuration;

public class DatabaseSettings : IDatabaseSettings
{
	#region Properties
	
	public string? ConnectionString { get; set; }
	
	#endregion
}