namespace Ertis.Data.Models;

public interface UpsertOptions
{
	#region Properties

	bool TriggerBeforeActionBinder { get; set; }
	
	bool TriggerAfterActionBinder { get; set; }

	#endregion
}

public struct InsertOptions : UpsertOptions
{
	#region Properties

	public bool TriggerBeforeActionBinder { get; set; }
	
	public bool TriggerAfterActionBinder { get; set; }

	#endregion

	#region Statics

	public static readonly InsertOptions Default = new()
	{
		TriggerBeforeActionBinder = true,
		TriggerAfterActionBinder = true
	};

	#endregion
}

public struct UpdateOptions : UpsertOptions
{
	#region Properties

	public bool TriggerBeforeActionBinder { get; set; }
	
	public bool TriggerAfterActionBinder { get; set; }

	#endregion

	#region Statics

	public static readonly UpdateOptions Default = new()
	{
		TriggerBeforeActionBinder = true,
		TriggerAfterActionBinder = true
	};

	#endregion
}