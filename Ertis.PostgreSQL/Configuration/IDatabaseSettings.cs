// ReSharper disable UnusedMember.Global
namespace Ertis.PostgreSQL.Configuration;

public interface IDatabaseSettings
{
	#region Properties
	
	string? ConnectionString { get; }
	
	#endregion
}