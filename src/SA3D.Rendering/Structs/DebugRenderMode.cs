namespace SA3D.Rendering.Structs
{
	/// <summary>
	/// Debug render modes.
	/// </summary>
	public enum DebugRenderMode : byte
	{
		/// <summary>
		/// Default
		/// </summary>
		Default = 0x00,

		/// <summary>
		/// Smoothe lighting
		/// </summary>
		Smooth = 0x01,

		/// <summary>
		/// Falloff instead of lighting
		/// </summary>
		Falloff = 0x02,

		/// <summary>
		/// Renders normals
		/// </summary>
		Normals = 0x03,

		/// <summary>
		/// Renders vertex colors
		/// </summary>
		Colors = 0x04,

		/// <summary>
		/// Renders vertex weights
		/// </summary>
		Weights = 0x05,

		/// <summary>
		/// Renders uv coordinates
		/// </summary>
		Texcoords = 0x06,

		/// <summary>
		/// Renders textures only
		/// </summary>
		Textures = 0x07,

		/// <summary>
		/// Renders lighting only
		/// </summary>
		Lighting = 0x08,

		/// <summary>
		/// Displays the culling side
		/// </summary>
		CullSide = 0x09
	}
}
