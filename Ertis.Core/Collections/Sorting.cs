using System.Collections;

// ReSharper disable UnusedMember.Global
namespace Ertis.Core.Collections;

// ReSharper disable once UnusedType.Global
public class Sorting : ICollection<SortField>
{
	#region Properties
	
	private List<SortField> Fields { get; } = new();
	
	public int Count => this.Fields.Count;
	
	public bool IsReadOnly => false;
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor with multiple fields
	/// </summary>
	/// <param name="fields"></param>
	public Sorting(IEnumerable<SortField> fields)
	{
		this.Fields = new List<SortField>(fields);
	}
	
	/// <summary>
	/// Constructor with single field
	/// </summary>
	/// <param name="sortField"></param>
	// ReSharper disable once MemberCanBePrivate.Global
	public Sorting(SortField sortField)
	{
		this.Fields = new List<SortField>
		{
			sortField
		};
	}
	
	/// <summary>
	/// Constructor with props
	/// </summary>
	/// <param name="orderBy"></param>
	/// <param name="sortDirection"></param>
	public Sorting(string? orderBy = null, SortDirection? sortDirection = null)
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