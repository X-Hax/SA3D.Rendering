using static SA3D.Rendering.Shaders.ShaderSource;

namespace SA3D.Rendering.Shaders
{
	/// <summary>
	/// Provides methods for easily accessing vertex shader code.
	/// </summary>
	public class VertexShaders
	{
		/// <summary>
		/// Returns vertex shader for:
		/// <br/> Near plane screen.
		/// </summary>
		public static string GetScreen()
		{
			return GetResource("Vertex.Screen.vert");
		}

		/// <summary>
		/// Returns vertex shader for:
		/// <br/> matrix influenced positions only.
		/// </summary>
		public static string GetPosition()
		{
			return GetResource("Vertex.Position.vert")
				.Replace("#include Matrices", ShaderIncludes.GetMatrices());
		}

		/// <summary>
		/// Returns vertex shader for:
		/// <br/> UI elements.
		/// </summary>
		public static string GetSprite()
		{
			return GetResource("Vertex.Sprite.vert")
				.Replace("#include SpriteInfo", ShaderIncludes.GetSpriteInfo());
		}

		/// <summary>
		/// Returns vertex shader for:
		/// <br/> Default mesh rendering.
		/// </summary>
		public static string GetSurface()
		{
			return GetResource("Vertex.Surface.vert")
				.Replace("#include Matrices", ShaderIncludes.GetMatrices());
		}

		/// <summary>
		/// Returns vertex shader for:
		/// <br/> Billboard sprites.
		/// </summary>
		public static string GetBillBoard()
		{
			return GetResource("Vertex.BillBoard.vert")
				.Replace("#include Matrices", ShaderIncludes.GetMatrices());
		}

		/// <summary>
		/// Returns vertex shader for:
		/// <br/> Debug mesh rendering.
		/// </summary>
		public static string GetSurfaceDebug()
		{
			return GetResource("Vertex.SurfaceDebug.vert")
				.Replace("#include Matrices", ShaderIncludes.GetMatrices());
		}

		/// <summary>
		/// Returns vertex shader for:
		/// <br/> Wireframe rendering.
		/// </summary>
		public static string GetWireframe()
		{
			return GetResource("Vertex.Wireframe.vert")
				.Replace("#include Matrices", ShaderIncludes.GetMatrices());
		}
	}
}
