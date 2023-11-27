using System;

namespace SA3D.Rendering.Shaders
{
	/// <summary>
	/// Shader related exceptions.
	/// </summary>
	public class ShaderException : Exception
	{
		private const string _integratedGraphicsMessage = "Shader compilation faulty on Integrated graphics! Please use your GPU! \n\n";

		/// <summary>
		/// Whether the error was thrown because of integrated graphics incompatibilities.
		/// </summary>
		public bool IntegratedGraphics { get; }

		/// <summary>
		/// Name of the shader that threw this error.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Type of the shader.
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// The action that caused the exception.
		/// </summary>
		public string Action { get; }

		/// <summary>
		/// Error log provided by the shader.
		/// </summary>
		public string Log { get; }

		private ShaderException(string name, string type, string action, string log, bool integratedGraphics, string message) : base(message)
		{
			Name = name;
			Type = type;
			Action = action;
			Log = log;
			IntegratedGraphics = integratedGraphics;
		}

		/// <summary>
		/// Creates an exception based on a shader error message.
		/// </summary>
		/// <param name="name">Name of the shader.</param>
		/// <param name="type">Type of the shader.</param>
		/// <param name="action">The action that caused the exception.</param>
		/// <param name="log">The log to create the exception from</param>
		/// <returns></returns>
		public static ShaderException CreateFromLog(string name, string type, string action, string log)
		{
			bool integratedGraphics = false;

			string message = $"{type} \"{name}\" failed to/at \"{action}\": \n" + log;

			if(log.Contains("ERROR___HEXADECIMAL_CONST_OVERFLOW"))
			{
				message = _integratedGraphicsMessage + message;
				integratedGraphics = true;
			}

			return new(name, type, action, log, integratedGraphics, message);
		}
	}
}
