using System;

namespace SA3D.Rendering.Structs
{
	/// <summary>
	/// Rendering settings.
	/// </summary>
	public readonly struct RenderSettings : IEquatable<RenderSettings>
	{
		/// <summary>
		/// Disable lighting as a whole.
		/// </summary>
		public bool DisableLighting { get; init; }

		/// <summary>
		/// Disable specular lighting.
		/// </summary>
		public bool DisableSpecular { get; init; }

		/// <summary>
		/// When enabled, the material ambient color will be ignored.
		/// </summary>
		public bool DisableSurfaceAmbient { get; init; }

		/// <summary>
		/// Disables backface culling.
		/// </summary>
		public bool DisableBackfaceCulling { get; init; }

		/// <summary>
		/// Value subtracted from the alpha of the current frame buffer. Used for reflection or onion blur rendering.
		/// </summary>
		public float TransparencySubtract { get; init; }


		/// <summary>
		/// Creates new render settings.
		/// </summary>
		public RenderSettings()
		{
			DisableLighting = false;
			DisableSpecular = false;
			DisableSurfaceAmbient = false;
			DisableBackfaceCulling = false;
			TransparencySubtract = 0;
		}

		/// <summary>
		/// Creates new render settings from the given template.
		/// </summary>
		/// <param name="template">The template to use.</param>
		public RenderSettings(RenderSettings template)
		{
			DisableLighting = template.DisableLighting;
			DisableSpecular = template.DisableSpecular;
			DisableSurfaceAmbient = template.DisableSurfaceAmbient;
			DisableBackfaceCulling = template.DisableBackfaceCulling;
			TransparencySubtract = template.TransparencySubtract;
		}

		/// <summary>
		/// Creates new render settings.
		/// </summary>
		/// <param name="disableLighting">Disable lighting as a whole.</param>
		/// <param name="disableSpecular">Disable specular lighting.</param>
		/// <param name="disableSurfaceAmbient">When enabled, the material ambient color will be ignored.</param>
		/// <param name="disableCulling">Disables backface culling.</param>
		/// <param name="transparencySubtract">Value subtracted from the alpha of the current frame buffer. Used for reflection or onion blur rendering.</param>
		public RenderSettings(bool disableLighting, bool disableSpecular, bool disableSurfaceAmbient, bool disableCulling, float transparencySubtract)
		{
			DisableLighting = disableLighting;
			DisableSpecular = disableSpecular;
			DisableSurfaceAmbient = disableSurfaceAmbient;
			DisableBackfaceCulling = disableCulling;
			TransparencySubtract = transparencySubtract;
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is RenderSettings settings &&
				   DisableLighting == settings.DisableLighting &&
				   DisableSpecular == settings.DisableSpecular &&
				   DisableSurfaceAmbient == settings.DisableSurfaceAmbient &&
				   DisableBackfaceCulling == settings.DisableBackfaceCulling &&
				   TransparencySubtract == settings.TransparencySubtract;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(DisableLighting, DisableSpecular, DisableSurfaceAmbient, DisableBackfaceCulling, TransparencySubtract);
		}

		readonly bool IEquatable<RenderSettings>.Equals(RenderSettings other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two render settings for equality.
		/// </summary>
		/// <param name="left">Lefthand render settings.</param>
		/// <param name="right">Righthand render settings.</param>
		/// <returns>Whether the render settings are equal.</returns>
		public static bool operator ==(RenderSettings left, RenderSettings right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two render settings for inequality.
		/// </summary>
		/// <param name="left">Lefthand render settings.</param>
		/// <param name="right">Righthand render settings.</param>
		/// <returns>Whether the render settings are inequal.</returns>
		public static bool operator !=(RenderSettings left, RenderSettings right)
		{
			return !(left == right);
		}
	}
}
