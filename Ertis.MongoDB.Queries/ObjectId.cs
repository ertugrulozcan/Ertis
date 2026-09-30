namespace Ertis.MongoDB.Queries;

public class ObjectId : QueryValue<string>, IQueryExpression
{
	#region Properties
	
	private string Id { get; }
	
	public string Field => "_id";
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="id">ObjectId</param>
	public ObjectId(string id) : base(id)
	{
		if (!IsValid(id))
		{
			throw new FormatException($"'{id}' is not a valid 24 digit hex string.");
		}
		
		this.Id = id;
	}
	
	#endregion
	
	#region Methods
	
	public override string ToString()
	{
		return $"ObjectId(\"{this.Id}\")";
	}
	
	/// <summary>
	/// An object id is 24 hexadecimal digits (it is written into the query as it is)
	/// </summary>
	private static bool IsValid(string? id)
	{
		return id is { Length: 24 } && id.All(char.IsAsciiHexDigit);
	}
	
	#endregion
}