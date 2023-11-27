using System.Collections.Generic;
using System.Configuration;

namespace SA3D.Rendering.Input.Settings
{
	/// <summary>
	/// Camera input settings.
	/// </summary>
	public class CameraInputSettings : ApplicationSettingsBase
	{
		/// <summary>
		/// Orbiting InputCode
		/// <br/> Mouse button used for navigating in orbit mode.
		/// </summary>
		[UserScopedSetting]
		[InputCodeCategory("Orbiting Controls")]
		[InputCode("Orbiting InputCode", "Mouse button used for navigating in orbit mode")]
		[DefaultSettingValue("MouseMiddle")]
		public InputCode Orbit
		{
			get => (InputCode)this[nameof(Orbit)];
			set => this[nameof(Orbit)] = value;
		}

		/// <summary>
		/// Perspective
		/// <br/> Switches between Perspective and Orthographic.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Perspective", "Switches between Perspective and Orthographic")]
		[DefaultSettingValue("NumPad5")]
		public InputCode Perspective
		{
			get => (InputCode)this[nameof(Perspective)];
			set => this[nameof(Perspective)] = value;
		}

		/// <summary>
		/// Navigation Mode
		/// <br/> Switches between Orbiting and FPS movement.
		/// </summary>
		[UserScopedSetting]
		[InputCodeCategory("Switching the camera")]
		[InputCode("Navigation Mode", "Switches between Orbiting and FPS movement")]
		[DefaultSettingValue("O")]
		public InputCode NavMode
		{
			get => (InputCode)this[nameof(NavMode)];
			set => this[nameof(NavMode)] = value;
		}

		/// <summary>
		/// Zoom Modifier
		/// <br/> Modifier used to zoom camera when pressing the orbit InputCode.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Zoom Modifier", "Modifier used to zoom camera when pressing the orbit InputCode")]
		[DefaultSettingValue("LeftCtrl")]
		public InputCode ZoomModifier
		{
			get => (InputCode)this[nameof(ZoomModifier)];
			set => this[nameof(ZoomModifier)] = value;
		}

		/// <summary>
		/// Drag Modifier
		/// <br/> Modifier used to move camera when pressing the orbit InputCode.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Drag Modifier", "Modifier used to move camera when pressing the orbit InputCode")]
		[DefaultSettingValue("LeftShift")]
		public InputCode DragModifier
		{
			get => (InputCode)this[nameof(DragModifier)];
			set => this[nameof(DragModifier)] = value;
		}

		/// <summary>
		/// Align Forward
		/// <br/> Aligns camera with the -Z axis.
		/// </summary>
		[UserScopedSetting]
		[InputCodeCategory("Camera Snapping")]
		[InputCode("Align Forward", "Aligns camera with the -Z axis")]
		[DefaultSettingValue("NumPad1")]
		public InputCode AlignForward
		{
			get => (InputCode)this[nameof(AlignForward)];
			set => this[nameof(AlignForward)] = value;
		}

		/// <summary>
		/// Align Side
		/// <br/> Aligns camera with the -X axis.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Align Side", "Aligns camera with the -X axis")]
		[DefaultSettingValue("NumPad3")]
		public InputCode AlignSide
		{
			get => (InputCode)this[nameof(AlignSide)];
			set => this[nameof(AlignSide)] = value;
		}

		/// <summary>
		/// Align Up
		/// <br/> Aligns camera with the -Y axis.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Align Up", "Aligns camera with the -Y axis")]
		[DefaultSettingValue("NumPad7")]
		public InputCode AlignUp
		{
			get => (InputCode)this[nameof(AlignUp)];
			set => this[nameof(AlignUp)] = value;
		}

		/// <summary>
		/// Align Invert
		/// <br/> Inverts the axis of the selected axis to align with.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Align Invert", "Inverts the axis of the selected axis to align with")]
		[DefaultSettingValue("LeftCtrl")]
		public InputCode AlignInvert
		{
			get => (InputCode)this[nameof(AlignInvert)];
			set => this[nameof(AlignInvert)] = value;
		}

		/// <summary>
		/// Resets Camera
		/// <br/> Resets camera properties to default values.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Resets Camera", "Resets camera properties to default values")]
		[DefaultSettingValue("R")]
		public InputCode ResetCamera
		{
			get => (InputCode)this[nameof(ResetCamera)];
			set => this[nameof(ResetCamera)] = value;
		}

		/// <summary>
		/// Forward
		/// <br/> InputCode used to move forward in first person.
		/// </summary>
		[UserScopedSetting]
		[InputCodeCategory("First Person Controls")]
		[InputCode("Forward", "InputCode used to move forward in first person")]
		[DefaultSettingValue("W")]
		public InputCode FirstPersonForward
		{
			get => (InputCode)this[nameof(FirstPersonForward)];
			set => this[nameof(FirstPersonForward)] = value;
		}

		/// <summary>
		/// Backward
		/// <br/> InputCode used to move backward in first person.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Backward", "InputCode used to move backward in first person")]
		[DefaultSettingValue("S")]
		public InputCode FirstPersonBackward
		{
			get => (InputCode)this[nameof(FirstPersonBackward)];
			set => this[nameof(FirstPersonBackward)] = value;
		}

		/// <summary>
		/// Right
		/// <br/> InputCode used to move right in first person.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Right", "InputCode used to move right in first person")]
		[DefaultSettingValue("D")]
		public InputCode FirstPersonRight
		{
			get => (InputCode)this[nameof(FirstPersonRight)];
			set => this[nameof(FirstPersonRight)] = value;
		}

		/// <summary>
		/// Left
		/// <br/> InputCode used to move left in first person.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Left", "InputCode used to move left in first person")]
		[DefaultSettingValue("A")]
		public InputCode FirstPersonLeft
		{
			get => (InputCode)this[nameof(FirstPersonLeft)];
			set => this[nameof(FirstPersonLeft)] = value;
		}

		/// <summary>
		/// Up
		/// <br/> InputCode used to move up in first person.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Up", "InputCode used to move up in first person")]
		[DefaultSettingValue("Space")]
		public InputCode FirstPersonUp
		{
			get => (InputCode)this[nameof(FirstPersonUp)];
			set => this[nameof(FirstPersonUp)] = value;
		}

		/// <summary>
		/// Down
		/// <br/> InputCode used to move down in first person.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Down", "InputCode used to move down in first person")]
		[DefaultSettingValue("LeftCtrl")]
		public InputCode FirstPersonDown
		{
			get => (InputCode)this[nameof(FirstPersonDown)];
			set => this[nameof(FirstPersonDown)] = value;
		}

		/// <summary>
		/// Speed up
		/// <br/> Movement modifier used to speed up a bit.
		/// </summary>
		[UserScopedSetting]
		[InputCode("Speed up", "Movement modifier used to speed up a bit")]
		[DefaultSettingValue("LeftShift")]
		public InputCode FirstPersonSpeedup
		{
			get => (InputCode)this[nameof(FirstPersonSpeedup)];
			set => this[nameof(FirstPersonSpeedup)] = value;
		}

		/// <summary>
		/// Applies the settings to a camera controller.
		/// </summary>
		/// <param name="controller">The controller to copy to.</param>
		public void ApplyToController(CameraController controller)
		{
			controller.SetControls(new Dictionary<CameraController.Action, InputCode>()
			{
				{ CameraController.Action.Orbit, Orbit },
				{ CameraController.Action.Perspective, Perspective },
				{ CameraController.Action.NavMode, NavMode },
				{ CameraController.Action.ZoomModifier, ZoomModifier },
				{ CameraController.Action.DragModifier, DragModifier },

				{ CameraController.Action.AlignForward, AlignForward },
				{ CameraController.Action.AlignSide, AlignSide },
				{ CameraController.Action.AlignUp, AlignUp },
				{ CameraController.Action.AlignInvert, AlignInvert },
				{ CameraController.Action.ResetCamera, ResetCamera },

				{ CameraController.Action.FirstPersonForward, FirstPersonForward },
				{ CameraController.Action.FirstPersonBackward, FirstPersonBackward },
				{ CameraController.Action.FirstPersonRight, FirstPersonRight },
				{ CameraController.Action.FirstPersonLeft, FirstPersonLeft },
				{ CameraController.Action.FirstPersonUp, FirstPersonUp },
				{ CameraController.Action.FirstPersonDown, FirstPersonDown },
				{ CameraController.Action.FirstPersonSpeedup, FirstPersonSpeedup },
			});
		}
	}
}
