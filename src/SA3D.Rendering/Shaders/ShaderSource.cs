using System;
using System.IO;
using System.Reflection;

namespace SA3D.Rendering.Shaders
{
	internal static class ShaderSource
	{
		internal static string GetResource(string filepath)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string resourceName = "SA3D.Rendering.Shaders." + filepath;

			using(Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new NullReferenceException())
			{
				return new StreamReader(stream).ReadToEnd();
			}
		}
	}
}
