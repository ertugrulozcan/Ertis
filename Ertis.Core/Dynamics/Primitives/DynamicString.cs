namespace Ertis.Core.Dynamics.Primitives
{
	public class DynamicString : DynamicObjectField<string>
	{
		#region Properties

		public override DynamicObjectFieldType Type => DynamicObjectFieldType.String;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		public DynamicString(string name, bool isRequired = false, object defaultValue = null) : base(name, isRequired, defaultValue)
		{
			
		}

		#endregion
	}
}