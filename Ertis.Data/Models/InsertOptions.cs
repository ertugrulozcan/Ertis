namespace Ertis.Data.Models;

// ReSharper disable once UnusedType.Global
public struct InsertOptions : IUpsertOptions
{
	#region Properties
	
	public bool TriggerBeforeActionBinder { get; set; }
	
	public bool TriggerAfterActionBinder { get; set; }
	
	#endregion
	
	#region Statics
	
	// ReSharper disable once UnusedMember.Global
	public static readonly InsertOptions Default = new()
	{
		TriggerBeforeActionBinder = true,
		TriggerAfterActionBinder = true
	};
	
	#endregion
}