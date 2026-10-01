using System.Text.Json;
using Ertis.Core.Collections;

namespace Ertis.Core.Tests.Collections;

public class SortingTests
{
	#region Methods
	
	[Fact]
	public void Constructors_CreateTheFields()
	{
		var fields = new[] { new SortField("a"), new SortField("b", SortDirection.Descending) };
		
		Assert.Equal(["a", "b"], new Sorting(fields).Select(x => x.OrderBy));
		Assert.Equal("a", new Sorting(new SortField("a"))[0].OrderBy);
		Assert.Equal(SortDirection.Descending, new Sorting("a", SortDirection.Descending)[0].SortDirection);
		Assert.Empty(new Sorting(string.Empty));
		Sorting implicitSorting = new SortField("c");
		Assert.Equal("c", Assert.Single(implicitSorting).OrderBy);
	}
	
	[Fact]
	public void CollectionMethods_ChangeTheFields()
	{
		var field = new SortField("a");
		var sorting = new Sorting(field)
		{
			new SortField("b")
		};
		
		Assert.Equal(2, sorting.Count);
		Assert.False(sorting.IsReadOnly);
		Assert.Contains(field, sorting);
		
		var array = new SortField[2];
		sorting.CopyTo(array, 0);
		Assert.Same(field, array[0]);
		Assert.True(sorting.Remove(field));
		Assert.Equal("b", Assert.Single(sorting).OrderBy);
		
		sorting.Clear();
		Assert.Empty(sorting);
		Assert.Empty(sorting);
	}
	
	[Fact]
	public void SortField_IsSerializedWithTheEnumName()
	{
		var json = JsonSerializer.Serialize(new SortField("name", SortDirection.Descending));
		
		Assert.Equal("""{"orderBy":"name","sortDirection":"Descending"}""", json);
		Assert.Equal(SortDirection.Ascending, JsonSerializer.Deserialize<SortField>("""{"orderBy":"a","sortDirection":"Ascending"}""")!.SortDirection);
	}
	
	[Fact]
	public void PaginationCollection_IsSerializedWithTheJsonNames()
	{
		var json = JsonSerializer.Serialize<IPaginationCollection<int>>(new PaginationCollection<int> { Count = 2, Items = [1, 2] });
		
		Assert.Equal("""{"count":2,"items":[1,2]}""", json);
	}
	
	#endregion
}
