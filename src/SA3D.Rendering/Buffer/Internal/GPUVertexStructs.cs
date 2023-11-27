using SA3D.Modeling.Mesh.Buffer;
using SA3D.Modeling.Structs;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SA3D.Rendering.Buffer.Internal
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal readonly struct GPUVertexNC
	{
		public readonly Vector3 position;
		public readonly Vector3 normal;
		public readonly Color color;
		public readonly Vector2 uv;
		public readonly float displayWeight;

		public GPUVertexNC(CachedVertex vertex, BufferCorner corner)
		{
			if(vertex.sumWeight != 1.0)
			{
				position = vertex.V3Position / vertex.sumWeight;
				normal = vertex.normal / vertex.sumWeight;
			}
			else
			{
				position = vertex.V3Position;
				normal = vertex.normal;
			}

			color = corner.Color;
			uv = corner.Texcoord;
			displayWeight = vertex.displayWeight;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal readonly struct GPUVertexN
	{
		public readonly Vector3 position;
		public readonly Vector3 normal;
		public readonly Vector2 uv;
		public readonly float displayWeight;

		public GPUVertexN(CachedVertex vertex, BufferCorner corner)
		{
			if(vertex.sumWeight != 1.0)
			{
				position = vertex.V3Position / vertex.sumWeight;
				normal = vertex.normal / vertex.sumWeight;
			}
			else
			{
				position = vertex.V3Position;
				normal = vertex.normal;
			}

			uv = corner.Texcoord;
			displayWeight = vertex.displayWeight;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal readonly struct GPUVertexC
	{
		public readonly Vector3 position;
		public readonly Color color;
		public readonly Vector2 uv;
		public readonly float displayWeight;

		public GPUVertexC(CachedVertex vertex, BufferCorner corner)
		{
			position = vertex.sumWeight != 1.0f
				? vertex.V3Position / vertex.sumWeight
				: vertex.V3Position;

			color = corner.Color;
			uv = corner.Texcoord;
			displayWeight = vertex.displayWeight;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal readonly struct GPUVertex
	{
		public readonly Vector3 position;
		public readonly Vector2 uv;
		public readonly float displayWeight;

		public GPUVertex(CachedVertex vertex, BufferCorner corner)
		{
			position = vertex.sumWeight != 1.0f
				? vertex.V3Position / vertex.sumWeight
				: vertex.V3Position;

			uv = corner.Texcoord;
			displayWeight = vertex.displayWeight;
		}
	}
}
