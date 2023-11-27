using FontStashSharp;
using FontStashSharp.Interfaces;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Color = SA3D.Modeling.Structs.Color;

namespace SA3D.Rendering.UI
{
	/// <summary>
	/// Manages fonts and their buffers.
	/// </summary>
	public class FontManager
	{
		/// <summary>
		/// Font system storing all used fonts.
		/// </summary>
		public FontSystem FontSystem { get; }

		internal FontRenderer Renderer { get; }

		/// <summary>
		/// Creates a new font manager.
		/// </summary>
		/// <param name="fontSystem"></param>
		public FontManager(FontSystem fontSystem)
		{
			FontSystem = fontSystem;
			Renderer = new();
		}

		/// <summary>
		/// Resets the buffered fonts.
		/// </summary>
		public void Cleanup()
		{
			Renderer.DeleteTextures();
			FontSystem.Reset();
		}

		internal class FontRenderer : IFontStashRenderer2, ITexture2DManager
		{
			private readonly Dictionary<int, (int handle, int width, int height)> _textTextureHandles;

			public ITexture2DManager TextureManager => this;
			internal UIRenderer? UIRenderer { get; set; }

			public FontRenderer()
			{
				_textTextureHandles = new();
			}

			public void DeleteTextures()
			{
				foreach(KeyValuePair<int, (int handle, int width, int height)> item in _textTextureHandles)
				{
					GL.DeleteBuffer(item.Key);
				}
			}

			public void DrawQuad(
				object texture,
				ref VertexPositionColorTexture topLeft,
				ref VertexPositionColorTexture topRight,
				ref VertexPositionColorTexture bottomLeft,
				ref VertexPositionColorTexture bottomRight)
			{
				int textureHandle = _textTextureHandles[(int)texture].handle;
				Vector2 position = new(topLeft.Position.X, topLeft.Position.Y);
				float xSize = topRight.Position.X - topLeft.Position.X;
				float ySize = bottomLeft.Position.Y - topLeft.Position.Y;

				Color color = new(topLeft.Color.R, topLeft.Color.G, topLeft.Color.B, topLeft.Color.A);

				Vector2 uvOffset = topLeft.TextureCoordinate;
				Vector2 uvScale = bottomRight.TextureCoordinate - topLeft.TextureCoordinate;

				if(UIRenderer == null)
				{
					throw new InvalidOperationException("Sprite renderer not set");
				}

				UIRenderer.RenderRaw(new()
				{
					TextureIndex = textureHandle,
					Position = position,
					Size = new(xSize, ySize),
					Color = color,
					UVOffset = uvOffset,
					UVScale = uvScale
				});
			}

			public object CreateTexture(int width, int height)
			{
				int textureHandle = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, textureHandle);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
				_textTextureHandles.Add(textureHandle, new(textureHandle, width, height));
				return textureHandle;
			}

			public Point GetTextureSize(object texture)
			{
				(_, int width, int height) = _textTextureHandles[(int)texture];
				return new(width, height);
			}

			public unsafe void SetTextureData(object texture, Rectangle bounds, byte[] data)
			{
				GL.BindTexture(TextureTarget.Texture2D, _textTextureHandles[(int)texture].handle);
				fixed(byte* ptr = data)
				{
					GL.TexSubImage2D(
						target: TextureTarget.Texture2D,
						level: 0,
						xoffset: bounds.Left,
						yoffset: bounds.Top,
						width: bounds.Width,
						height: bounds.Height,
						format: PixelFormat.Rgba,
						type: PixelType.UnsignedByte,
						pixels: new nint(ptr)
					);
				}
			}
		}
	}
}
