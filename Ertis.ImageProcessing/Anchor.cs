using AnchorEnum = SixLabors.ImageSharp.Processing.AnchorPositionMode;
namespace Ertis.ImageProcessing;

// ReSharper disable MemberCanBePrivate.Global
public class Anchor
{
	#region Statics

	public static readonly Anchor Center = new(AnchorEnum.Center);
	public static readonly Anchor Top = new(AnchorEnum.Top);
	public static readonly Anchor Bottom = new(AnchorEnum.Bottom);
	public static readonly Anchor Left = new(AnchorEnum.Left);
	public static readonly Anchor Right = new(AnchorEnum.Right);
	public static readonly Anchor TopLeft = new(AnchorEnum.TopLeft);
	public static readonly Anchor TopRight = new(AnchorEnum.TopRight);
	public static readonly Anchor BottomLeft = new(AnchorEnum.BottomLeft);
	public static readonly Anchor BottomRight = new(AnchorEnum.BottomRight);

	#endregion
	
	#region Properties

	private AnchorEnum Value { get; }

	#endregion
	
	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value"></param>
	private Anchor(AnchorEnum value)
	{
		this.Value = value;
	}

	#endregion
	
	#region Operators

	public static implicit operator AnchorEnum(Anchor d) => d.Value;

	#endregion
	
	#region Methods

	public static Anchor? Parse(string key)
	{
		if (Enum.TryParse<AnchorEnum>(key, true, out var enumValue))
		{
			return enumValue switch
			{
				AnchorEnum.Center => Center,
				AnchorEnum.Top => Top,
				AnchorEnum.Bottom => Bottom,
				AnchorEnum.Left => Left,
				AnchorEnum.Right => Right,
				AnchorEnum.TopLeft => TopLeft,
				AnchorEnum.TopRight => TopRight,
				AnchorEnum.BottomLeft => BottomLeft,
				AnchorEnum.BottomRight => BottomRight,
				_ => null
			};
		}

		return null;
	}

	#endregion
}