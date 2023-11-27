using SA3D.Modeling.Structs;
using System.Numerics;

namespace SA3D.Rendering.Structs
{
	/// <summary>
	/// Set of matrices needed for rendering.
	/// </summary>
	public readonly struct RenderMatrices
	{
		/// <summary>
		/// Model transformation matrix.
		/// </summary>
		public Matrix4x4 WorldSpace { get; }

		/// <summary>
		/// Model transformation matrix for normals.
		/// </summary>
		public Matrix4x4 WorldSpaceNormal { get; }

		/// <summary>
		/// Model-view-projection matrix.
		/// </summary>
		public Matrix4x4 ModelViewProjection { get; }

		/// <summary>
		/// Creates a new set of render matrices.
		/// </summary>
		/// <param name="worldMtx"></param>
		/// <param name="mvp"></param>
		public RenderMatrices(Matrix4x4 worldMtx, Matrix4x4 mvp)
		{
			WorldSpace = worldMtx;
			WorldSpaceNormal = worldMtx.GetNormalMatrix();
			ModelViewProjection = mvp;
		}
	}
}
