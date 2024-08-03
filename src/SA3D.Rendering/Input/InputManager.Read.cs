using System.Collections.Generic;
using System.Numerics;

namespace SA3D.Rendering.Input
{
	public partial class InputManager
	{
		/* ==== Read section ====
         * Access to the inputs for the graphics API, regardless of which framework is used
         */

		private HashSet<InputCode> _isPressed = [];
		private HashSet<InputCode> _wasPressed = [];
		private bool _lockCursor;

		/// <summary>
		/// The last read cursor location
		/// </summary>
		public Vector2 CursorPosition { get; private set; }

		/// <summary>
		/// The amount that the cursor moved
		/// </summary>
		public Vector2 CursorDelta { get; private set; }

		/// <summary>
		/// The amount that the scroll was used 
		/// </summary>
		public float ScrollDelta { get; private set; }

		/// <summary>
		/// Whether the cursor is locked and cannot be moved.
		/// </summary>
		public bool LockCursor
		{
			get => _lockCursor;
			set
			{
				if(value == _lockCursor)
				{
					return;
				}

				_lockCursor = value;
				OnSetMouselock?.Invoke(value);
			}
		}

		/// <summary>
		/// Whether the context is currently focused.
		/// </summary>
		public bool IsFocused { get; private set; }

		/// <summary>
		/// Whether the context was focused on the last input cycle..
		/// </summary>
		public bool WasFocused { get; private set; }


		/// <summary>
		/// Places the cursor in global screen space
		/// </summary>
		/// <param name="loc">The new location that the cursor should be at</param>
		public void PlaceCursor(Vector2 loc)
		{
			OnSetCursorPosition?.Invoke(loc);
			_newCursorLocation = loc;
			CursorPosition = loc;
		}

		/// <summary>
		/// Whether a keyboard key is being held
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsDown(InputCode key)
		{
			return _isPressed.Contains(key);
		}

		/// <summary>
		/// Whether a keyboard key was pressed
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsPressed(InputCode key)
		{
			return _isPressed.Contains(key) && !_wasPressed.Contains(key);
		}

		/// <summary>
		/// Whether a keyboard key was released
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsReleased(InputCode key)
		{
			return !_isPressed.Contains(key) && _wasPressed.Contains(key);
		}
	}
}
