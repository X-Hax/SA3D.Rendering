using System;

namespace SA3D.Rendering.Input.Settings
{
	/// <summary>
	/// Attribute determining the category of inputs to which an input belongs.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class InputCodeCategoryAttribute : Attribute
	{
		/// <summary>
		/// Title of the cateogory.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Creates a new category attribute.
		/// </summary>
		/// <param name="title">Title of the cateogory.</param>
		public InputCodeCategoryAttribute(string title)
		{
			Title = title;
		}
	}

	/// <summary>
	/// Attribute determining name and description of an input.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class InputCodeAttribute : Attribute
	{
		/// <summary>
		/// Name of the input.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Description of the input.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Creates a new input code attribute.
		/// </summary>
		/// <param name="name">Name of the input.</param>
		/// <param name="description">Description of the input.</param>
		public InputCodeAttribute(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}
