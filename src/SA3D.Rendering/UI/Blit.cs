using OpenTK.Graphics.OpenGL4;

namespace SA3D.Rendering.UI
{
	/// <summary>
	/// Blit utility for rendering to the entire frame buffer on near-plane.
	/// </summary>
	public static class Blit
	{
		private static int _vao;
		private static int _vbo;

		/// <summary>
		/// Initializes the vertex buffer for drawing.
		/// </summary>
		public static void Initialize()
		{
			if(_vao != 0)
			{
				return;
			}

			_vao = GL.GenVertexArray();
			GL.BindVertexArray(_vao);

			_vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			float[] data = { -1, 1, -1, -3, 3, 1 };

			GL.BufferData(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.StaticDraw);

			// pos
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8, 0);

			GL.BindVertexArray(0);
		}

		/// <summary>
		/// Renders the blit.
		/// </summary>
		public static void Render()
		{
			int previousVAO = GL.GetInteger(GetPName.VertexArrayBinding);
			GL.BindVertexArray(_vao);
			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
			GL.BindVertexArray(previousVAO);
		}
	}
}
