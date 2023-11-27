using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

namespace SA3D.Rendering.Buffer
{
	/// <summary>
	/// Manages textures of frame buffers
	/// </summary>
	public class TextureFrameBuffer
	{
		private int _framebuffer;
		private int _colorTexture;
		private int _depthStencilRBO;
		private int _depthStencilTexture;
		private Size _resolution;

		private readonly bool _textureDepthStencil;

		/// <summary>
		/// Creates a new texture frame buffer.
		/// </summary>
		/// <param name="textureDepthStencil">Whether to create depth stencil textures.</param>
		public TextureFrameBuffer(bool textureDepthStencil)
		{
			_textureDepthStencil = textureDepthStencil;
		}

		/// <summary>
		/// Generates the buffers.
		/// </summary>
		/// <param name="resolution">Resolution of the textures.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void Generate(Size resolution)
		{
			if((resolution.Width == 0 && resolution.Height == 0) || _resolution == resolution)
			{
				return;
			}

			if(_framebuffer != 0)
			{
				GL.DeleteFramebuffer(_framebuffer);
				GL.DeleteTexture(_colorTexture);
				GL.DeleteRenderbuffer(_depthStencilRBO);
				GL.DeleteTexture(_depthStencilTexture);
			}

			_framebuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

			_colorTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
			GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, resolution.Width, resolution.Height);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _colorTexture, 0);

			if(!_textureDepthStencil)
			{
				_depthStencilTexture = 0;
				_depthStencilRBO = GL.GenRenderbuffer();
				GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthStencilRBO);
				GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, resolution.Width, resolution.Height);
				GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
				GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _depthStencilRBO);
			}
			else
			{
				_depthStencilRBO = 0;
				_depthStencilTexture = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, _depthStencilTexture);
				GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Depth24Stencil8, resolution.Width, resolution.Height);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, _depthStencilTexture, 0);
			}

			FramebufferErrorCode state = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if(state != FramebufferErrorCode.FramebufferComplete)
			{
				throw new InvalidOperationException("Framebuffer not complete!");
			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			_resolution = resolution;
		}

		/// <summary>
		/// Binds the frame buffers, so that succeeding draw calls draw to the buffer.
		/// </summary>
		public void BindFrameBuffer()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
		}

		/// <summary>
		/// Binds the color texture.
		/// </summary>
		public void BindColorTexture()
		{
			GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
		}

		/// <summary>
		/// Binds the depth stencil texture.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public void BindDepthStencilTexture()
		{
			if(!_textureDepthStencil)
			{
				throw new InvalidOperationException("Depth/Stencil buffer not a texture!");
			}

			GL.BindTexture(TextureTarget.Texture2D, _depthStencilTexture);
		}
	}
}
