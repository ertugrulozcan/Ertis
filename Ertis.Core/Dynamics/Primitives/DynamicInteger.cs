namespace Ertis.Core.Dynamics.Primitives
{
	public class DynamicInteger : DynamicObjectField<int>
	{
		#region Properties

		public override DynamicObjectFieldType Type => DynamicObjectFieldType.Integer;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		public DynamicInteger(string name, bool isRequired = false, object defaultValue = null) : base(name, isRequired, defaultValue)
		{
			
		}

		#endregion
	}
}