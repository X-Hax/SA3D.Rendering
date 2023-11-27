using System;
using System.Numerics;
using static SA3D.Common.MathHelper;

namespace SA3D.Rendering.Input
{
	/// <summary>
	/// Controls camera with keyboard and mouse inputs.
	/// </summary>
	public class CameraController : BaseController<CameraController.Action>
	{
		/// <summary>
		/// Camera control input actions.
		/// </summary>
		public enum Action
		{
			/// <summary>
			/// Mouse button used for navigating in orbit mode.
			/// </summary>
			Orbit,

			/// <summary>
			/// Switches between Perspective and Orthographic.
			/// </summary>
			Perspective,

			/// <summary>
			/// Switches between Orbiting and FPS movement.
			/// </summary>
			NavMode,

			/// <summary>
			/// Modifier used to zoom camera when pressing the orbit InputCode.
			/// </summary>
			ZoomModifier,

			/// <summary>
			/// Modifier used to move camera when pressing the orbit InputCode.
			/// </summary>
			DragModifier,

			/// <summary>
			/// Aligns camera with the -Z axis.
			/// </summary>
			AlignForward,

			/// <summary>
			/// Aligns camera with the -X axis.
			/// </summary>
			AlignSide,

			/// <summary>
			/// Aligns camera with the -Y axis.
			/// </summary>
			AlignUp,

			/// <summary>
			/// Inverts the axis of the selected axis to align with.
			/// </summary>
			AlignInvert,

			/// <summary>
			/// Resets camera properties to default values.
			/// </summary>
			ResetCamera,

			/// <summary>
			/// Moves the camera forward in first person.
			/// </summary>
			FirstPersonForward,

			/// <summary>
			/// Moves the camera backward in first person.
			/// </summary>
			FirstPersonBackward,

			/// <summary>
			/// Moves the camera right in first person.
			/// </summary>
			FirstPersonRight,

			/// <summary>
			/// Moves the camera left in first person.
			/// </summary>
			FirstPersonLeft,

			/// <summary>
			/// Moves the camera up in first person.
			/// </summary>
			FirstPersonUp,

			/// <summary>
			/// Moves the camera down in first person.
			/// </summary>
			FirstPersonDown,

			/// <summary>
			/// Movement modifier used to speed up a bit.
			/// </summary>
			FirstPersonSpeedup
		}

		/// <summary>
		/// Camera to control.
		/// </summary>
		public Camera Camera { get; }

		/// <summary>
		/// Camera Orbit-drag speed for the mouse.
		/// </summary>
		public float CamDragSpeed { get; set; } = 0.001f;

		/// <summary>
		/// Camera first-person movement speed.
		/// </summary>
		public float CamMovementSpeed { get; set; } = 30f;

		/// <summary>
		/// Basically "sprint" speed multiplier for <see cref="CamMovementSpeed"/>.
		/// </summary>
		public float CamMovementModif { get; set; } = 2f;

		/// <summary>
		/// Camera first-person mouse sensitivity.
		/// </summary>
		public float CamMouseSensitivity { get; set; } = 0.002f;

		/// <summary>
		/// Camera orbiting mouse sensitivity.
		/// </summary>
		public float CamOrbitSensitivity { get; set; } = 0.008f;

		/// <summary>
		/// Creates a new camera controller.
		/// </summary>
		/// <param name="input">The inputs to read.</param>
		/// <param name="camera">The camera to control.</param>
		public CameraController(InputManager input, Camera camera) : base(
			input,
			new()
			{
				{ Action.Orbit, InputCode.MouseMiddle },
				{ Action.Perspective, InputCode.NumPad5 },
				{ Action.NavMode, InputCode.O },
				{ Action.ZoomModifier, InputCode.LeftCtrl },
				{ Action.DragModifier, InputCode.LeftShift },

				{ Action.AlignForward, InputCode.NumPad1 },
				{ Action.AlignSide, InputCode.NumPad3 },
				{ Action.AlignUp, InputCode.NumPad7 },
				{ Action.AlignInvert, InputCode.LeftCtrl },
				{ Action.ResetCamera, InputCode.R },

				{ Action.FirstPersonForward, InputCode.W },
				{ Action.FirstPersonBackward, InputCode.S },
				{ Action.FirstPersonRight, InputCode.D },
				{ Action.FirstPersonLeft, InputCode.A },
				{ Action.FirstPersonUp, InputCode.Space },
				{ Action.FirstPersonDown, InputCode.LeftCtrl },
				{ Action.FirstPersonSpeedup, InputCode.LeftShift },
			})
		{
			Camera = camera;
		}

		/// <summary>
		/// Runs the controller.
		/// </summary>
		/// <param name="delta">Seconds since the last update cycle/frame.</param>
		public void Run(double delta)
		{
			if(!Camera.Orbiting) // if in first-person mode
			{
				if(!Input.IsFocused || Input.IsPressed(InputCode.Escape) || IsPressed(Action.NavMode))
				{
					Camera.Orbiting = true;
					Input.LockCursor = false;
				}
			}
			else if(IsPressed(Action.NavMode))
			{
				Camera.Orbiting = false;
				Input.LockCursor = true;
			}

			if(IsPressed(Action.ResetCamera))
			{
				Camera.Orthographic = true;
				Camera.Orbiting = true;
				Camera.Position = default;
				Camera.Rotation = default;
				Camera.Distance = 50;
				CamMovementSpeed = 30;
				return;
			}

			if(!Camera.Orbiting)
			{
				// rotation
				Camera.Rotation = new Vector3(
					Math.Max(-HalfPi, Math.Min(HalfPi, Camera.Rotation.X - (Input.CursorDelta.Y * CamMouseSensitivity))),
					(Camera.Rotation.Y - (Input.CursorDelta.X * CamMouseSensitivity)) % float.Tau,
					0);

				// modifying movement speed 
				float dir = Input.ScrollDelta < 0 ? -0.05f : 0.05f;
				for(int i = (int)Math.Abs(Input.ScrollDelta); i > 0; i--)
				{
					CamMovementSpeed += CamMovementSpeed * dir;
					CamMovementSpeed = Math.Max(0.0001f, Math.Min(1000, CamMovementSpeed));
				}

				// movement
				Vector3 dif = default;

				if(IsDown(Action.FirstPersonForward))
				{
					dif += Camera.Forward;
				}

				if(IsDown(Action.FirstPersonBackward))
				{
					dif -= Camera.Forward;
				}

				if(IsDown(Action.FirstPersonLeft))
				{
					dif += Camera.Right;
				}

				if(IsDown(Action.FirstPersonRight))
				{
					dif -= Camera.Right;
				}

				if(IsDown(Action.FirstPersonUp))
				{
					dif += Camera.Up;
				}

				if(IsDown(Action.FirstPersonDown))
				{
					dif -= Camera.Up;
				}

				if(dif.Length() == 0)
				{
					return;
				}

				Camera.Position += Vector3.Normalize(dif) * CamMovementSpeed * (IsDown(Action.FirstPersonSpeedup) ? CamMovementModif : 1) * (float)delta;
			}
			else
			{
				// mouse orientation
				if(IsDown(Action.Orbit))
				{
					if(IsDown(Action.ZoomModifier)) // zooming
					{
						Camera.Distance += Camera.Distance * Input.CursorDelta.Y * 0.01f;
					}
					else if(IsDown(Action.DragModifier)) // moving
					{
						Vector3 dif = default;
						float speed = CamDragSpeed * Camera.Distance;
						dif += Camera.Right * Input.CursorDelta.X * speed;
						dif += Camera.Up * Input.CursorDelta.Y * speed;
						Camera.Position += dif;
					}
					else // rotation
					{
						Camera.Rotation = new Vector3(
							Math.Max(-HalfPi, Math.Min(HalfPi, Camera.Rotation.X - (Input.CursorDelta.Y * CamOrbitSensitivity))),
							(Camera.Rotation.Y - (Input.CursorDelta.X * CamOrbitSensitivity)) % float.Tau,
							0);
					}
				}
				else
				{
					if(IsPressed(Action.Perspective))
					{
						Camera.Orthographic = !Camera.Orthographic;
					}

					bool invertAxis = IsDown(Action.AlignInvert);
					if(IsPressed(Action.AlignForward))
					{
						Camera.Rotation = new Vector3(0, invertAxis ? float.Pi : 0, 0);
					}
					else if(IsPressed(Action.AlignSide))
					{
						Camera.Rotation = new Vector3(0, invertAxis ? -HalfPi : HalfPi, 0);
					}
					else if(IsPressed(Action.AlignUp))
					{
						Camera.Rotation = new Vector3(invertAxis ? -HalfPi : HalfPi, 0, 0);
					}

					float dir = Input.ScrollDelta < 0 ? 0.07f : -0.07f;
					for(int i = (int)Math.Abs(Input.ScrollDelta); i > 0; i--)
					{
						Camera.Distance += Math.Max(Camera.Distance, 1f) * dir;
					}
				}
			}
		}

	}
}
