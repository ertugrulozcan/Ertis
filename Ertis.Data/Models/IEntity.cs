namespace Ertis.Data.Models;

// ReSharper disable once UnusedType.Global
public interface IEntity<out TIdentifier> where TIdentifier : notnull
{
	#region Properties
	
	// ReSharper disable once UnusedMember.Global
	TIdentifier Id { get; }
	
	#endregion
}