namespace Ertis.Data.Models
{
	public interface IEntity<out TIdentifier> where TIdentifier : notnull
	{
		#region Properties

		TIdentifier Id { get; }

		#endregion
	}
}