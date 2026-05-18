namespace Ertis.Data.Models;

// ReSharper disable once UnusedType.Global
public struct UpdateOptions : IUpsertOptions
{
	#region Properties
	
	public bool TriggerBeforeActionBinder { get; set; }
	
	public bool TriggerAfterActionBinder { get; set; }
	
	#endregion
	
	#region Statics
	
	// ReSharper disable once UnusedMember.Global
	public static readonly UpdateOptions Default = new()
	{
		TriggerBeforeActionBinder = true,
		TriggerAfterActionBinder = true
	};
	
	#endregion
}