using OpenTK.Graphics.OpenGL4;
using SA3D.Rendering.Buffer.Internal;

namespace SA3D.Rendering.Buffer
{
	internal partial class BufferManager
	{
		public BufferMeshHandle LineBufferHandle { get; private set; }
		public BufferMeshHandle BillBoardHandle { get; private set; }
		public BufferMeshHandle SpriteHandle { get; private set; }

		private int _fallbackTexture;

		public BufferManager()
		{
			MatrixUBO = new(192);
			SettingsUBO = new(16);
			CameraUBO = new(32);
			LightingUBO = new(64 * 4);
			SurfaceUBO = new(56);
			SpriteInfoUBO = new(96);
		}

		public void Initialize()
		{
			MatrixUBO.Generate();
			SettingsUBO.Generate();
			CameraUBO.Generate();
			LightingUBO.Generate();
			SurfaceUBO.Generate();
			SpriteInfoUBO.Generate();

			BillBoardHandle = InitializeBillboard();
			LineBufferHandle = InitializeLineBuffer();
			SpriteHandle = InitializeSpriteBuffer();
			_fallbackTexture = InitializeFallbackTexture();
		}

		private BufferMeshHandle InitializeBillboard()
		{
			const float size = 1;

			int vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);

			int vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			float[] data =
			{
				-size, -size, 0, 0,
				-size, size, 0, 1,
				size, -size, 1, 0,
				size, size, 1, 1,
			};

			GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);

			GL.EnableVertexAttribArray(3);
			GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, 16, 8);

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			return new BufferMeshHandle(vao, vbo, 0, 4, false, false, PrimitiveType.TriangleStrip);
		}

		private BufferMeshHandle InitializeSpriteBuffer()
		{
			int vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);

			int vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			float[] data =
			{
				0, 0,
				0, 1,
				1, 0,
				1, 1,

			};
			GL.BufferData(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8, 0);

			GL.BindVertexArray(0);

			return new BufferMeshHandle(vao, vbo, 0, 4, false, false, PrimitiveType.TriangleStrip);
		}

		private BufferMeshHandle InitializeLineBuffer()
		{
			int vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);

			int vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, 0);

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			return new BufferMeshHandle(vao, vbo, 0, 0, false, false);
		}

		private int InitializeFallbackTexture()
		{
			int handle = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, handle);

			GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.R8, 1, 1);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleRgba, new[] { 1, 1, 1, 1 });

			GL.BindTexture(TextureTarget.Texture2D, 0);

			return handle;
		}
	}
}
