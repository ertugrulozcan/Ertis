using System.Collections;
using Ertis.Net.Http;
using Ertis.Net.Tests.TestHelpers;

namespace Ertis.Net.Tests.Http;

public class CollectionTests
{
	#region Query String Methods
	
	[Fact]
	public void QueryString_BuildsTheQuery()
	{
		var queryString = QueryString.Create().Add("a", 1).Add(new KeyValuePair<string, object>("b", "x y")).Add("a", 2);
		
		Assert.Equal("a=2&b=x%20y", queryString.ToString());
		Assert.True(queryString.ContainsKey("b"));
		Assert.Equal(2, queryString.ToDictionary().Count);
		Assert.Equal([2, (object) "x y"], queryString);
		Assert.Equal("b=x%20y", queryString.Remove("a").ToString());
		Assert.Single(((IEnumerable) queryString).Cast<KeyValuePair<string, object>>());
	}
	
	[Fact]
	public void QueryString_Empty_IsNotChanged()
	{
		var empty = QueryString.Empty;
		
		var added = empty.Add("a", 1);
		
		Assert.Empty(QueryString.Empty);
		Assert.Equal("a=1", added.ToString());
		Assert.Equal("a=1&b=2", ((HttpQueryString) QueryString.Add(QueryString.Add("a", 1))).Add(QueryString.Add("b", 2)).ToString());
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void QueryString_WithAnEmptyKeyOrValue_Throws(string? text)
	{
		Assert.Throws<ArgumentException>(() => QueryString.Create().Add(text!, "v"));
		Assert.Throws<ArgumentException>(() => QueryString.Create().Add("k", text!));
	}
	
	[Fact]
	public void QueryString_WritesTheValuesWithTheInvariantCulture()
	{
		using var _ = new CultureScope("tr-TR");
		
		Assert.Equal("ratio=1.5&date=01%2F31%2F2026%2010%3A00%3A00", QueryString.Add("ratio", 1.5).Add("date", new DateTime(2026, 1, 31, 10, 0, 0)).ToString());
	}
	
	[Fact]
	public void QueryString_EscapesTheKeys()
	{
		Assert.Equal("a%20b%26c=1", QueryString.Add("a b&c", 1).ToString());
	}
	
	#endregion
	
	#region Header Methods
	
	[Fact]
	public void HeaderCollection_CollectsTheHeaders()
	{
		var headers = HeaderCollection.Create().Add("A", 1).Add(new KeyValuePair<string, object>("B", "x")).Add("A", 2);
		
		Assert.Equal(["A", "B"], headers.Keys);
		Assert.Equal([2, (object) "x"], headers.Values);
		Assert.True(headers.ContainsKey("B"));
		Assert.Equal(["x"], headers.Remove("A").Values);
		Assert.Equal("B=x", headers.ToString());
		Assert.Single(((IEnumerable) headers).Cast<KeyValuePair<string, object>>());
		Assert.Equal(["x"], (IEnumerable<object>) headers);
	}
	
	[Fact]
	public void HeaderCollection_Empty_IsNotChanged()
	{
		var added = HeaderCollection.Empty.Add("A", 1);
		
		Assert.Empty(HeaderCollection.Empty.Keys);
		Assert.Equal(["A"], added.Keys);
		Assert.Equal(["A", "B"], ((RequestHeaders) HeaderCollection.Add(HeaderCollection.Add("A", 1))).Add(HeaderCollection.Add("B", 2)).Keys);
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void HeaderCollection_WithAnEmptyKeyOrValue_Throws(string? text)
	{
		Assert.Throws<ArgumentException>(() => HeaderCollection.Create().Add(text!, "v"));
		Assert.Throws<ArgumentException>(() => HeaderCollection.Create().Add("k", text!));
	}
	
	#endregion
}
