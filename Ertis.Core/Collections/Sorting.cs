using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Ertis.Core.Collections;

public class Sorting : ICollection<SortField>
{
	#region Properties

	private List<SortField> Fields { get; }

	public int Count => this.Fields.Count;

	public bool IsReadOnly => false;

	#endregion

	#region Constructors

	public Sorting()
	{
		this.Fields = new List<SortField>();
	}
	
	public Sorting(IEnumerable<SortField> fields)
	{
		this.Fields = new List<SortField>(fields);
	}
	
	// ReSharper disable once MemberCanBePrivate.Global
	public Sorting(SortField sortField)
	{
		this.Fields = new List<SortField>
		{
			sortField
		};
	}
	
	public Sorting(string orderBy = null, SortDirection? sortDirection = null)
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

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class SortField
{
	#region Properties

	public string OrderBy { get; set; }
	
	public SortDirection? SortDirection { get; set; }

	#endregion
	
	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="orderBy"></param>
	/// <param name="sortDirection"></param>
	public SortField(string orderBy = null, SortDirection? sortDirection = null)
	{
		this.OrderBy = orderBy;
		this.SortDirection = sortDirection;
	}

	#endregion
}