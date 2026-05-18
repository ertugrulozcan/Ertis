// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Ertis.Data.Models;

// ReSharper disable once UnusedType.Global
public interface IUpsertOptions
{
	#region Properties
	
	bool TriggerBeforeActionBinder { get; set; }
	
	bool TriggerAfterActionBinder { get; set; }
	
	#endregion
}