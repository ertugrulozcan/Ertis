using System;
using System.Linq;

namespace Ertis.Core.Dynamics
{
	public interface IDynamicObjectField
	{
		DynamicObjectFieldType Type { get; }
		
		string Name { get; }
		
		bool IsRequired { get; }
		
		dynamic Default { get; }
	}

	public abstract class DynamicObjectField : IDynamicObjectField
	{
		#region Properties

		public abstract DynamicObjectFieldType Type { get; }
		
		public string Name { get; }

		public bool IsRequired { get; }
		
		public dynamic Default { get; protected set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		protected DynamicObjectField(string name, bool isRequired = false)
		{
			this.Name = name;
			this.IsRequired = isRequired;
		}

		#endregion

		#region Methods

		public static DynamicObjectFieldType ParseFieldType(string fieldTypeString)
		{
			if (string.IsNullOrEmpty(fieldTypeString))
			{
				throw new Exception("type is required for dynamic object field declaration!");
			}
			
			var fieldType = fieldTypeString switch
			{
				"object" => DynamicObjectFieldType.Object, 
				"array" => DynamicObjectFieldType.Array,
				"string" => DynamicObjectFieldType.String,
				"integer" => DynamicObjectFieldType.Integer,
				"double" => DynamicObjectFieldType.Double,
				"boolean" => DynamicObjectFieldType.Boolean,
				"date" => DynamicObjectFieldType.Date,
				_ => throw new Exception($"Unknown field type! The type must be one of them [{string.Join(", ", Enum.GetNames(typeof(DynamicObjectFieldType)).Select(x => x.ToLower()))}]")
			};

			return fieldType;
		}

		#endregion
	}
	
	public abstract class DynamicObjectField<T> : DynamicObjectField
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		/// <param name="defaultValue"></param>
		protected DynamicObjectField(string name, bool isRequired = false, object defaultValue = null) : base(name, isRequired)
		{
			if (defaultValue != null)
			{
				this.Default = defaultValue is T value ? value : default;
			}
		}

		#endregion
		
		#region Abstract Methods

		//protected abstract bool ValidateValue(T value);

		#endregion
	}
}