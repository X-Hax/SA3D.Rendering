using System.Collections.Generic;
using System.Numerics;

namespace SA3D.Rendering.Input
{
	public partial class InputManager
	{
		/* ==== Write section ====
         * Whatever framework is used transfers their inputs via these method calls.
         */

		private readonly HashSet<InputCode> _newPressed = [];
		private readonly HashSet<InputCode> _newReleased = [];
		private bool _hadValidCursorLocationBefore;
		private Vector2 _newCursorLocation;
		private Vector2 _newCursorDelta;
		private float _newScrollDelta;

		/// <summary>
		/// Event delegate for cursor position changing.
		/// </summary>
		/// <param name="position"></param>
		public delegate void SetCursorPosition(Vector2 position);

		/// <summary>
		/// Event delegate for mouse lock changing.
		/// </summary>
		/// <param name="locked"></param>
		public delegate void SetMouseLock(bool locked);

		/// <summary>
		/// Gets called when the API places the cursor (local position)
		/// </summary>
		public event SetCursorPosition? OnSetCursorPosition;

		/// <summary>
		/// Gets called when the API changes the cursor lock
		/// </summary>
		public event SetMouseLock? OnSetMouselock;


		/// <summary>
		/// Called when a key was pressed/released.
		/// </summary>
		public void SetInput(InputCode key, bool pressed)
		{
			(pressed ? _newPressed : _newReleased).Add(key);
		}

		/// <summary>
		/// Clears inputs.
		/// </summary>
		public void ClearInputs()
		{
			_newPressed.Clear();
			_newReleased.Clear();
			SetCursorPos(null, null);
		}

		/// <summary>
		/// Updates the cursor position.
		/// </summary>
		/// <param name="pos">The new position; null if its outside the window.</param>
		/// <param name="recenter">Recentering position; null if mouse isnt locked to center.</param>
		public void SetCursorPos(Vector2? pos, Vector2? recenter)
		{
			if(!pos.HasValue)
			{
				_hadValidCursorLocationBefore = false;
				_newCursorDelta = default;
				return;
			}

			if(recenter.HasValue)
			{
				if(recenter != pos.Value)
				{
					_newCursorDelta = pos.Value - recenter.Value;
				}

				_newCursorLocation = recenter.Value;
				return;
			}
			else if(_hadValidCursorLocationBefore)
			{
				_newCursorDelta += pos.Value - _newCursorLocation;
			}
			else
			{
				_newCursorDelta = default;
			}

			_hadValidCursorLocationBefore = true;
			_newCursorLocation = pos.Value;
		}

		/// <summary>
		/// Updates the scroll delta.
		/// </summary>
		/// <param name="delta">The delta to add.</param>
		public void SetScroll(float delta)
		{
			_newScrollDelta += delta;
		}
	}
}
