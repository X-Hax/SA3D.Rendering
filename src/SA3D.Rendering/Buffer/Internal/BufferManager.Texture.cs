using OpenTK.Graphics.OpenGL4;
using SA3D.Texturing;
using System;
using System.Collections.Generic;

namespace SA3D.Rendering.Buffer
{
	internal partial class BufferManager
	{
		private readonly Dictionary<TextureSet, int> _textureSetUsages = new();
		private readonly Dictionary<TextureSet, int[]> _textureHandles = new();

		/// <summary>
		/// Active texture set to be used by the material
		/// </summary>
		public TextureSet? ActiveTextures { get; set; }

		private unsafe int BufferTexture(Texture texture, int handle = 0)
		{
			if(handle == 0)
			{
				handle = GL.GenTexture();
			}

			GL.BindTexture(TextureTarget.Texture2D, handle);

			fixed(byte* ptr = texture.GetColorPixels())
			{
				GL.TexImage2D(
					TextureTarget.Texture2D,
					0,
					PixelInternalFormat.Rgba,
					texture.Width,
					texture.Height,
					0,
					PixelFormat.Rgba,
					PixelType.UnsignedByte,
					(IntPtr)ptr);
			}

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			return handle;
		}

		public void RebufferTextureSet(TextureSet textures)
		{
			if(!_textureHandles.TryGetValue(textures, out int[]? handles))
			{
				throw new InvalidOperationException("Textures not buffered yet");
			}

			for(int i = 0; i < textures.Textures.Count; i++)
			{
				BufferTexture(textures.Textures[i], handles[i]);
			}
		}

		public void RebufferTextureSetTexture(TextureSet textures, int index)
		{
			if(!_textureHandles.TryGetValue(textures, out int[]? handles))
			{
				throw new InvalidOperationException("Textures not buffered yet");
			}

			if(index < 0 || index >= handles.Length - 1)
			{
				throw new IndexOutOfRangeException();
			}

			BufferTexture(textures.Textures[index], handles[index]);
		}

		public void RebufferTextureSetTexture(Texture texture)
		{
			foreach(KeyValuePair<TextureSet, int[]> item in _textureHandles)
			{
				int index = item.Key.Textures.IndexOf(texture);
				if(index != -1)
				{
					BufferTexture(texture, item.Value[index]);
				}
			}
		}

		public void BufferTextures(TextureSet textures)
		{
			if(_textureSetUsages.TryGetValue(textures, out int value))
			{
				_textureSetUsages[textures] = value + 1;
				return;
			}

			_textureSetUsages.Add(textures, 1);

			if(_textureHandles.ContainsKey(textures))
			{
				return;
			}

			int[] handles = new int[textures.Textures.Count];

			for(int i = 0; i < textures.Textures.Count; i++)
			{
				handles[i] = BufferTexture(textures.Textures[i]);
			}

			_textureHandles.Add(textures, handles);
		}

		public void DebufferTextureSet(TextureSet textures)
		{
			int timesUsed = _textureSetUsages[textures];
			if(timesUsed > 1)
			{
				_textureSetUsages[textures] = timesUsed - 1;
				return;
			}

			_textureSetUsages.Remove(textures);
			int[] handles = _textureHandles[textures];
			GL.DeleteBuffers(handles.Length, handles);
		}

		public bool BindTexture(TextureUnit unit, int textureIndex)
		{
			GL.ActiveTexture(unit);
			if(ActiveTextures == null || ActiveTextures.Textures.Count <= textureIndex)
			{
				GL.BindTexture(TextureTarget.Texture2D, _fallbackTexture);
				return false;
			}
			else
			{
				GL.BindTexture(TextureTarget.Texture2D, _textureHandles[ActiveTextures][textureIndex]);
				return true;
			}
		}

		public bool BindTextureRaw(TextureUnit unit, int textureIndex)
		{
			GL.ActiveTexture(unit);
			if(textureIndex == 0)
			{
				GL.BindTexture(TextureTarget.Texture2D, _fallbackTexture);
				return false;
			}
			else
			{
				GL.BindTexture(TextureTarget.Texture2D, textureIndex);
				return true;
			}
		}
	}
}
