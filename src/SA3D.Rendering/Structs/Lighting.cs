using SA3D.Modeling.Structs;
using System.Numerics;

namespace SA3D.Rendering.Structs
{
	/// <summary>
	/// Lighting information.
	/// </summary>
	public struct Lighting
	{
		/// <summary>
		/// Light direction.
		/// </summary>
		public Vector3 Direction { get; set; }

		/// <summary>
		/// Diffuse color intensity.
		/// </summary>
		public float ColorIntensity { get; set; }

		/// <summary>
		/// Diffuse color.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Ambient color intensity.
		/// </summary>
		public float AmbientIntensity { get; set; }

		/// <summary>
		/// Ambient color.
		/// </summary>
		public Color Ambient { get; set; }

		/// <summary>
		/// Creates new lighting information.
		/// </summary>
		/// <param name="direction">Light direction.</param>
		/// <param name="colorIntensity">Diffuse color intensity.</param>
		/// <param name="color">Diffuse color.</param>
		/// <param name="ambientIntensity">Ambient color intensity.</param>
		/// <param name="ambient">Ambient color.</param>
		public Lighting(Vector3 direction, float colorIntensity, Color color, float ambientIntensity, Color ambient)
		{
			Direction = direction;
			ColorIntensity = colorIntensity;
			Color = color;
			AmbientIntensity = ambientIntensity;
			Ambient = ambient;
		}
	}
}
