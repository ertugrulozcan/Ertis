using Ertis.Data.Models;

namespace Ertis.Data.Tests.Models;

public class UpsertOptionsTests
{
	#region Methods
	
	[Fact]
	public void Defaults_TriggerTheActionBinders()
	{
		UpsertOptions[] options = [InsertOptions.Default, UpdateOptions.Default];
		
		Assert.All(options, x =>
		{
			Assert.True(x.TriggerBeforeActionBinder);
			Assert.True(x.TriggerAfterActionBinder);
		});
	}
	
	[Fact]
	public void NewOptions_DoNotTriggerTheActionBinders()
	{
		UpsertOptions[] options = [new InsertOptions(), new UpdateOptions()];
		
		Assert.All(options, x =>
		{
			Assert.False(x.TriggerBeforeActionBinder);
			Assert.False(x.TriggerAfterActionBinder);
		});
	}
	
	#endregion
}
