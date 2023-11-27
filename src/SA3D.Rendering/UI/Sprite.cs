using SA3D.Modeling.Mesh;
using SA3D.Modeling.Structs;
using System.Numerics;

namespace SA3D.Rendering.UI
{
	/// <summary>
	/// Solid color UI element.
	/// </summary>
	public readonly struct RectangleSprite
	{
		/// <summary>
		/// Position from the top left of the screen in pixels.
		/// </summary>
		public Vector2 Position { get; init; }

		/// <summary>
		/// Clipping position. Null equals to near plane.
		/// </summary>
		public float? ClipPos { get; init; }

		/// <summary>
		/// Size in pixels.
		/// </summary>
		public Vector2 Size { get; init; }

		/// <summary>
		/// Rotation around the center in radians.
		/// </summary>
		public float Rotation { get; init; }

		/// <summary>
		/// Color to render.
		/// </summary>
		public Color Color { get; init; }

		/// <summary>
		/// Source blend mode.
		/// </summary>
		public BlendMode SourceBlendMode { get; init; }

		/// <summary>
		/// Destination blend mode.
		/// </summary>
		public BlendMode DestinationBlendMode { get; init; }

		/// <summary>
		/// Creates a new rectangle sprite.
		/// </summary>
		public RectangleSprite()
		{
			Position = Vector2.Zero;
			Size = Vector2.One;
			ClipPos = null;
			Rotation = 0;
			Color = Color.ColorWhite;
			SourceBlendMode = BlendMode.SrcAlpha;
			DestinationBlendMode = BlendMode.SrcAlphaInverted;
		}
	}

	/// <summary>
	/// Image UI element.
	/// </summary>
	public readonly struct Sprite
	{
		/// <summary>
		/// Index to the texture to draw.
		/// </summary>
		public int TextureIndex { get; init; }


		/// <summary>
		/// Position from the top left of the screen in pixels.
		/// </summary>
		public Vector2 Position { get; init; }

		/// <summary>
		/// Clipping position. Null equals to near plane.
		/// </summary>
		public float? ClipPos { get; init; }

		/// <summary>
		/// Size in pixels.
		/// </summary>
		public Vector2 Size { get; init; }

		/// <summary>
		/// Rotation around the center in radians.
		/// </summary>
		public float Rotation { get; init; }

		/// <summary>
		/// Color to render.
		/// </summary>
		public Color Color { get; init; }

		/// <summary>
		/// Texture UV offset.
		/// </summary>
		public Vector2 UVOffset { get; init; }

		/// <summary>
		/// Texture UV scale.
		/// </summary>
		public Vector2 UVScale { get; init; }

		/// <summary>
		/// Source blend mode.
		/// </summary>
		public BlendMode SourceBlendMode { get; init; }

		/// <summary>
		/// Destination blend mode.
		/// </summary>
		public BlendMode DestinationBlendMode { get; init; }

		/// <summary>
		/// Creates a new sprite.
		/// </summary>
		public Sprite()
		{
			TextureIndex = 0;
			Position = Vector2.Zero;
			Size = Vector2.One;
			ClipPos = null;
			Rotation = 0;
			Color = Color.ColorWhite;
			UVOffset = Vector2.Zero;
			UVScale = Vector2.One;
			SourceBlendMode = BlendMode.SrcAlpha;
			DestinationBlendMode = BlendMode.SrcAlphaInverted;
		}
	}
}
