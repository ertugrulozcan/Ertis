// ReSharper disable UnusedMember.Global
namespace Ertis.ImageProcessing;

/// <summary>
/// How far a large downscale may be done by the decoder (TargetSizeMode.Auto); the decoder scaled images are resized to the target size by the requested sampler
/// </summary>
public enum ResizeQuality
{
	/// <summary>
	/// The decoder scales down to the target size (the fastest)
	/// </summary>
	Balanced,
	
	/// <summary>
	/// The decoder scales down to twice the target size at most, so the requested sampler does the last halving (closer to a full decode, slower)
	/// </summary>
	High
}
