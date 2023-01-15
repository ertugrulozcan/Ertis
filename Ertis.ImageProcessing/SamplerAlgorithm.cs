using SixLabors.ImageSharp.Processing.Processors.Transforms;

// ReSharper disable MemberCanBePrivate.Global
namespace Ertis.ImageProcessing;

public class SamplerAlgorithm
{
	#region Statics

	public static readonly SamplerAlgorithm Bicubic = new(SamplerAlgorithmEnum.Bicubic);
	public static readonly SamplerAlgorithm Box = new(SamplerAlgorithmEnum.Box);
	public static readonly SamplerAlgorithm Cubic = new(SamplerAlgorithmEnum.Cubic);
	public static readonly SamplerAlgorithm Lanczos = new(SamplerAlgorithmEnum.Lanczos);
	public static readonly SamplerAlgorithm Triangle = new(SamplerAlgorithmEnum.Triangle);
	public static readonly SamplerAlgorithm Welch = new(SamplerAlgorithmEnum.Welch);
	public static readonly SamplerAlgorithm NearestNeighbor = new(SamplerAlgorithmEnum.NearestNeighbor);
	
	#endregion
	
	#region Properties

	private SamplerAlgorithmEnum Value { get; }

	#endregion
	
	#region Constructors

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value"></param>
	private SamplerAlgorithm(SamplerAlgorithmEnum value)
	{
		this.Value = value;
	}

	#endregion

	#region Methods

	// ReSharper disable once IdentifierTypo
	public IResampler? ToResampler()
	{
		return this.Value switch
		{
			SamplerAlgorithmEnum.Bicubic => new BicubicResampler(),
			SamplerAlgorithmEnum.Box => new BoxResampler(),
			SamplerAlgorithmEnum.Cubic => new CubicResampler(),
			SamplerAlgorithmEnum.Lanczos => new LanczosResampler(),
			SamplerAlgorithmEnum.Triangle => new TriangleResampler(),
			SamplerAlgorithmEnum.Welch => new WelchResampler(),
			SamplerAlgorithmEnum.NearestNeighbor => new NearestNeighborResampler(),
			_ => null
		};
	}
	
	public static SamplerAlgorithm? Parse(string key)
	{
		if (Enum.TryParse<SamplerAlgorithmEnum>(key, true, out var enumValue))
		{
			return enumValue switch
			{
				SamplerAlgorithmEnum.Bicubic => Bicubic,
				SamplerAlgorithmEnum.Box => Box,
				SamplerAlgorithmEnum.Cubic => Cubic,
				SamplerAlgorithmEnum.Lanczos => Lanczos,
				SamplerAlgorithmEnum.Triangle => Triangle,
				SamplerAlgorithmEnum.Welch => Welch,
				SamplerAlgorithmEnum.NearestNeighbor => NearestNeighbor,
				_ => null
			};
		}

		return null;
	}

	#endregion

	#region Enum

	private enum SamplerAlgorithmEnum
	{
		Bicubic,
		Box,
		Cubic,
		Lanczos,
		Triangle,
		Welch,
		NearestNeighbor
	}

	#endregion
}