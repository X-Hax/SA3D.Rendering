using OpenTK.Graphics.OpenGL4;
using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System.IO;
using System.Numerics;

namespace SA3D.Rendering.Buffer.Internal
{
	internal class UniformBuffer
	{

		private readonly byte[] _buffer;
		private int _handle;
		private readonly MemoryStream _bufferStream;
		public EndianStackWriter BufferWriter { get; }

		public UniformBuffer(int size)
		{
			byte[] buffer = new byte[size];
			_buffer = buffer;
			_bufferStream = new MemoryStream(buffer);
			BufferWriter = new(_bufferStream);
			_handle = -1;
		}

		public void Generate()
		{
			if(_handle != -1)
			{
				DeleteBuffer();
			}

			_handle = GL.GenBuffer();
			BufferData();
		}

		public unsafe void BufferData()
		{
			GL.BindBuffer(BufferTarget.UniformBuffer, _handle);
			fixed(byte* ptr = _buffer)
			{
				GL.BufferData(BufferTarget.UniformBuffer, _buffer.Length, (nint)ptr, BufferUsageHint.StreamDraw);
			}

			GL.BindBuffer(BufferTarget.UniformBuffer, 0);
		}

		public void DeleteBuffer()
		{
			GL.DeleteBuffer(_handle);
			_handle = -1;
		}

		public void ResetPosition()
		{
			BufferWriter.Stream.Seek(0, SeekOrigin.Begin);
		}

		public void WriteColor(Color c)
		{
			BufferWriter.WriteFloat(c.RedF);
			BufferWriter.WriteFloat(c.GreenF);
			BufferWriter.WriteFloat(c.BlueF);
			BufferWriter.WriteFloat(c.AlphaF);
		}

		public void WriteMatrix(Matrix4x4 matrix)
		{
			BufferWriter.WriteFloat(matrix.M11);
			BufferWriter.WriteFloat(matrix.M12);
			BufferWriter.WriteFloat(matrix.M13);
			BufferWriter.WriteFloat(matrix.M14);

			BufferWriter.WriteFloat(matrix.M21);
			BufferWriter.WriteFloat(matrix.M22);
			BufferWriter.WriteFloat(matrix.M23);
			BufferWriter.WriteFloat(matrix.M24);

			BufferWriter.WriteFloat(matrix.M31);
			BufferWriter.WriteFloat(matrix.M32);
			BufferWriter.WriteFloat(matrix.M33);
			BufferWriter.WriteFloat(matrix.M34);

			BufferWriter.WriteFloat(matrix.M41);
			BufferWriter.WriteFloat(matrix.M42);
			BufferWriter.WriteFloat(matrix.M43);
			BufferWriter.WriteFloat(matrix.M44);
		}

		public void Bind(int index)
		{
			GL.BindBufferBase(BufferRangeTarget.UniformBuffer, index, _handle);
		}
	}
}
