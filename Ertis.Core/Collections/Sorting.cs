using System.Collections;
using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.Core.Collections;

public class Sorting : ICollection<SortField>
{
	#region Properties
	
	private List<SortField> Fields { get; } = new();
	
	public int Count => this.Fields.Count;
	
	public bool IsReadOnly => false;
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor 1
	/// </summary>
	/// <param name="fields"></param>
	public Sorting(IEnumerable<SortField> fields)
	{
		this.Fields = new List<SortField>(fields);
	}
	
	/// <summary>
	/// Constructor 2
	/// </summary>
	/// <param name="sortField"></param>
	public Sorting(SortField sortField)
	{
		this.Fields = new List<SortField>
		{
			sortField
		};
	}
	
	/// <summary>
	/// Constructor 3
	/// </summary>
	/// <param name="orderBy"></param>
	/// <param name="sortDirection"></param>
	public Sorting(string orderBy, SortDirection? sortDirection = null)
	{
		if (!string.IsNullOrEmpty(orderBy))
		{
			this.Fields = new List<SortField>
			{
				new (orderBy, sortDirection)
			};
		}
	}
	
	#endregion
	
	#region Operators
	
	public SortField this[int index] => this.Fields[index];
	
	public static implicit operator Sorting(SortField sortField) => new (sortField);
	
	#endregion
	
	#region Methods
	
	public IEnumerator<SortField> GetEnumerator()
	{
		return this.Fields.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
	public void Add(SortField item)
	{
		this.Fields.Add(item);
	}
	
	public void Clear()
	{
		this.Fields.Clear();
	}
	
	public bool Contains(SortField item)
	{
		return this.Fields.Contains(item);
	}
	
	public void CopyTo(SortField[] array, int arrayIndex)
	{
		this.Fields.CopyTo(array, arrayIndex);
	}
	
	public bool Remove(SortField item)
	{
		return this.Fields.Remove(item);
	}
	
	#endregion
}

public class SortField
{
	#region Properties
	
	[JsonPropertyName("orderBy")]
	[Newtonsoft.Json.JsonProperty("orderBy")]
	public string OrderBy { get; set; }
	
	[JsonPropertyName("sortDirection")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Newtonsoft.Json.JsonProperty("sortDirection")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public SortDirection? SortDirection { get; set; }
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="orderBy"></param>
	/// <param name="sortDirection"></param>
	public SortField(string orderBy, SortDirection? sortDirection = null)
	{
		this.OrderBy = orderBy;
		this.SortDirection = sortDirection;
	}
	
	#endregion
}