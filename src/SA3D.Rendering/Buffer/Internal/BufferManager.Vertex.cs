using OpenTK.Graphics.OpenGL4;
using SA3D.Common;
using SA3D.Modeling.Mesh.Buffer;
using SA3D.Rendering.Buffer.Internal;
using SA3D.Rendering.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SA3D.Rendering.Buffer
{
	internal partial class BufferManager
	{
		/// <summary>
		/// Vertex cache
		/// </summary>
		private readonly CachedVertex[] _vertices = new CachedVertex[0x10000];
		private readonly RegionMarker<bool> _hasNormalRegions = new();
		private readonly Dictionary<BufferMesh, BufferMeshHandle> _meshHandles = new();

		public void BufferMeshes(IEnumerable<BufferMesh> meshData, Matrix4x4? weightWorldMatrix, bool active)
		{
			Matrix4x4 normalMtx = default;
			if(weightWorldMatrix.HasValue)
			{
				Matrix4x4.Invert(weightWorldMatrix.Value, out normalMtx);
				normalMtx = Matrix4x4.Transpose(normalMtx);
			}

			foreach(BufferMesh mesh in meshData)
			{
				if(mesh.Vertices != null)
				{
					int from = int.MaxValue;
					int to = 0;

					if(weightWorldMatrix == null)
					{
						foreach(BufferVertex vtx in mesh.Vertices)
						{
							int index = vtx.Index + mesh.VertexWriteOffset;
							_vertices[index] = new(vtx);
							if(index < from)
							{
								from = index;
							}

							if(index > to)
							{
								to = index;
							}
						}
					}
					else
					{
						if(mesh.HasNormals)
						{
							foreach(BufferVertex vtx in mesh.Vertices)
							{
								Vector4 pos = Vector4.Transform(vtx.Position, weightWorldMatrix.Value) * vtx.Weight;
								Vector3 nrm = Vector3.TransformNormal(vtx.Normal, normalMtx) * vtx.Weight;

								int index = vtx.Index + mesh.VertexWriteOffset;

								if(index < from)
								{
									from = index;
								}

								if(index > to)
								{
									to = index;
								}

								if(mesh.ContinueWeight)
								{
									_vertices[index].position += pos;
									_vertices[index].normal += nrm;
								}
								else
								{
									_vertices[index] = new(pos, nrm);
								}

								_vertices[index].sumWeight += vtx.Weight;

								if(active)
								{
									_vertices[index].displayWeight = vtx.Weight;
								}
							}

						}
						else
						{
							foreach(BufferVertex vtx in mesh.Vertices)
							{
								Vector4 pos = Vector4.Transform(vtx.Position, weightWorldMatrix.Value) * vtx.Weight;

								int index = vtx.Index + mesh.VertexWriteOffset;

								if(index < from)
								{
									from = index;
								}

								if(index > to)
								{
									to = index;
								}

								if(mesh.ContinueWeight)
								{
									_vertices[index].position += pos;
								}
								else
								{
									_vertices[index] = new(pos, BufferMesh.DefaultNormal);
								}

								_vertices[index].sumWeight += vtx.Weight;

								if(active)
								{
									_vertices[index].displayWeight = vtx.Weight;
								}
							}
						}
					}

					to++;

					// if a mesh continues weight but has no normals, then it doesnt modify the regions
					if(!mesh.ContinueWeight || mesh.HasNormals)
					{
						_hasNormalRegions.MarkRegion((uint)from, (uint)to, mesh.HasNormals);
					}
				}

				if(mesh.Corners != null)
				{
					bool hasNormals;
					bool hasColors;


					if(_meshHandles.TryGetValue(mesh, out BufferMeshHandle handles))
					{
						hasColors = handles.HasColors;
						hasNormals = handles.HasNormals;
					}
					else
					{
						hasColors = mesh.HasColors;
						hasNormals = mesh.Vertices != null && mesh.HasNormals;
						if(!hasNormals)
						{
							uint from = mesh.VertexReadOffset;
							uint to = from + mesh.Corners.Max(x => x.VertexIndex) + 1;
							hasNormals = _hasNormalRegions.HasValue(from, to, true);
						}
					}

					if(hasNormals)
					{
						if(hasColors)
						{
							BufferMeshData(mesh, mesh.Corners.Select(x => new GPUVertexNC(_vertices[x.VertexIndex + mesh.VertexReadOffset], x)).ToArray());
						}
						else
						{
							BufferMeshData(mesh, mesh.Corners.Select(x => new GPUVertexN(_vertices[x.VertexIndex + mesh.VertexReadOffset], x)).ToArray());
						}
					}
					else
					{
						if(hasColors)
						{
							BufferMeshData(mesh, mesh.Corners.Select(x => new GPUVertexC(_vertices[x.VertexIndex + mesh.VertexReadOffset], x)).ToArray());
						}
						else
						{
							BufferMeshData(mesh, mesh.Corners.Select(x => new GPUVertex(_vertices[x.VertexIndex + mesh.VertexReadOffset], x)).ToArray());
						}
					}
				}
			}
		}

		private unsafe void BufferMeshData<T>(BufferMesh mesh, T[] cache) where T : unmanaged
		{
			if(!BufferVertexData(mesh, cache))
			{
				return;
			}

			int size = sizeof(T);
			int offset = 0;

			// position
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, size, offset);
			offset += 12;

			// normal
			if(typeof(T) == typeof(GPUVertexN) || typeof(T) == typeof(GPUVertexNC))
			{
				GL.EnableVertexAttribArray(1);
				GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, size, offset);
				offset += 12;
			}
			else
			{
				GL.VertexAttrib3(1, 0f, 0f, 0f);
			}

			// color
			if(typeof(T) == typeof(GPUVertexC) || typeof(T) == typeof(GPUVertexNC))
			{
				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, size, offset);
				offset += 4;
			}
			else
			{
				GL.VertexAttrib4(2, 1f, 1f, 1f, 1f);
			}

			// uv
			GL.EnableVertexAttribArray(3);
			GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, size, offset);
			offset += 8;

			// weight
			GL.EnableVertexAttribArray(4);
			GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, size, offset);
		}

		private unsafe bool BufferVertexData<T>(BufferMesh mesh, T[] cache) where T : unmanaged
		{
			if(_meshHandles.TryGetValue(mesh, out BufferMeshHandle meshHandle))
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, meshHandle.VertexBufferObject);
				GL.BufferData(BufferTarget.ArrayBuffer, cache.Length * sizeof(T), cache, BufferUsageHint.StreamDraw);
				return false;
			}

			if(mesh.Corners == null)
			{
				throw new NullReferenceException("Mesh to buffer has no polygon data!");
			}

			// generating the buffers
			int vao = GL.GenVertexArray();
			int vbo = GL.GenBuffer();
			int eao = 0;
			int vtxCount = mesh.IndexList == null
				? mesh.Corners.Length
				: mesh.IndexList.Length;

			// Binding the buffers
			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			GL.BufferData(BufferTarget.ArrayBuffer, cache.Length * sizeof(T), cache, BufferUsageHint.StaticDraw);

			if(mesh.IndexList != null)
			{
				eao = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, eao);
				GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.IndexList.Length * sizeof(uint), mesh.IndexList, BufferUsageHint.StaticDraw);
			}
			else
			{
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			}

			_meshHandles.Add(mesh, new(
				vao, vbo, eao, vtxCount,
				typeof(T) == typeof(GPUVertexN) || typeof(T) == typeof(GPUVertexNC),
				typeof(T) == typeof(GPUVertexC) || typeof(T) == typeof(GPUVertexNC),
				mesh.Strippified ? PrimitiveType.TriangleStrip : PrimitiveType.Triangles));
			return true;
		}

		public void DebufferMesh(BufferMesh mesh)
		{
			if(!_meshHandles.TryGetValue(mesh, out BufferMeshHandle handle))
			{
				throw new InvalidOperationException("Mesh was not buffered");
			}

			GL.DeleteVertexArray(handle.VertexArrayObject);
			GL.DeleteBuffer(handle.VertexBufferObject);
			if(handle.ElementArrayObject != 0)
			{
				GL.DeleteBuffer(handle.ElementArrayObject);
			}

			_meshHandles.Remove(mesh);
		}

		public bool IsBuffered(BufferMesh mesh)
		{
			return _meshHandles.ContainsKey(mesh);
		}

		public BufferMeshHandle GetHandle(BufferMesh mesh)
		{
			if(!_meshHandles.TryGetValue(mesh, out BufferMeshHandle handle))
			{
				throw new InvalidOperationException("Mesh was not buffered!");
			}

			return handle;
		}

		public unsafe void BufferMatrices(RenderMatrices matrices)
		{
			MatrixUBO.ResetPosition();

			MatrixUBO.WriteMatrix(matrices.WorldSpace);
			MatrixUBO.WriteMatrix(matrices.WorldSpaceNormal);
			MatrixUBO.WriteMatrix(matrices.ModelViewProjection);

			MatrixUBO.BufferData();
		}

	}
}
