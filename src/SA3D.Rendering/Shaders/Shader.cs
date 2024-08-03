using OpenTK.Graphics.OpenGL4;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SA3D.Rendering.Shaders
{
	/// <summary>
	/// A GLSL Shader
	/// </summary>
	public class Shader
	{
		/// <summary>
		/// Used to store the different uniforms in the shader
		/// </summary>
		private readonly struct UniformType
		{
			/// <summary>
			/// The uniform location in the shader
			/// </summary>
			public readonly int location;

			/// <summary>
			/// The uniform type
			/// </summary>
			public readonly ActiveUniformType type;

			/// <summary>
			/// The texture unit (if it is a texture)
			/// </summary>
			public readonly TextureUnit? unit;

			public UniformType(int location, ActiveUniformType type)
			{
				this.location = location;
				this.type = type;
				unit = null;
			}

			public UniformType(int location, TextureUnit unit)
			{
				this.location = location;
				type = ActiveUniformType.Sampler2D;
				this.unit = unit;
			}
		}

		/// <summary>
		/// Used to trim byte order mark
		/// </summary>
		private readonly char[] _bomTrimmer = new char[] { '\uFEFF', '\u200B' };

		/// <summary>
		/// The shader program handle
		/// </summary>
		private int _handle;

		/// <summary>
		/// The shaders
		/// </summary>
		private readonly Dictionary<string, UniformType> _uniformLocations;

		private readonly string _vertexShaderSource;
		private readonly string _fragmentShaderSource;

		/// <summary>
		/// Name of the shader,
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Creates a new Shader from a vertex and fragment shader
		/// </summary>
		/// <param name="name">Name of the shader.</param>
		/// <param name="vertexShaderSource">The vertex shader source</param>
		/// <param name="fragmentShaderSource">The fragment shader source</param>
		public Shader(string name, string vertexShaderSource, string fragmentShaderSource)
		{
			Name = name;
			_vertexShaderSource = CorrectString(vertexShaderSource);
			_fragmentShaderSource = CorrectString(fragmentShaderSource);
			_uniformLocations = [];
			_handle = -1;
		}

		/// <summary>
		/// Compiles the shader code.
		/// </summary>
		/// <exception cref="ShaderException"></exception>
		public void Compile()
		{
			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, _vertexShaderSource);
			;
			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, _fragmentShaderSource);

			GL.CompileShader(vertexShader);

			string infoLogVert = GL.GetShaderInfoLog(vertexShader);
			if(!string.IsNullOrWhiteSpace(infoLogVert))
			{
				throw ShaderException.CreateFromLog(Name, "Vertex shader", "compile", infoLogVert);
			}

			GL.CompileShader(fragmentShader);

			string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
			if(!string.IsNullOrWhiteSpace(infoLogFrag))
			{
				throw ShaderException.CreateFromLog(Name, "Fragment shader", "compile", infoLogFrag);
			}

			//linking the shaders

			_handle = GL.CreateProgram();

			GL.AttachShader(_handle, vertexShader);
			GL.AttachShader(_handle, fragmentShader);

			GL.LinkProgram(_handle);

			//cleanup

			GL.DetachShader(_handle, vertexShader);
			GL.DetachShader(_handle, fragmentShader);
			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);

			// getting the uniforms

			GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);

			int samplerInt = 0;

			for(int i = 0; i < numberOfUniforms; i++)
			{
				string key = GL.GetActiveUniform(_handle, i, out _, out ActiveUniformType t);

				int location = GL.GetUniformLocation(_handle, key);
				if(location < 0)
				{
					continue;
				}

				if(t == ActiveUniformType.Sampler2D)
				{
					_uniformLocations.Add(key, new UniformType(location, TextureUnit.Texture0 + samplerInt));
					SetUniform(key, samplerInt);
					samplerInt++;
				}
				else
				{
					_uniformLocations.Add(key, new UniformType(location, t));
				}
			}
		}

		private string CorrectString(string input)
		{
			return input.Trim(_bomTrimmer) + "\n\0";
		}


		/// <summary>
		/// Binds a uniform buffer to a uniform block of a shader
		/// </summary>
		/// <param name="blockname">The name of the block</param>
		/// <param name="blockid">The block ID</param>
		/// <param name="ubo">The uniform buffer object handle</param>
		public void BindUniformBlock(string blockname, int blockid, int ubo)
		{
			int index = GL.GetUniformBlockIndex(_handle, blockname);
			if(index < 0)
			{
				throw new ArgumentException("Block name does not exist: " + blockname);
			}

			GL.UniformBlockBinding(_handle, index, blockid);
			GL.BindBufferBase(BufferRangeTarget.UniformBuffer, blockid, ubo);
		}

		// Uniform setters
		// Uniforms are variables that can be set by user code, instead of reading them from the VBO.
		// You use VBOs for vertex-related data, and uniforms for almost everything else.

		// Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
		//     1. Bind the program you want to set the uniform on
		//     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
		//     3. Use the appropriate GL.Uniform* function to set the uniform.

		/// <summary>
		/// Set a uniform int on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetUniform(string name, int data)
		{
			Use();
			GL.Uniform1(_uniformLocations[name].location, data);
		}

		/// <summary>
		/// Set a uniform float on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetUniform(string name, float data)
		{
			Use();
			GL.Uniform1(_uniformLocations[name].location, data);
		}

		/// <summary>
		/// Set a uniform double on this shader
		/// </summary>
		/// <param name="name">Name of the uniform</param>
		/// <param name="data">the data</param>
		public void SetUniform(string name, double data)
		{
			Use();
			GL.Uniform1(_uniformLocations[name].location, data);
		}

		/// <summary>
		/// Set a uniform boolean on this shader
		/// </summary>
		/// <param name="name">Name of the uniform</param>
		/// <param name="data">the data</param>
		public void SetUniform(string name, bool data)
		{
			Use();
			GL.Uniform1(_uniformLocations[name].location, data ? 1 : 0);
		}

		/// <summary>
		/// Set a uniform Matrix4 on this shader
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public unsafe void SetUniform(string name, Matrix4x4 data)
		{
			Use();
			GL.UniformMatrix4(_uniformLocations[name].location, 1, false, &data.M11);
		}

		/// <summary>
		/// Set a uniform Vector2 on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetUniform(string name, Vector2 data)
		{
			Use();
			GL.Uniform2(_uniformLocations[name].location, data.X, data.Y);
		}

		/// <summary>
		/// Set a uniform Vector3 on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetUniform(string name, Vector3 data)
		{
			if(!_uniformLocations.ContainsKey(name))
			{
				return;
			}

			Use();
			GL.Uniform3(_uniformLocations[name].location, data.X, data.Y, data.Z);
		}

		/// <summary>
		/// Set a uniform Vector4 on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetUniform(string name, Vector4 data)
		{
			Use();
			GL.Uniform4(_uniformLocations[name].location, data.X, data.Y, data.Z, data.W);
		}

		/// <summary>
		/// Set a uniform color on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetUniform(string name, Color data)
		{
			Use();
			GL.Uniform4(_uniformLocations[name].location, data.SystemColor);
		}

		/// <summary>
		/// Makes OpenGL use the shader.
		/// </summary>
		public void Use()
		{
			GL.UseProgram(_handle);
		}
	}
}
