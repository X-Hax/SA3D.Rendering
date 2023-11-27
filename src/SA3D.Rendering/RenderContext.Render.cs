using OpenTK.Graphics.OpenGL4;
using SA3D.Modeling.Mesh.Buffer;
using SA3D.Modeling.ObjectData;
using SA3D.Rendering.Buffer.Internal;
using SA3D.Rendering.Shaders;
using SA3D.Rendering.Structs;
using System.Linq;
using System.Numerics;

namespace SA3D.Rendering
{
	public partial class RenderContext
	{
		/// <summary>
		/// Renders a model.
		/// </summary>
		/// <param name="node">Node from which to start rendering.</param>
		/// <param name="active">Currently active/highlighted node.</param>
		public void RenderModel(Node node, Node? active = null)
		{
			RenderModel(node, active, null, null, node.CheckHasTreeWeightedMesh());
		}

		private void RenderModel(
			Node node,
			Node? active,
			Matrix4x4? parentWorld,
			Matrix4x4? weightRootWorld,
			bool weighted)
		{
			Matrix4x4 world = node.LocalMatrix;

			if(parentWorld.HasValue)
			{
				world *= parentWorld.Value;
			}
			else if(weighted)
			{
				weightRootWorld = world;
				world = Matrix4x4.Identity;
			}

			if(node.Attach != null && node.Attach.MeshData.Length > 0)
			{
				// if a model is weighted, then the buffered vertex positions/normals will have to be set to world space, which means that world and normal matrix should be identities
				if(weighted)
				{
					_bufferManager.BufferMeshes(node.Attach.MeshData, world, active == node);
				}
				else if(!_bufferManager.IsBuffered(node.Attach.MeshData[0]))
				{
					_bufferManager.BufferMeshes(node.Attach.MeshData, null, false);
				}

				RenderMatrices matrices = weightRootWorld.HasValue
					? new(weightRootWorld.Value, Camera.GetMVPMatrix(weightRootWorld.Value))
					: new(world, Camera.GetMVPMatrix(world));

				(BufferMesh[] opaque, BufferMesh[] transparent) = node.Attach.GetDisplayMeshes();
				BufferMesh[] meshes = node.Attach.MeshData.Where(x => x.Corners != null).ToArray();

				if(meshes.Length > 0 && !node.SkipDraw)
				{
					RenderMeshes(meshes, new RenderMatrices[] { matrices });
				}
			}

			for(int i = 0; i < node.ChildCount; i++)
			{
				RenderModel(node[i], active, world, weightRootWorld, weighted);
			}
		}

		private void RenderMesh(BufferMeshHandle handle, Shader? fallback = null)
		{
			if(handle.ElementArrayObject == 0)
			{
				GL.DrawArrays(handle.Type, 0, handle.VertexCount);
			}
			else
			{
				GL.DrawElements(handle.Type, handle.VertexCount, DrawElementsType.UnsignedInt, 0);
			}

			if(WireFrameMode == WireFrameMode.Overlay)
			{
				Shaders.Shaders.Wireframe.Use();
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

				if(handle.ElementArrayObject == 0)
				{
					GL.DrawArrays(handle.Type, 0, handle.VertexCount);
				}
				else
				{
					GL.DrawElements(handle.Type, handle.VertexCount, DrawElementsType.UnsignedInt, 0);
				}

				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
				(fallback ?? MeshShader).Use();
			}
		}

		/// <summary>
		/// Renders a single mesh.
		/// </summary>
		/// <param name="mesh">Mesh to render.</param>
		/// <param name="matrices">Matrices to render the mesh with.</param>
		public void RenderMesh(BufferMesh mesh, RenderMatrices matrices)
		{
			if(mesh.Corners == null)
			{
				return;
			}

			MeshShader.Use();

			BufferMeshHandle handle = _bufferManager.GetHandle(mesh);
			handle.Bind();

			_bufferManager.SetSurfaceMaterial(mesh.Material, handle.HasNormals, handle.HasColors);
			_bufferManager.BufferMatrices(matrices);

			RenderMesh(handle);
		}

		/// <summary>
		/// Renders a collection of meshes with multiple render matrices (meaning, rendering each mesh multiple times)
		/// </summary>
		/// <param name="meshes">Meshes to render.</param>
		/// <param name="matrices">Matrices to render the meshes with.</param>
		public void RenderMeshes(BufferMesh[] meshes, RenderMatrices[] matrices)
		{
			MeshShader.Use();

			foreach(BufferMesh mesh in meshes)
			{
				if(mesh.Corners == null)
				{
					continue;
				}

				BufferMeshHandle handle = _bufferManager.GetHandle(mesh);
				handle.Bind();

				_bufferManager.SetSurfaceMaterial(mesh.Material, handle.HasNormals, handle.HasColors);

				foreach(RenderMatrices m in matrices)
				{
					_bufferManager.BufferMatrices(m);
					RenderMesh(handle);
				}
			}
		}

		/// <summary>
		/// Renders a billboard.
		/// </summary>
		/// <param name="position">Position in worldspace.</param>
		/// <param name="scale">Scale of the billboard.</param>
		/// <param name="material">Surface information.</param>
		public void RenderBillBoard(Vector3 position, Vector2 scale, BufferMaterial material)
		{
			BillBoardShader.Use();

			BufferMeshHandle handle = _bufferManager.BillBoardHandle;
			handle.Bind();

			position = Vector3.Transform(position, Camera.GetMVMatrix(Matrix4x4.Identity));

			Matrix4x4 world = Matrix4x4.CreateScale(scale.X, scale.Y, 1) * Matrix4x4.CreateTranslation(position);
			RenderMatrices matrices = new(world, Camera.GetMPMatrix(world));
			_bufferManager.BufferMatrices(matrices);

			_bufferManager.SetSurfaceMaterial(material, handle.HasNormals, handle.HasColors);

			RenderMesh(handle, BillBoardShader);
		}
	}
}
