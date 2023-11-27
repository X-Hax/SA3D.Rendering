using FontStashSharp;
using OpenTK.Graphics.OpenGL4;
using SA3D.Rendering.Buffer;
using System.Drawing;
using System.Numerics;
using Color = SA3D.Modeling.Structs.Color;

namespace SA3D.Rendering.UI
{
	internal class UIRenderer
	{
		private readonly BufferManager _bufferManager;
		public Matrix4x4 ProjectionMatrix { get; private set; }
		private float _nearplane;

		public UIRenderer(BufferManager bufferManager)
		{
			_bufferManager = bufferManager;
		}

		internal void UpdateMatrix(Size viewport, float nearclip, float farclip)
		{
			_nearplane = -nearclip;
			ProjectionMatrix = Matrix4x4.CreatePerspectiveOffCenter(0, viewport.Width, viewport.Height, 0, nearclip, farclip);
		}

		public void RenderText(FontManager fontManager, string text, float fontSize, Vector2 position, float rotation, Color color)
		{
			DynamicSpriteFont font = fontManager.FontSystem.GetFont(fontSize);

			fontManager.Renderer.UIRenderer = this;
			font.DrawText(
				fontManager.Renderer,
				text,
				position,
				new FSColor(color.Red, color.Green, color.Blue, color.Alpha),
				rotation: rotation);
			fontManager.Renderer.UIRenderer = null;
		}

		public void Render(Sprite sprite)
		{
			_bufferManager.BindTexture(TextureUnit.Texture0, sprite.TextureIndex);
			RenderSprite(sprite);
		}

		public void RenderRaw(Sprite sprite)
		{
			_bufferManager.BindTextureRaw(TextureUnit.Texture0, sprite.TextureIndex);
			RenderSprite(sprite);
		}

		private void RenderSprite(Sprite sprite)
		{
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, 0);

			float clipPos = sprite.ClipPos ?? _nearplane;

			Vector2 scale = new(sprite.Size.X * -clipPos, sprite.Size.Y * -clipPos);
			Matrix4x4 modelMatrix = Matrix4x4.CreateScale(scale.X, scale.Y, 1);

			if(sprite.Rotation != 0)
			{
				modelMatrix *= Matrix4x4.CreateTranslation(-0.5f * scale.X, -0.5f * scale.Y, 0);
				modelMatrix *= Matrix4x4.CreateRotationZ(sprite.Rotation);
				modelMatrix *= Matrix4x4.CreateTranslation(0.5f * scale.X, 0.5f * scale.Y, 0);
			}

			modelMatrix *= Matrix4x4.CreateTranslation(sprite.Position.X * -clipPos, sprite.Position.Y * -clipPos, clipPos);
			Matrix4x4 mp = modelMatrix * ProjectionMatrix;

			_bufferManager.SetSpriteInfo(mp, sprite.UVOffset, sprite.UVScale, sprite.Color);

			GL.BlendFunc(sprite.SourceBlendMode.ToGLBlend(), sprite.DestinationBlendMode.ToGLBlend());

			GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
		}
	}
}
