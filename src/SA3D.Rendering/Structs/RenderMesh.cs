using SA3D.Modeling.Mesh.Buffer;
using SA3D.Texturing;

namespace SA3D.Rendering.Structs
{
	/// <summary>
	/// Mesh render information-
	/// </summary>
	public readonly struct RenderMesh
	{
		/// <summary>
		/// Meshes to render.
		/// </summary>
		public BufferMesh[] Meshes { get; }

		/// <summary>
		/// Matrices to render with.
		/// </summary>
		public RenderMatrices Matrices { get; }

		/// <summary>
		/// Textures to use when rendering.
		/// </summary>
		public TextureSet? Textures { get; }

		/// <summary>
		/// Creates a new render mesh.
		/// </summary>
		/// <param name="meshes">Meshes to render.</param>
		/// <param name="matrices">Matrices to render with.</param>
		/// <param name="textures">Textures to use when rendering.</param>
		public RenderMesh(BufferMesh[] meshes, RenderMatrices matrices, TextureSet? textures)
		{
			Meshes = meshes;
			Matrices = matrices;
			Textures = textures;
		}
	}
}
