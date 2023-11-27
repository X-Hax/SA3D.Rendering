using static SA3D.Rendering.Shaders.ShaderSource;

namespace SA3D.Rendering.Shaders
{
	/// <summary>
	/// Provides methods for easily accessing shader include code.
	/// </summary>
	public static class ShaderIncludes
	{
		/// <summary>
		/// Returns shader include for:
		/// <br/> Functions involving order-independent-transparency.
		/// </summary>
		public static string GetOIT()
		{
			return GetResource("Include.OIT.glsl");
		}

		/// <summary>
		/// Returns shader include for:
		/// <br/> Global buffers acting as shader inputs.
		/// </summary>
		public static string GetGlobalInputs()
		{
			return GetResource("Include.GlobalInputs.glsl");
		}

		/// <summary>
		/// Returns shader include for:
		/// <br/> Commonly used functions between fragment shaders.
		/// </summary>
		public static string GetCommonFunctions()
		{
			return GetResource("Include.Common.glsl")
				.Replace("#include GlobalInputs", GetGlobalInputs())
				.Replace("#include OIT", GetOIT());
		}

		/// <summary>
		/// Returns shader include for:
		/// <br/> Input matix structs.
		/// </summary>
		public static string GetMatrices()
		{
			return GetResource("Include.Matrices.glsl");
		}

		/// <summary>
		/// Returns shader include for:
		/// <br/> Sprite info input structs.
		/// </summary>
		public static string GetSpriteInfo()
		{
			return GetResource("Include.SpriteInfo.glsl");
		}
	}
}
