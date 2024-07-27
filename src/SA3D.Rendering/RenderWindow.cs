using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using SA3D.Rendering.Input;
using System;
using System.Collections.Generic;
using TKey = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using TMouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;

namespace SA3D.Rendering
{
	/// <summary>
	/// Manages a window that displays the rendered frames.
	/// </summary>
	public class RenderWindow : GameWindow
	{
		private static readonly Dictionary<TKey, InputCode> _keyMap = new()
		{
			{ TKey.Unknown, InputCode.None },
			{ TKey.Space, InputCode.Space },
			{ TKey.Apostrophe, InputCode.OemQuotes },
			{ TKey.Comma, InputCode.OemComma },
			{ TKey.Minus, InputCode.OemMinus },
			{ TKey.Period, InputCode.OemPeriod },
			{ TKey.Slash, InputCode.OemQuestion },
			{ TKey.D0, InputCode.D0 },
			{ TKey.D1, InputCode.D1 },
			{ TKey.D2, InputCode.D2 },
			{ TKey.D3, InputCode.D3 },
			{ TKey.D4, InputCode.D4 },
			{ TKey.D5, InputCode.D5 },
			{ TKey.D6, InputCode.D6 },
			{ TKey.D7, InputCode.D7 },
			{ TKey.D8, InputCode.D8 },
			{ TKey.D9, InputCode.D9 },
			{ TKey.Semicolon, InputCode.OemSemicolon },
			{ TKey.Equal, InputCode.OemPlus },
			{ TKey.A, InputCode.A },
			{ TKey.B, InputCode.B },
			{ TKey.C, InputCode.C },
			{ TKey.D, InputCode.D },
			{ TKey.E, InputCode.E },
			{ TKey.F, InputCode.F },
			{ TKey.G, InputCode.G },
			{ TKey.H, InputCode.H },
			{ TKey.I, InputCode.I },
			{ TKey.J, InputCode.J },
			{ TKey.K, InputCode.K },
			{ TKey.L, InputCode.L },
			{ TKey.M, InputCode.M },
			{ TKey.N, InputCode.N },
			{ TKey.O, InputCode.O },
			{ TKey.P, InputCode.P },
			{ TKey.Q, InputCode.Q },
			{ TKey.R, InputCode.R },
			{ TKey.S, InputCode.S },
			{ TKey.T, InputCode.T },
			{ TKey.U, InputCode.U },
			{ TKey.V, InputCode.V },
			{ TKey.W, InputCode.W },
			{ TKey.X, InputCode.X },
			{ TKey.Y, InputCode.Y },
			{ TKey.Z, InputCode.Z },
			{ TKey.LeftBracket, InputCode.OemOpenBrackets },
			{ TKey.Backslash, InputCode.OemBackslash },
			{ TKey.RightBracket, InputCode.OemCloseBrackets },
			{ TKey.GraveAccent, InputCode.OemTilde },
			{ TKey.Escape, InputCode.Escape },
			{ TKey.Enter, InputCode.Enter },
			{ TKey.Tab, InputCode.Tab },
			{ TKey.Backspace, InputCode.Back },
			{ TKey.Insert, InputCode.Insert },
			{ TKey.Delete, InputCode.Delete },
			{ TKey.Right, InputCode.Right },
			{ TKey.Left, InputCode.Left },
			{ TKey.Down, InputCode.Down },
			{ TKey.Up, InputCode.Up },
			{ TKey.PageUp, InputCode.PageUp },
			{ TKey.PageDown, InputCode.PageDown },
			{ TKey.Home, InputCode.Home },
			{ TKey.End, InputCode.End },
			{ TKey.CapsLock, InputCode.CapsLock },
			{ TKey.ScrollLock, InputCode.Scroll },
			{ TKey.NumLock, InputCode.NumLock },
			{ TKey.PrintScreen, InputCode.PrintScreen },
			{ TKey.Pause, InputCode.Pause },
			{ TKey.F1, InputCode.F1 },
			{ TKey.F2, InputCode.F2 },
			{ TKey.F3, InputCode.F3 },
			{ TKey.F4, InputCode.F4 },
			{ TKey.F5, InputCode.F5 },
			{ TKey.F6, InputCode.F6 },
			{ TKey.F7, InputCode.F7 },
			{ TKey.F8, InputCode.F8 },
			{ TKey.F9, InputCode.F9 },
			{ TKey.F10, InputCode.F10 },
			{ TKey.F11, InputCode.F11 },
			{ TKey.F12, InputCode.F12 },
			{ TKey.F13, InputCode.F13 },
			{ TKey.F14, InputCode.F14 },
			{ TKey.F15, InputCode.F15 },
			{ TKey.F16, InputCode.F16 },
			{ TKey.F17, InputCode.F17 },
			{ TKey.F18, InputCode.F18 },
			{ TKey.F19, InputCode.F19 },
			{ TKey.F20, InputCode.F20 },
			{ TKey.F21, InputCode.F21 },
			{ TKey.F22, InputCode.F22 },
			{ TKey.F23, InputCode.F23 },
			{ TKey.F24, InputCode.F24 },
			{ TKey.F25, InputCode.None },
			{ TKey.KeyPad0, InputCode.NumPad0 },
			{ TKey.KeyPad1, InputCode.NumPad1 },
			{ TKey.KeyPad2, InputCode.NumPad2 },
			{ TKey.KeyPad3, InputCode.NumPad3 },
			{ TKey.KeyPad4, InputCode.NumPad4 },
			{ TKey.KeyPad5, InputCode.NumPad5 },
			{ TKey.KeyPad6, InputCode.NumPad6 },
			{ TKey.KeyPad7, InputCode.NumPad7 },
			{ TKey.KeyPad8, InputCode.NumPad8 },
			{ TKey.KeyPad9, InputCode.NumPad9 },
			{ TKey.KeyPadDecimal, InputCode.Decimal },
			{ TKey.KeyPadDivide, InputCode.Divide },
			{ TKey.KeyPadMultiply, InputCode.Multiply },
			{ TKey.KeyPadSubtract, InputCode.Subtract },
			{ TKey.KeyPadAdd, InputCode.Add },
			{ TKey.KeyPadEnter, InputCode.Enter },
			{ TKey.KeyPadEqual, InputCode.None }, // ?
            { TKey.LeftShift, InputCode.LeftShift },
			{ TKey.LeftControl, InputCode.LeftCtrl },
			{ TKey.LeftAlt, InputCode.LeftAlt },
			{ TKey.LeftSuper, InputCode.LWin },
			{ TKey.RightShift, InputCode.RightShift },
			{ TKey.RightControl, InputCode.RightCtrl },
			{ TKey.RightAlt, InputCode.RightAlt },
			{ TKey.RightSuper, InputCode.RWin },
			{ TKey.Menu, InputCode.None }
		};

		private static readonly Dictionary<TMouseButton, InputCode> _mouseButtonMap = new()
		{
			{ TMouseButton.Left, InputCode.MouseLeft },
			{ TMouseButton.Middle, InputCode.MouseMiddle },
			{ TMouseButton.Right, InputCode.MouseRight },
			{ TMouseButton.Button4, InputCode.MouseXButton1 },
			{ TMouseButton.Button5, InputCode.MouseXButton2 }
		};

		private readonly RenderContext _context;

		private bool _mouseLocked;
		private Vector2 _center;

		/// <summary>
		/// Creates a new render window.
		/// </summary>
		/// <param name="context">The rendering context to use.</param>
		/// <param name="title">Title of the window.</param>
		/// <param name="icon">Icon to use.</param>
		/// <param name="UseVsync">Whether to render using vertical syncronization.</param>
		public RenderWindow(RenderContext context, string title = "SA3D", WindowIcon? icon = null, bool UseVsync = true)
			: base(
				new(),
				new()
				{
					API = ContextAPI.OpenGL,
					APIVersion = new Version(4, 6),
					Title = title,
					ClientSize = new(context.Viewport.Width, context.Viewport.Height),
					Flags = ContextFlags.ForwardCompatible,
					WindowBorder = WindowBorder.Resizable,
					Icon = icon
				})
		{
			if(!UseVsync)
			{
				VSync = VSyncMode.Off;
			}

			_context = context;

			_context.Input.OnSetCursorPosition += (v2) =>
			{
				if(!_mouseLocked)
				{
					MousePosition = new(v2.X, v2.Y);
				}
			};

			_context.Input.OnSetMouselock += (v) =>
			{
				_mouseLocked = v;
				CursorState = v ? CursorState.Hidden : CursorState.Normal;
			};

			_center = Size / 2;
		}

		/// <inheritdoc/>
		protected override void OnLoad()
		{
			base.OnLoad();
			_context.Initialize();
		}

		/// <inheritdoc/>
		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			_context.Viewport = new(ClientSize.X, ClientSize.Y);
			_center = Size / 2;
		}

		/// <inheritdoc/>
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
			_context.Update(IsFocused, e.Time);
			if(_mouseLocked)
			{
				MousePosition = _center;
			}
		}

		/// <inheritdoc/>
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			_context.Render();
			Context.SwapBuffers();
		}

		#region Input handling

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);
			if(_keyMap.TryGetValue(e.Key, out InputCode code))
			{
				_context.Input.SetInput(code, true);
			}
		}

		/// <inheritdoc/>
		protected override void OnKeyUp(KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
			if(_keyMap.TryGetValue(e.Key, out InputCode code))
			{
				_context.Input.SetInput(code, false);
			}
		}

		/// <inheritdoc/>
		protected override void OnFocusedChanged(FocusedChangedEventArgs e)
		{
			base.OnFocusedChanged(e);
			if(!e.IsFocused)
			{
				_context.Input.ClearInputs();
			}
		}

		/// <inheritdoc/>
		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			Vector2 pos = e.Position;
			if(_mouseLocked)
			{
				_context.Input.SetCursorPos(new(pos.X, pos.Y), new(_center.X, _center.Y));
			}
			else
			{
				_context.Input.SetCursorPos(new(pos.X, pos.Y), null);
			}
		}

		/// <inheritdoc/>
		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			_context.Input.ClearInputs();
		}

		/// <inheritdoc/>
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			_context.Input.SetScroll(e.Offset.Y);
		}

		/// <inheritdoc/>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if(_mouseButtonMap.TryGetValue(e.Button, out InputCode code))
			{
				_context.Input.SetInput(code, true);
			}
		}

		/// <inheritdoc/>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if(_mouseButtonMap.TryGetValue(e.Button, out InputCode code))
			{
				_context.Input.SetInput(code, false);
			}
		}

		#endregion

	}
}
