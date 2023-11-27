using OpenTK.Graphics.OpenGL4;
using SA3D.Modeling.Mesh.Buffer;
using System.Numerics;

namespace SA3D.Rendering.Buffer.Internal
{
	/// <summary>
	/// Contains all handles and material info of a mesh
	/// </summary>
	internal readonly struct BufferMeshHandle
	{
		public int VertexArrayObject { get; }
		public int VertexBufferObject { get; }
		public int ElementArrayObject { get; }
		public int VertexCount { get; }
		public bool HasNormals { get; }
		public bool HasColors { get; }
		public PrimitiveType Type { get; }

		public BufferMeshHandle(int vertexArrayObject, int vertexBufferObject, int eAO, int vertexCount, bool hasNormals, bool hasColors, PrimitiveType type = PrimitiveType.Triangles)
		{
			VertexArrayObject = vertexArrayObject;
			VertexBufferObject = vertexBufferObject;
			ElementArrayObject = eAO;
			VertexCount = vertexCount;
			HasNormals = hasNormals;
			HasColors = hasColors;
			Type = type;
		}

		public void Bind()
		{
			GL.BindVertexArray(VertexArrayObject);
		}

	}

	/// <summary>
	/// Cached vertex
	/// </summary>
	internal struct CachedVertex
	{
		public Vector4 position;
		public Vector3 normal;
		public float displayWeight;
		public float sumWeight;

		public readonly Vector3 V3Position => new(position.X, position.Y, position.Z);

		public CachedVertex(Vector4 position, Vector3 normal)
		{
			this.position = position;
			this.normal = normal;
			displayWeight = 0;
			sumWeight = 0;
		}

		public CachedVertex(BufferVertex vtx)
		{
			position = new(vtx.Position, 1);
			normal = vtx.Normal;
			displayWeight = 0;
			sumWeight = 1;
		}


		public override readonly string ToString()
		{
			return sumWeight.ToString("F3"); //$"({position.X:f3}, {position.Y:f3}, {position.Z:f3}, {position.W:f3}) - ({normal.DebugString()})";
		}
	}
}
