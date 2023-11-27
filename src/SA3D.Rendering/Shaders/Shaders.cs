namespace SA3D.Rendering.Shaders
{
	/// <summary>
	/// Provides builtin shaders.
	/// </summary>
	public static class Shaders
	{
		private static bool _compiled;

		/// <summary>
		/// Default mesh shader.
		/// </summary>
		public static Shader Surface { get; }

		/// <summary>
		/// Debug mesh shader.
		/// </summary>
		public static Shader SurfaceDebug { get; }

		/// <summary>
		/// Billboard rendering of sprites.
		/// </summary>
		public static Shader Billboard { get; }

		/// <summary>
		/// Wireframe rendering.
		/// </summary>
		public static Shader Wireframe { get; }

		/// <summary>
		/// UI Element rendering.
		/// </summary>
		public static Shader Sprite { get; }

		/// <summary>
		/// OIT framebuffer compositing.
		/// </summary>
		public static Shader Composite { get; }

		/// <summary>
		/// OIT mesh surface compositing.
		/// </summary>
		public static Shader MeshComposite { get; }

		static Shaders()
		{
			Surface = new(
				"Surface",
				VertexShaders.GetSurface(),
				FragmentShaders.GetSurface());

			SurfaceDebug = new(
				"Surface Debug",
				VertexShaders.GetSurfaceDebug(),
				FragmentShaders.GetSurfaceDebug());

			Billboard = new(
				"Billboard",
				VertexShaders.GetBillBoard(),
				FragmentShaders.GetBillBoard());

			Wireframe = new(
				"Wireframe",
				VertexShaders.GetWireframe(),
				FragmentShaders.GetWireframe());

			Sprite = new(
				"Sprite",
				VertexShaders.GetSprite(),
				FragmentShaders.GetSprite());

			Composite = new(
				"OIT Composite",
				VertexShaders.GetScreen(),
				FragmentShaders.GetComposite());

			MeshComposite = new(
				"OIT Mesh Composite",
				VertexShaders.GetPosition(),
				FragmentShaders.GetMeshComposite());
		}

		internal static void Compile()
		{
			if(_compiled)
			{
				return;
			}

			_compiled = true;

			Surface.Compile();
			SurfaceDebug.Compile();
			Billboard.Compile();
			Wireframe.Compile();
			Sprite.Compile();
			Composite.Compile();
			MeshComposite.Compile();
		}
	}
}
