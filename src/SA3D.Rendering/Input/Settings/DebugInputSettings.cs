using System.Collections.Generic;
using System.Configuration;

namespace SA3D.Rendering.Input.Settings
{
	/// <summary>
	/// Debug input settings.
	/// </summary>
	public class DebugInputSettings : CameraInputSettings
	{

		/// <summary>
		/// Debug Help
		/// <br/> Displays the debug help menu.
		/// </summary>
		[UserScopedSetting]
		[InputCodeCategory("Debug Menu InputCodes")]
		[InputCode("Debug Help", "Displays the debug help menu")]
		[DefaultSettingValue("F1")]
		public InputCode DebugHelp
		{
			get => (InputCode)this[nameof(DebugHelp)];
			set => this[nameof(DebugHelp)] = value;
		}

		/// <summary>
		/// Debug Camera
		/// <br/> Displays the debug camera menu.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Debug Camera", "Displays the debug camera menu")]
		[DefaultSettingValue("F2")]
		public InputCode DebugCamera
		{
			get => (InputCode)this[nameof(DebugCamera)];
			set => this[nameof(DebugCamera)] = value;
		}

		/// <summary>
		/// Debug Render
		/// <br/> Displays the debug render menu.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Debug Render", "Displays the debug render menu")]
		[DefaultSettingValue("F3")]
		public InputCode DebugRender
		{
			get => (InputCode)this[nameof(DebugRender)];
			set => this[nameof(DebugRender)] = value;
		}


		/// <summary>
		/// Circle Render mode
		/// <br/> Circles between the various render modes.
		/// </summary>
		[UserScopedSetting]
		[InputCodeCategory("Debug Option InputCodes")]
		[InputCode("Circle Render mode", "Circles between the various render modes")]
		[DefaultSettingValue("F5")]
		public InputCode CircleRenderMode
		{
			get => (InputCode)this[nameof(CircleRenderMode)];
			set => this[nameof(CircleRenderMode)] = value;
		}

		/// <summary>
		/// Circle Wireframe Mode
		/// <br/> Circles between the various wireframe modes.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Circle Wireframe Mode", "Circles between the various wireframe modes")]
		[DefaultSettingValue("F6")]
		public InputCode CircleWireframe
		{
			get => (InputCode)this[nameof(CircleWireframe)];
			set => this[nameof(CircleWireframe)] = value;
		}

		/// <summary>
		/// Display Bounds
		/// <br/> Displays geometry bounds.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Display Bounds", "Displays geometry bounds")]
		[DefaultSettingValue("F9")]
		public InputCode DisplayBounds
		{
			get => (InputCode)this[nameof(DisplayBounds)];
			set => this[nameof(DisplayBounds)] = value;
		}

		/// <summary>
		/// Circle Backwards
		/// <br/> Hold this button when using a hotInputCode for circling options to circle in the other direction.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Circle Backwards", "Hold this button when using a hotInputCode for circling options to circle in the other direction")]
		[DefaultSettingValue("RightShift")]
		public InputCode CircleBackward
		{
			get => (InputCode)this[nameof(CircleBackward)];
			set => this[nameof(CircleBackward)] = value;
		}


		/// <summary>
		/// Applies the settings to a debug controller.
		/// </summary>
		/// <param name="controller">The controller to copy to.</param>
		public void ApplyToController(DebugController controller)
		{
			controller.SetControls(new Dictionary<DebugController.Action, InputCode>()
			{
				{ DebugController.Action.DebugHelp, DebugHelp },
				{ DebugController.Action.DebugCamera, DebugCamera },
				{ DebugController.Action.DebugRender, DebugRender },
				{ DebugController.Action.CircleRenderMode, CircleRenderMode },
				{ DebugController.Action.CircleWireframe, CircleWireframe },
				{ DebugController.Action.CircleBackward, CircleBackward },
			});
		}
	}
}
