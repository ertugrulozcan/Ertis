namespace Ertis.Extensions.AspNetCore.Models.Response
{
	public interface IHasErrorModel
	{
		ErrorModel Error { get; }
	}
}