// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ertis.MongoDB.Configuration;

// ReSharper disable once UnusedType.Global
public class DatabaseSettings : IDatabaseSettings
{
	#region Properties
	
	public string? ConnectionString { get; set; }
	
	public string? DefaultAuthDatabase { get; set; }
	
	public bool? AllowDiskUse { get; set; }
	
	#endregion
}