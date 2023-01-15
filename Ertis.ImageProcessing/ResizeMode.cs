using ResizeModeEnum = SixLabors.ImageSharp.Processing.ResizeMode;
namespace Ertis.ImageProcessing;

// ReSharper disable MemberCanBePrivate.Global
public class ResizeMode
{
	#region Statics

	public static readonly ResizeMode Crop = new(ResizeModeEnum.Crop);
	public static readonly ResizeMode Manual = new(ResizeModeEnum.Manual);
	public static readonly ResizeMode Max = new(ResizeModeEnum.Max);
	public static readonly ResizeMode Min = new(ResizeModeEnum.Min);
	public static readonly ResizeMode Stretch = new(ResizeModeEnum.Stretch);
	public static readonly ResizeMode Pad = new(ResizeModeEnum.Pad);
	public static readonly ResizeMode BoxPad = new(ResizeModeEnum.BoxPad);

	#endregion
	
	#region Properties

	private ResizeModeEnum Value { get; }

	#endregion
	
	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value"></param>
	private ResizeMode(ResizeModeEnum value)
	{
		this.Value = value;
	}

	#endregion

	#region Operators

	public static implicit operator ResizeModeEnum(ResizeMode d) => d.Value;

	#endregion

	#region Methods

	public static ResizeMode? Parse(string key)
	{
		if (Enum.TryParse<ResizeModeEnum>(key, true, out var enumValue))
		{
			return enumValue switch
			{
				ResizeModeEnum.Crop => Crop,
				ResizeModeEnum.Pad => Pad,
				ResizeModeEnum.BoxPad => BoxPad,
				ResizeModeEnum.Max => Max,
				ResizeModeEnum.Min => Min,
				ResizeModeEnum.Stretch => Stretch,
				ResizeModeEnum.Manual => Manual,
				_ => null
			};
		}

		return null;
	}

	#endregion
}