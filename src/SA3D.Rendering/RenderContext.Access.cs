using SA3D.Modeling.Mesh.Buffer;
using SA3D.Rendering.Shaders;
using SA3D.Rendering.Structs;
using SA3D.Rendering.UI;
using SA3D.Texturing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SA3D.Rendering
{
	public partial class RenderContext
	{
		/// <summary>
		/// Set the shader used for drawing 3D meshes.
		/// <br/> Setting null will use the default shader (<see cref="Shaders.Shaders.Surface"/>)
		/// </summary>
		/// <param name="shader">The shader to set.</param>
		public void SetMeshShader(Shader? shader)
		{
			_customMeshShader = shader;
		}

		/// <summary>
		/// Set the shader used for drawing billboards.
		/// <br/> Setting null will use the default shader (<see cref="Shaders.Shaders.Billboard"/>)
		/// </summary>
		/// <param name="shader">The shader to set.</param>
		public void SetBillBoardShader(Shader? shader)
		{
			_customBillBoardShader = shader;
		}

		/// <summary>
		/// Set the lighting by index.
		/// </summary>
		/// <param name="index">Index of the lighting to set. ranges 0 to 3 (inclusive).</param>
		/// <param name="light">Lighting data to set.</param>
		public void SetLighting(int index, Lighting light)
		{
			_lighting[index] = light;
		}

		/// <summary>
		/// Set all lighting at once. Requires array with exactly 4 elements.
		/// </summary>
		/// <param name="lighting">The lighting to set.</param>
		/// <exception cref="ArgumentException"></exception>
		public void SetLighting(Lighting[] lighting)
		{
			if(lighting.Length != 4)
			{
				throw new ArgumentException("Expected 4 lights");
			}

			Array.Copy(lighting, _lighting, 4);
		}

		/// <summary>
		/// Clears the lighting.
		/// </summary>
		public void ClearLighting()
		{
			Array.Clear(_lighting);
		}

		/// <summary>
		/// Loads and 'enqueues' a texture set. 
		/// </summary>
		/// <param name="textures">Textures to load.</param>
		public void LoadTextureSet(TextureSet textures)
		{
			_bufferManager.BufferTextures(textures);
		}

		/// <summary>
		/// 'Dequeues' a texture set and unloads it if it is not used anymore.
		/// </summary>
		/// <param name="textures">The textures to unload.</param>
		public void UnloadTextureSet(TextureSet textures)
		{
			_bufferManager.DebufferTextureSet(textures);
		}

		/// <summary>
		/// Rebuffers the textures from a texture set.
		/// </summary>
		/// <param name="textures">The textures to reload.</param>
		public void ReloadTextureSet(TextureSet textures)
		{
			_bufferManager.RebufferTextureSet(textures);
		}

		/// <summary>
		/// Reloads a specific texture of a texture set.
		/// </summary>
		/// <param name="textures">Textureset to reload a texture of.</param>
		/// <param name="index">Index of the texture to reload.</param>
		public void ReloadTextureSetTexture(TextureSet textures, int index)
		{
			_bufferManager.RebufferTextureSetTexture(textures, index);
		}

		/// <summary>
		/// Reloads a specific texture. Does so for every texture set it is found in.
		/// </summary>
		/// <param name="texture">The texture to reload.</param>
		public void ReloadTextureSetTexture(Texture texture)
		{
			_bufferManager.RebufferTextureSetTexture(texture);
		}

		/// <summary>
		/// Adds a canvas to the context.
		/// </summary>
		/// <param name="canvas">The canvas to add.</param>
		public void AddCanvas(Canvas canvas)
		{
			_canvases.Add(canvas);
			if(_initialized)
			{
				_bufferManager.BufferTextures(canvas.Textures);
				canvas.InternalInitialize();
			}
		}

		/// <summary>
		/// Removes a canvas from the context.
		/// </summary>
		/// <param name="canvas">The canvas to remove.</param>
		public void RemoveCanvas(Canvas canvas)
		{
			_canvases.Remove(canvas);
			if(_initialized)
			{
				_bufferManager.DebufferTextureSet(canvas.Textures);
			}
		}

		/// <summary>
		/// Buffers meshes for drawing,
		/// </summary>
		/// <param name="meshes">Meshes to buffer.</param>
		/// <param name="force">Buffer regardless of whether the mesh has been buffered before.</param>
		public void BufferMeshes(IEnumerable<BufferMesh> meshes, bool force = false)
		{
			if(meshes.Any() && (force || !_bufferManager.IsBuffered(meshes.First())))
			{
				_bufferManager.BufferMeshes(meshes, null, false);
			}
		}

		/// <summary>
		/// Disposes the buffered mesh data.
		/// </summary>
		/// <param name="meshes">Mesh data to debuffer.</param>
		public void DebufferMeshes(BufferMesh[] meshes)
		{
			foreach(BufferMesh mesh in meshes)
			{
				_bufferManager.DebufferMesh(mesh);
			}
		}

	}
}
