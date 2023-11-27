using System;
using System.Collections.Generic;

namespace SA3D.Rendering.Input
{
	/// <summary>
	/// Base controller class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseController<T> where T : struct, Enum
	{
		private readonly Dictionary<T, InputCode> _inputMapping;

		/// <summary>
		/// Inputs being read.
		/// </summary>
		protected InputManager Input { get; }

		/// <summary>
		/// Creates a new base controller with a default input mapping.
		/// </summary>
		/// <param name="input">The input to read friom.</param>
		/// <param name="defaultMapping">The default input action mapping.</param>
		protected BaseController(InputManager input, Dictionary<T, InputCode> defaultMapping)
		{
			Input = input;
			_inputMapping = defaultMapping;
		}

		/// <summary>
		/// Sets the control of a single action.
		/// </summary>
		/// <param name="action">The action to set the control of.</param>
		/// <param name="input">The input code to read for the action.</param>
		public void SetControl(T action, InputCode input)
		{
			_inputMapping[action] = input;
		}

		/// <summary>
		/// Sets the controls per action.
		/// </summary>
		/// <param name="mapping">The mapping to set.</param>
		public void SetControls(IDictionary<T, InputCode> mapping)
		{
			foreach(KeyValuePair<T, InputCode> item in mapping)
			{
				_inputMapping[item.Key] = item.Value;
			}
		}


		/// <summary>
		/// Checks whether a specific action has been pressed during the current update cycle.
		/// </summary>
		/// <param name="action">The action to check.</param>
		/// <returns></returns>
		protected bool IsPressed(T action)
		{
			return Input.IsPressed(_inputMapping[action]);
		}

		/// <summary>
		/// Checks whether a specific action is being held down.
		/// </summary>
		/// <param name="action">The action to check.</param>
		/// <returns></returns>
		protected bool IsDown(T action)
		{
			return Input.IsDown(_inputMapping[action]);
		}

		/// <summary>
		/// Checks whether a specific action has been released during the current update cycle.
		/// </summary>
		/// <param name="action">The action to check.</param>
		/// <returns></returns>
		protected bool IsReleased(T action)
		{
			return Input.IsReleased(_inputMapping[action]);
		}
	}
}
