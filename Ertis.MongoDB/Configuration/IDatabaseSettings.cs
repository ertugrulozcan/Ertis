// ReSharper disable UnusedMember.Global
namespace Ertis.MongoDB.Configuration;

// ReSharper disable once UnusedType.Global
public interface IDatabaseSettings
{
	#region Properties
	
	string? ConnectionString { get; }
	
	string? DefaultAuthDatabase { get; }
	
	bool? AllowDiskUse { get; }
	
	#endregion
}