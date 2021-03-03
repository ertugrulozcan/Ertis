namespace Ertis.Core.Dynamics.Primitives
{
	public class DynamicBoolean : DynamicObjectField<bool>
	{
		#region Properties

		public override DynamicObjectFieldType Type => DynamicObjectFieldType.Boolean;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		public DynamicBoolean(string name, bool isRequired = false, object defaultValue = null) : base(name, isRequired, defaultValue)
		{
			
		}

		#endregion
	}
}