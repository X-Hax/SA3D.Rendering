using OpenTK.Graphics.OpenGL4;
using SA3D.Rendering.Buffer;
using SA3D.Rendering.Input;
using SA3D.Rendering.Shaders;
using SA3D.Rendering.Structs;
using SA3D.Rendering.UI;
using SA3D.Texturing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using System.Threading;
using Color = SA3D.Modeling.Structs.Color;

namespace SA3D.Rendering
{
	/// <summary>
	/// Rendering context.
	/// </summary>
	public partial class RenderContext
	{
		#region Private fields

		private static RenderContext? _rendering;

		private Size _viewport;
		private RenderSettings _settings;
		private readonly Lighting[] _lighting;
		private TextureSet? _activeTextures;
		private Shader? _customMeshShader;
		private Shader? _customBillBoardShader;

		private bool _initialized;

		private readonly BufferManager _bufferManager;
		private readonly UIRenderer _uiRenderer;
		private readonly List<Canvas> _canvases;

		#endregion

		#region Properties

		/// <summary>
		/// Whether the context is currently rendering.
		/// </summary>
		public bool IsRendering
			=> _rendering == this;

		/// <summary>
		/// Viewport determining the output resolution of the context in pixels.
		/// </summary>
		public Size Viewport
		{
			get => _viewport;
			set
			{
				if(IsRendering)
				{
					throw new InvalidOperationException("Cannot change viewport while rendering!");
				}

				_viewport = value;
				Camera.Aspect = value.Width / (float)value.Height;

				if(_initialized)
				{
					OITBuffer.Setup(value);
				}
			}
		}

		/// <summary>
		/// Returns the viewport as a two-component floating point vector.
		/// </summary>
		public Vector2 FloatViewport
			=> new(Viewport.Width, Viewport.Height);

		/// <summary>
		/// Color of the render background.
		/// </summary>
		public Color BackgroundColor { get; set; }

		/// <summary>
		/// Render settings being applied.
		/// </summary>
		public RenderSettings Settings
		{
			get => _settings;
			set
			{
				if(IsRendering && !value.Equals(_settings))
				{
					_bufferManager.SetRenderSettings(value);
				}

				_settings = value;
			}
		}

		/// <summary>
		/// Wireframe rendering mode.
		/// </summary>
		public WireFrameMode WireFrameMode { get; set; }

		/// <summary>
		/// Lighting.
		/// </summary>
		public ReadOnlyCollection<Lighting> Lighting { get; }

		/// <summary>
		/// Currently used shader for rendering 3D meshes.
		/// </summary>
		public Shader MeshShader
			=> _customMeshShader ?? Shaders.Shaders.Surface;

		/// <summary>
		/// Currently used shader for rendering billboards.
		/// </summary>
		public Shader BillBoardShader
			=> _customBillBoardShader ?? Shaders.Shaders.Billboard;

		/// <summary>
		/// Currently active texture set,
		/// </summary>
		public TextureSet? ActiveTextures
		{
			get => _activeTextures;
			set
			{
				_activeTextures = value;
				if(IsRendering)
				{
					_bufferManager.ActiveTextures = _activeTextures;
				}
			}
		}

		/// <summary>
		/// Camera used to render the models..
		/// </summary>
		public Camera Camera { get; }

		/// <summary>
		/// Input manager.
		/// </summary>
		public InputManager Input { get; }

		/// <summary>
		/// Order independent transparency buffer.
		/// </summary>
		public OITBuffer OITBuffer { get; }

		#endregion

		#region Events

		/// <summary>
		/// Event delegate for the contexts
		/// </summary>
		/// <param name="context">Context being initialized.</param>
		public delegate void ContextEvent(RenderContext context);

		/// <summary>
		/// Event delegate for contexts with delta.
		/// </summary>
		/// <param name="context">Context performing the update.</param>
		/// <param name="delta">Seconds since the last update.</param>
		public delegate void ContextDeltaEvent(RenderContext context, double delta);

		/// <summary>
		/// Invoked upon initializing the graphics library.
		/// </summary>
		public event ContextEvent? OnInitialize;

		/// <summary>
		/// Invoked on update.
		/// </summary>
		public event ContextDeltaEvent? OnUpdate;

		/// <summary>
		/// Invoked on render (before Rendering 2D).
		/// </summary>
		public event ContextEvent? OnRender;

		#endregion

		/// <summary>
		/// Creates a new render context.
		/// </summary>
		/// <param name="viewport">Viewport determining the output resolution of the context in pixels.</param>
		public RenderContext(Size viewport)
		{
			_viewport = viewport;

			_bufferManager = new();
			_uiRenderer = new(_bufferManager);
			OITBuffer = new();
			_canvases = [];

			Input = new();
			Camera = new(viewport.Width / (float)viewport.Height);
			_lighting = new Lighting[4];
			_lighting[0] = new(Vector3.UnitY, 1, new(0xFF, 0xFF, 0xFF), 1, new(0x40, 0x40, 0x40));
			Lighting = new(_lighting);
		}

		/// <summary>
		/// Initializes the graphics library backend.
		/// </summary>
		public void Initialize()
		{
			if(_initialized)
			{
				return;
			}

			GL.DepthFunc(DepthFunction.Lequal);
			GL.Enable(EnableCap.Multisample);

			Shaders.Shaders.Compile();
			Blit.Initialize();

			_bufferManager.Initialize();
			OITBuffer.Setup(_viewport);

			OnInitialize?.Invoke(this);

			foreach(Canvas canvas in _canvases)
			{
				_bufferManager.BufferTextures(canvas.Textures);
				canvas.InternalInitialize();
			}

			_initialized = true;
		}

		/// <summary>
		/// Updates the context.
		/// </summary>
		/// <param name="focused">Whether the context is focused.</param>
		/// <param name="delta">Seconds since the last update.</param>
		public void Update(bool focused, double delta)
		{
			Input.Update(focused);
			OnUpdate?.Invoke(this, delta);
		}

		private void Render3D(int frameBufferHandle)
		{
			GL.Disable(EnableCap.Blend);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthMask(true);

			OITBuffer.Use();
			OITBuffer.Reset();

			_bufferManager.SetRenderSettings(_settings);
			_bufferManager.SetShaderLighting(_lighting);
			_bufferManager.SetShaderCamera(Camera);
			_bufferManager.BindShaderBuffers();

			switch(WireFrameMode)
			{
				case WireFrameMode.ReplaceLine:
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
					break;
				case WireFrameMode.ReplacePoint:
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
					break;
				case WireFrameMode.None:
				case WireFrameMode.Overlay:
				default:
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
					break;
			}

			OnRender?.Invoke(this);

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferHandle);
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			OITBuffer.CompositeBlit();
		}

		private void Render2D()
		{
			_bufferManager.BindSpriteBuffers();
			_uiRenderer.UpdateMatrix(_viewport, Camera.NearPlane, Camera.FarPlane);
			GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthMask(false);

			foreach(Canvas canvas in _canvases)
			{
				if(!canvas.EnableDepthCheck)
				{
					continue;
				}

				ActiveTextures = canvas.Textures;
				canvas.Render(this, _uiRenderer);
			}

			GL.Disable(EnableCap.DepthTest);
			foreach(Canvas canvas in _canvases)
			{
				if(canvas.EnableDepthCheck)
				{
					continue;
				}

				ActiveTextures = canvas.Textures;
				canvas.Render(this, _uiRenderer);
			}
		}

		/// <summary>
		/// Renders the context.
		/// </summary>
		/// <param name="frameBufferHandle">Frame buffer to render to. 0 renders to default buffer.</param>
		public void Render(int frameBufferHandle = 0)
		{
			while(_rendering != null)
			{
				Thread.Sleep(10);
			}

			_rendering = this;

			// setup
			GL.Viewport(_viewport);
			GL.ClearColor(BackgroundColor.SystemColor);

			Render3D(frameBufferHandle);
			Render2D();

			_rendering = null;
		}

	}
}
