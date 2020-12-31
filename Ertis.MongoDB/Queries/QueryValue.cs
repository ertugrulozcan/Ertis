using System;

namespace Ertis.MongoDB.Queries
{
	public class QueryValue : IQueryable
	{
		#region Properties

		private object Value { get; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value"></param>
		public QueryValue(object value)
		{
			this.Value = value;
		}

		#endregion
		
		#region Implicit & Explicit Operators

		public static implicit operator QueryValue(string value)
		{
			return new QueryValue(value);
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			if (this.Value == null)
			{
				return null;
			}

			var type = this.Value.GetType();
			if (type == typeof(string))
			{
				return $"\"{this.Value}\"";
			}

			if (type == typeof(char))
			{
				return $"\"{this.Value}\"";
			}

			if (type == typeof(DateTime))
			{
				return $"\"{this.Value}\"";
			}

			if (type == typeof(bool))
			{
				return this.Value.ToString()?.ToLower();
			}
			
			return this.Value.ToString();
		}

		#endregion
	}
}