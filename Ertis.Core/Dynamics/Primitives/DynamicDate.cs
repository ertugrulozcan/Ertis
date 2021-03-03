using System;

namespace Ertis.Core.Dynamics.Primitives
{
	public class DynamicDate : DynamicObjectField<DateTime>
	{
		#region Properties

		public override DynamicObjectFieldType Type => DynamicObjectFieldType.Date;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		public DynamicDate(string name, bool isRequired = false, object defaultValue = null) : base(name, isRequired, defaultValue)
		{
			
		}

		#endregion
	}
}