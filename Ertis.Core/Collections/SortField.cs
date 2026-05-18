using System.Text.Json.Serialization;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Ertis.Core.Collections;

public class SortField
{
	#region Properties
	
	[JsonPropertyName("orderBy")]
	public string? OrderBy { get; set; }
	
	[JsonPropertyName("sortDirection")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public SortDirection? SortDirection { get; set; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Parameterless Constructor
	/// </summary>
	// ReSharper disable once UnusedMember.Global
	public SortField()
	{
		// NOP (For serialization)
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="orderBy"></param>
	/// <param name="sortDirection"></param>
	public SortField(string? orderBy = null, SortDirection? sortDirection = null)
	{
		this.OrderBy = orderBy;
		this.SortDirection = sortDirection;
	}
	
	#endregion
}