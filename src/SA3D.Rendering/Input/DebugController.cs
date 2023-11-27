using SA3D.Rendering.UI.Debugging;
using System;

namespace SA3D.Rendering.Input
{
	/// <summary>
	/// Controller for debug menus and states.
	/// </summary>
	public class DebugController : BaseController<DebugController.Action>
	{
		/// <summary>
		/// Debug controller actions.
		/// </summary>
		public enum Action
		{
			/// <summary>
			/// Displays the debug help menu.
			/// </summary>
			DebugHelp,

			/// <summary>
			/// Displays the debug camera menu.
			/// </summary>
			DebugCamera,

			/// <summary>
			/// Displays the debug render menu.
			/// </summary>
			DebugRender,


			/// <summary>
			/// Circles between the various render modes.
			/// </summary>
			CircleRenderMode,

			/// <summary>
			/// Circles between the various wireframe modes.
			/// </summary>
			CircleWireframe,

			/// <summary>
			/// Displays geometry bounds.
			/// </summary>
			CircleBackward,
		}

		/// <summary>
		/// Render context being affected.
		/// </summary>
		public RenderContext Context { get; }

		/// <summary>
		/// Debug overlay being controlled.
		/// </summary>
		public DebugOverlay Overlay { get; }

		/// <summary>
		/// Creates a new debug controller for the given render context.
		/// </summary>
		/// <param name="context">Render context to affect.</param>
		public DebugController(RenderContext context)
			: base(context.Input, new()
			{
				{ Action.DebugHelp, InputCode.F1 },
				{ Action.DebugCamera, InputCode.F2 },
				{ Action.DebugRender, InputCode.F3 },

				{ Action.CircleRenderMode, InputCode.F5 },
				{ Action.CircleWireframe, InputCode.F6 },

				{ Action.CircleBackward, InputCode.RightShift },
			})
		{
			Context = context;
			Overlay = new();
		}

		/// <summary>
		/// Used to circle through enums
		/// </summary>
		/// <typeparam name="T">Enum type</typeparam>
		/// <param name="current">Current value to change</param>
		/// <param name="back">Whether to scroll back</param>
		/// <returns></returns>
		private static T Circle<T>(T current, bool back) where T : Enum
		{
			int max = Enum.GetValues(typeof(T)).Length - 1;
			int value = Convert.ToInt32(current);
			_ = back ? value-- : value++;

			if(value < 0)
			{
				value = max;
			}
			else if(value > max)
			{
				value = 0;
			}

			return (T)Enum.ToObject(typeof(T), value);
		}

		/// <summary>
		/// Runs the controller.
		/// </summary>
		/// <param name="delta">Seconds since the last update cycle/frame.</param>
		public void Run(double delta)
		{
			Overlay.CaptureDelta(delta);
			bool backward = IsDown(Action.CircleBackward);

			DebugUIMenu? newMenu = null;
			if(IsPressed(Action.DebugHelp))
			{
				newMenu = DebugUIMenu.Help;
			}
			else if(IsPressed(Action.DebugCamera))
			{
				newMenu = DebugUIMenu.Camera;
			}
			else if(IsPressed(Action.DebugRender))
			{
				newMenu = DebugUIMenu.RenderInfo;
			}

			if(newMenu != null)
			{
				Overlay.Menu = newMenu == Overlay.Menu ? DebugUIMenu.Disabled : newMenu.Value;
			}

			if(IsPressed(Action.CircleRenderMode))
			{
				Overlay.RenderMode = Circle(Overlay.RenderMode, backward);
			}

			if(IsPressed(Action.CircleWireframe))
			{
				Context.WireFrameMode = Circle(Context.WireFrameMode, backward);
			}
		}
	}
}
