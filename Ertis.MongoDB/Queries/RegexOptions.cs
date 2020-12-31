namespace Ertis.MongoDB.Queries
{
	public class RegexOptions
	{
		#region Properties

		/// <summary>
		/// The 'i' option
		/// </summary>
		public bool IsCaseSensitive { get; set; } = true;
		
		/// <summary>
		/// The 'm' option
		/// </summary>
		public bool MatchAnchors { get; set; }
		
		/// <summary>
		/// The 'x' option
		/// </summary>
		public bool IgnoreWhiteSpace { get; set; }

		/// <summary>
		/// The 's' option
		/// </summary>
		public bool IncludeAllCharacters { get; set; }
		
		#endregion

		#region Methods

		public override string ToString()
		{
			var options = string.Empty;
			
			if (!this.IsCaseSensitive)
				options += "i";
			if (this.MatchAnchors)
				options += "m";
			if (this.IgnoreWhiteSpace)
				options += "x";
			if (this.IncludeAllCharacters)
				options += "s";
			
			return options;
		}

		#endregion
	}
}