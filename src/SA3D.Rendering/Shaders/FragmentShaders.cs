using static SA3D.Rendering.Shaders.ShaderSource;

namespace SA3D.Rendering.Shaders
{
	/// <summary>
	/// Provides methods for easily accessing fragment shader code.
	/// </summary>
	public static class FragmentShaders
	{
		/// <summary>
		/// Returns fragment shader for:
		/// <br/> UI Elements.
		/// </summary>
		public static string GetSprite()
		{
			return GetResource("Fragment.Sprite.frag")
				.Replace("#include SpriteInfo", ShaderIncludes.GetSpriteInfo());
		}

		/// <summary>
		/// Returns fragment shader for:
		/// <br/> OIT compositing for framebuffers.
		/// </summary>
		public static string GetComposite()
		{
			return GetResource("Fragment.Composite.frag")
				.Replace("#include OIT", ShaderIncludes.GetOIT());
		}

		/// <summary>
		/// Returns fragment shader for:
		/// <br/> OIT compositing for meshes.
		/// </summary>
		public static string GetMeshComposite()
		{
			return GetResource("Fragment.MeshComposite.frag")
				.Replace("#include OIT", ShaderIncludes.GetOIT())
				.Replace("#include GlobalInputs", ShaderIncludes.GetGlobalInputs());
		}

		/// <summary>
		/// Returns fragment shader for:
		/// <br/> Default mesh rendering.
		/// </summary>
		public static string GetSurface()
		{
			return GetResource("Fragment.Surface.frag")
				.Replace("#include Common", ShaderIncludes.GetCommonFunctions());
		}

		/// <summary>
		/// Returns fragment shader for:
		/// <br/> Billboards.
		/// </summary>
		public static string GetBillBoard()
		{
			return GetResource("Fragment.BillBoard.frag")
				.Replace("#include Common", ShaderIncludes.GetCommonFunctions());
		}

		/// <summary>
		/// Returns fragment shader for:
		/// <br/> Debug mesh rendering.
		/// </summary>
		public static string GetSurfaceDebug()
		{
			return GetResource("Fragment.SurfaceDebug.frag")
				.Replace("#include GlobalInputs", ShaderIncludes.GetGlobalInputs())
				.Replace("#include Common", ShaderIncludes.GetCommonFunctions());
		}

		/// <summary>
		/// Returns fragment shader for:
		/// <br/> Wireframe rendering.
		/// </summary>
		public static string GetWireframe()
		{
			return GetResource("Fragment.Wireframe.frag");
		}
	}
}
