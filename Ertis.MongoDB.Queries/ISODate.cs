using System;

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
        return $"ISODate(\"{this.Date:yyyy-MM-ddTHH:mm:ssZ}\")";
    }

    #endregion
}