using System;

namespace Ertis.Core.Dynamics.Primitives
{
	public class DynamicObject : DynamicObjectField<dynamic>
	{
		#region Properties

		public override DynamicObjectFieldType Type => DynamicObjectFieldType.Object;

		public DynamicObjectSchema Schema { get; }
		
		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="schema"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		public DynamicObject(string name, DynamicObjectSchema schema, bool isRequired = false, object defaultValue = null) : base(name, isRequired, defaultValue)
		{
			this.Schema = schema ?? throw new NullReferenceException($"The schema is required for '{name}' field");
		}

		#endregion
	}
}