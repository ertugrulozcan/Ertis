using System.Globalization;

namespace Ertis.TemplateEngine.Tests.TestHelpers;

/// <summary>
/// Switches the current culture of the thread until disposed
/// </summary>
public sealed class CultureScope : IDisposable
{
	#region Fields
	
	private readonly CultureInfo _previousCulture;
	private readonly CultureInfo _previousUICulture;
	
	#endregion
	
	#region Constructors
	
	public CultureScope(string cultureName)
	{
		this._previousCulture = CultureInfo.CurrentCulture;
		this._previousUICulture = CultureInfo.CurrentUICulture;
		
		var culture = CultureInfo.GetCultureInfo(cultureName);
		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = culture;
	}
	
	#endregion
	
	#region Methods
	
	public void Dispose()
	{
		CultureInfo.CurrentCulture = this._previousCulture;
		CultureInfo.CurrentUICulture = this._previousUICulture;
	}
	
	#endregion
}
