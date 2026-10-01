// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.MongoDB.Configuration;

public interface IDatabaseSettings
{
	#region Properties
	
	string? ConnectionString { get; }
	
	string? DefaultAuthDatabase { get; }
	
	bool? AllowDiskUse { get; }
	
	#endregion
}