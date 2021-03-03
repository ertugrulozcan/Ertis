namespace Ertis.Core.Dynamics.Primitives
{
	public class DynamicDouble : DynamicObjectField<double>
	{
		#region Properties

		public override DynamicObjectFieldType Type => DynamicObjectFieldType.Double;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		public DynamicDouble(string name, bool isRequired = false, object defaultValue = null) : base(name, isRequired, defaultValue)
		{
			
		}

		#endregion
	}
}