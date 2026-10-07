namespace Ertis.Schema.Validation;

/// <summary>
/// Tracks the path of the value being validated in the current validation flow.
/// The schemas are shared (e.g. cached and used by concurrent validations), so the validation keeps its state here instead of the field infos.
/// </summary>
internal static class ValidationPath
{
	#region Fields
	
	private static readonly AsyncLocal<List<string>?> Segments = new();
	
	#endregion
	
	#region Properties
	
	/// <summary>
	/// The path of the value being validated, or null when there is no validation in progress
	/// </summary>
	internal static string? Current
	{
		get
		{
			var segments = Segments.Value;
			if (segments == null || segments.Count == 0)
			{
				return null;
			}
			
			return string.Concat(segments.Select((segment, index) => index == 0 || segment.StartsWith('[') ? segment : $".{segment}"));
		}
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Starts a new validation flow on the root path
	/// </summary>
	internal static Scope Begin(string rootPath)
	{
		var previous = Segments.Value;
		Segments.Value = [rootPath];
		return new Scope(previous, isRoot: true);
	}
	
	/// <summary>
	/// Moves into a property of the current object
	/// </summary>
	internal static Scope Push(string propertyName)
	{
		var segments = Segments.Value;
		if (segments == null)
		{
			return default;
		}
		
		segments.Add(propertyName);
		return new Scope(segments, isRoot: false);
	}
	
	/// <summary>
	/// Moves into an item of the current array
	/// </summary>
	internal static Scope PushIndex(int index)
	{
		return Push($"[{index}]");
	}
	
	#endregion
	
	#region Scope
	
	internal readonly struct Scope : IDisposable
	{
		#region Fields
		
		private readonly List<string>? _segments;
		private readonly bool _isRoot;
		
		#endregion
		
		#region Constructors
		
		internal Scope(List<string>? segments, bool isRoot)
		{
			this._segments = segments;
			this._isRoot = isRoot;
		}
		
		#endregion
		
		#region Methods
		
		public void Dispose()
		{
			if (this._isRoot)
			{
				// Restores the previous flow (null when the validation was not nested)
				Segments.Value = this._segments;
			}
			else if (this._segments is { Count: > 0 })
			{
				this._segments.RemoveAt(this._segments.Count - 1);
			}
		}
		
		#endregion
	}
	
	#endregion
}
