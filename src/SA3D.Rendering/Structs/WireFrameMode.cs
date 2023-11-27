namespace SA3D.Rendering.Structs
{
	/// <summary>
	/// Ways to display model wireframes
	/// </summary>
	public enum WireFrameMode
	{
		/// <summary>
		/// No wireframe shown
		/// </summary>
		None,

		/// <summary>
		/// Layers wireframe over the polygons
		/// </summary>
		Overlay,

		/// <summary>
		/// Replaces polygons with outlines
		/// </summary>
		ReplaceLine,

		/// <summary>
		/// Replaces polygons with points
		/// </summary>
		ReplacePoint
	}
}
