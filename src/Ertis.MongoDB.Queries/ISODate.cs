using System.Globalization;

namespace Ertis.MongoDB.Queries;

public class ISODate : QueryValue<DateTime>
{
	#region Properties
	
	private DateTime Date { get; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="date">Date</param>
	public ISODate(DateTime date) : base(date)
	{
		this.Date = date;
	}
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		var date = QueryHelper.ToUniversalTime(this.Date);
		var format = date.Millisecond == 0 ? "yyyy-MM-ddTHH:mm:ssZ" : "yyyy-MM-ddTHH:mm:ss.fffZ";
		// Extended JSON: a strict json, which MongoDB reads as a date
		return "{ \"$date\": \"" + date.ToString(format, CultureInfo.InvariantCulture) + "\" }";
	}
	
	#endregion
}