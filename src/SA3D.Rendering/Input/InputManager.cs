namespace SA3D.Rendering.Input
{
	/// <summary>
	/// Represents an input interface between the code using the render context and the application consuming it.
	/// </summary>
	public partial class InputManager
	{
		/// <summary>
		/// Creates a new input manager.
		/// </summary>
		public InputManager()
		{

		}

		/// <summary>
		/// Called before the input update; 
		/// <br/> Releases keys
		/// </summary>
		private void PreUpdate()
		{
			if(_newReleased.Count > 0)
			{
				foreach(InputCode k in _newReleased)
				{
					if(_isPressed.Contains(k))
					{
						_newPressed.Remove(k);
					}
				}

				_newReleased.RemoveWhere(x => !_newPressed.Contains(x));
			}
		}

		/// <summary>
		/// Called after the input update; 
		/// <br/> Clears deltas.
		/// </summary>
		private void PostUpdate()
		{
			_newScrollDelta = 0;
			_newCursorDelta = default;
		}

		/// <summary>
		/// Updates the input
		/// </summary>
		public void Update(bool focused)
		{
			PreUpdate();

			(_isPressed, _wasPressed) = (_wasPressed, _isPressed);
			_isPressed.Clear();

			if(focused)
			{
				_isPressed.UnionWith(_newPressed);
			}

			CursorDelta = _newCursorDelta;
			ScrollDelta = _newScrollDelta;
			CursorPosition = _newCursorLocation;

			PostUpdate();

			WasFocused = !focused && IsFocused;
			IsFocused = focused;
		}
	}
}
