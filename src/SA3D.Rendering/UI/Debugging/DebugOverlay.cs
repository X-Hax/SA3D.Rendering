using FontStashSharp;
using SA3D.Common;
using SA3D.Modeling.Structs;
using SA3D.Rendering.Structs;
using SA3D.Texturing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using Color = SA3D.Modeling.Structs.Color;

namespace SA3D.Rendering.UI.Debugging
{
	/// <summary>
	/// Debug UI menu state.
	/// </summary>
	public enum DebugUIMenu
	{
		/// <summary>
		/// Shows no debug menu
		/// </summary>
		Disabled,

		/// <summary>
		/// Shows the help debug menu
		/// </summary>
		Help,

		/// <summary>
		/// Shows the camera debug menu
		/// </summary>
		Camera,

		/// <summary>
		/// Shows the render debug menu
		/// </summary>
		RenderInfo
	}

	/// <summary>
	/// Canvas that displays debug panels with various info.
	/// </summary>
	public class DebugOverlay : Canvas
	{
		private readonly Queue<double> _deltas = new();
		private double _deltasAdded;
		private DebugRenderMode _renderMode;

		/// <summary>
		/// Current render mode of the <see cref="Shaders.Shaders.SurfaceDebug"/> shader
		/// </summary>
		public DebugRenderMode RenderMode
		{
			get => _renderMode;
			set
			{
				_renderMode = value;
				Shaders.Shaders.SurfaceDebug.SetUniform("debugMode", (int)value);
			}
		}

		/// <summary>
		/// Current debug menu being displayed.
		/// </summary>
		public DebugUIMenu Menu { get; set; }

		/// <summary>
		/// Scale of the menu.
		/// </summary>
		public float Scale = 1.5f;

		private static readonly TextureSet _debugTextures;

		static DebugOverlay()
		{
			_debugTextures = new(new[]
			{
				DebugBackgroundGenerator.GenerateTexture(160, 100),
				DebugBackgroundGenerator.GenerateTexture(340, 150),
				DebugBackgroundGenerator.GenerateTexture(340, 120)
			});
		}

		/// <summary>
		/// Creates a new debug overlay.
		/// </summary>
		/// <exception cref="NullReferenceException"></exception>
		public DebugOverlay() : base(_debugTextures)
		{
			FontSystem fontSystem = new(new()
			{
				FontResolutionFactor = 2,
				KernelWidth = 2,
				KernelHeight = 2
			});

			Assembly assembly = Assembly.GetExecutingAssembly();
			const string resourceName = "SA3D.Rendering.UI.Debugging.CascadiaCode.ttf";

			using(Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new NullReferenceException("Cannot find font"))
			{
				fontSystem.AddFont(stream);
			}

			FontManager = new(fontSystem);
		}

		/// <inheritdoc/>
		protected override void Render(RenderContext context)
		{
			switch(Menu)
			{
				case DebugUIMenu.Help:
					RenderHelpMenu();
					break;
				case DebugUIMenu.Camera:
					RenderCameraMenu(context.Camera);
					break;
				case DebugUIMenu.RenderInfo:
					RenderRenderMenu(context);
					break;
				case DebugUIMenu.Disabled:
				default:
					break;
			}
		}

		/// <summary>
		/// Captures the number of seconds between frames to calculate the number of FPS.
		/// </summary>
		/// <param name="delta">Seconds since the previous frame was drawn.</param>
		public void CaptureDelta(double delta)
		{
			_deltas.Enqueue(delta);
			_deltasAdded += delta;
			while(_deltasAdded > 1)
			{
				_deltasAdded -= _deltas.Dequeue();
			}
		}

		private void RenderBackground(int textureIndex)
		{
			Texture texture = Textures.Textures[textureIndex];
			Vector2 size = new(texture.Width * Scale, texture.Height * Scale);
			DrawSprite(new()
			{
				TextureIndex = textureIndex,
				Size = size,
				Color = new Color(255, 255, 255, 168),
			});
		}

		private void DrawScaledText(string text, float fontSize, Vector2 position)
		{
			DrawText(text, fontSize * Scale, position * Scale);
		}

		private void RenderMenu(int textureIndex, string title, string[] lines)
		{
			if(FontManager == null)
			{
				return;
			}

			RenderBackground(textureIndex);
			Texture texture = Textures.Textures[textureIndex];

			const float titleFontSize = 20;
			const float fontSize = 15;
			const float padding = 2;
			float offset = 35;

			float length = FontManager.FontSystem.GetFont(titleFontSize).MeasureString(title).X;
			DrawScaledText(title, titleFontSize, new((texture.Width - length) * 0.5f, 10));

			foreach(string line in lines)
			{
				DrawScaledText(line, fontSize, new(16, offset));
				offset += fontSize + padding;
			}
		}

		private void RenderHelpMenu()
		{
			RenderMenu(
				0,
				"== Debug ==",
				new string[]
				{
					"F1 - Debug",
					"F2 - Camera",
					"F3 - Renderinfo"
				}
			);
		}

		private void RenderCameraMenu(Camera camera)
		{
			RenderMenu(
				1,
				"== Camera == ",
				new string[]
				{
					$"Location: {camera.Position.DebugString()}",
					$"Rotation: {camera.Rotation.DebugString()}",
					$"Distance: {camera.Distance}",
					$"View type: " + (camera.Orthographic ? "Orthographic" : $"Perspective - FoV: {MathHelper.RadToDeg(camera.FieldOfView):F3}"),
					$"Clipping planes: {camera.NearPlane:F3} -> {camera.FarPlane:F3}",
					$"Nav mode: " + (camera.Orbiting ? "Orbiting" : "First Person"),
				}
			);
		}

		private void RenderRenderMenu(RenderContext context)
		{
			RenderMenu(
				2,
				"== Rendering == ",
				new string[]
				{
					$"FPS: {_deltas.Count / _deltasAdded:f1}",
					$"View Pos.: {context.Camera.Realposition.DebugString()}",
					$"Render Mode: {RenderMode}",
					$"Wireframe Mode: {context.WireFrameMode}"
				}
			);
		}
	}
}
