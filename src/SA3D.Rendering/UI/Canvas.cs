using SA3D.Rendering.Shaders;
using SA3D.Texturing;
using System;
using System.Numerics;
using Color = SA3D.Modeling.Structs.Color;

namespace SA3D.Rendering.UI
{
	/// <summary>
	/// Canvas for drawing UI elements on top of the 3D models.
	/// </summary>
	public abstract class Canvas
	{
		private bool _initialized;

		private UIRenderer? _uiRenderer;

		private UIRenderer UIRenderer
			=> _uiRenderer ?? throw new InvalidOperationException("Canvas not rendering right now!");

		/// <summary>
		/// Projection matrix being applied on the UI elements.
		/// </summary>
		protected Matrix4x4 ProjectionMatrix
			=> UIRenderer.ProjectionMatrix;

		/// <summary>
		/// Custom shader for rendering UI elements.
		/// </summary>
		protected Shader? ActiveShader { get; set; }

		/// <summary>
		/// The currently used shader for rendering UI elements. 
		/// <br/> Returns <see cref="ActiveShader"/> if not null, otherwise <see cref="Shaders.Shaders.Sprite"/>
		/// </summary>
		protected Shader UsedShader => ActiveShader ?? Shaders.Shaders.Sprite;

		/// <summary>
		/// Font manager for drawing texts.
		/// </summary>
		protected FontManager? FontManager { get; set; }

		/// <summary>
		/// Textures of the canvas to use.
		/// </summary>
		public TextureSet Textures { get; }

		/// <summary>
		/// Whether to enable depth checking on the canvas.
		/// </summary>
		public virtual bool EnableDepthCheck => false;

		/// <summary>
		/// Creates a new canvas.
		/// </summary>
		/// <param name="textures"></param>
		protected Canvas(TextureSet textures)
		{
			Textures = textures;
		}

		internal void InternalInitialize()
		{
			if(_initialized)
			{
				return;
			}

			_initialized = true;
			Initialize();
		}

		internal void Render(RenderContext context, UIRenderer uiRenderer)
		{
			_uiRenderer = uiRenderer;
			Render(context);
			_uiRenderer = null;
		}

		/// <summary>
		/// Draws text to the canvas.
		/// </summary>
		/// <param name="text">The text to draw.</param>
		/// <param name="fontSize">Size of the font in em(?).</param>
		/// <param name="position">Position of the text in pixels from the top left screen corner.</param>
		protected void DrawText(string text, float fontSize, Vector2 position)
		{
			DrawText(text, fontSize, position, 0, Color.ColorWhite);
		}

		/// <summary>
		/// Draws text to the canvas.
		/// </summary>
		/// <param name="text">The text to draw.</param>
		/// <param name="fontSize">Size of the font in em(?).</param>
		/// <param name="position">Position of the text in pixels from the top left screen corner.</param>
		/// <param name="rotation">Rotation of the text in radians around the center.</param>
		/// <param name="color">Color of the text.</param>
		/// <exception cref="InvalidOperationException"></exception>
		protected void DrawText(string text, float fontSize, Vector2 position, float rotation, Color color)
		{
			if(FontManager == null)
			{
				throw new InvalidOperationException("No fontmanager set!");
			}

			UsedShader.Use();
			UIRenderer.RenderText(FontManager, text, fontSize, position, rotation, color);
		}


		/// <summary>
		/// Draws an image to the canvas.
		/// </summary>
		/// <param name="sprite">The image info.</param>
		protected void DrawSprite(Sprite sprite)
		{
			UsedShader.Use();
			UIRenderer.Render(sprite);
		}

		/// <summary>
		/// Draw a solid color rectangle to the canvas.
		/// </summary>
		/// <param name="rectangle">The rectangle to draw.</param>
		protected void DrawRectangle(RectangleSprite rectangle)
		{
			UsedShader.Use();

			Sprite sprite = new()
			{
				Position = rectangle.Position,
				Size = rectangle.Size,
				ClipPos = rectangle.ClipPos,
				Rotation = rectangle.Rotation,
				Color = rectangle.Color,
				SourceBlendMode = rectangle.SourceBlendMode,
				DestinationBlendMode = rectangle.DestinationBlendMode
			};

			UIRenderer.RenderRaw(sprite);
		}

		/// <summary>
		/// Gets called on graphics initialization. Does nothing by default.
		/// </summary>
		protected virtual void Initialize()
		{

		}

		/// <summary>
		/// Renders the canvas to a render context.
		/// </summary>
		/// <param name="context">The context to render to.</param>
		protected abstract void Render(RenderContext context);
	}
}
