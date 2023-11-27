using OpenTK.Graphics.OpenGL4;
using SA3D.Rendering.Shaders;
using SA3D.Rendering.UI;
using SA3D.Texturing;
using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Color = SA3D.Modeling.Structs.Color;

namespace SA3D.Rendering.Buffer
{
	/// <summary>
	/// Order independent transparency handler and buffer storage.
	/// </summary>
	public class OITBuffer
	{
		private const int _maxSamples = 16;

		private readonly TextureFrameBuffer _fbo;

		private int _fragmentCountTex;
		private int _fragmentHeadTex;
		private int _nodeBuffer;
		private int _nodeBufferSize;
		private int _nodeCounter;

		private Size _viewport;
		private float _nodeSpace;

		/// <summary>
		/// A single fragment node in the buffer.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct Node
		{
			/// <summary>
			/// Default value for <see cref="Next"/>, indicating that there is no next element.
			/// </summary>
			public const uint FragmentListNull = uint.MaxValue;

			/// <summary>
			/// Depth at which the fragment was drawn.
			/// </summary>
			public float Depth { get; set; }

			/// <summary>
			/// Color of the fragment.
			/// </summary>
			public Color Color { get; set; }

			/// <summary>
			/// Fragment flags storing blend mode info and more.
			/// </summary>
			public uint Flags { get; set; }

			/// <summary>
			/// Index to the next node.
			/// </summary>
			public uint Next { get; set; }

			/// <inheritdoc/>
			public override readonly string ToString()
			{
				return Next == FragmentListNull ? "[/]" : $"[{Next:X}]";
			}
		}

		/// <summary>
		/// Create a new OIT buffer.
		/// </summary>
		public OITBuffer()
		{
			_fbo = new(true);
		}

		private int CreateTexture(SizedInternalFormat format)
		{
			int result = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, result);
			GL.TextureStorage2D(result, 1, format, _viewport.Width, _viewport.Height);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			GL.BindTexture(TextureTarget.Texture2D, 0);
			return result;
		}


		private void SetupViewportRelatedBuffers()
		{
			if(_viewport.Width <= 0 || _viewport.Height <= 0)
			{
				return;
			}

			if(_fragmentCountTex != 0)
			{
				GL.DeleteTexture(_fragmentCountTex);
				GL.DeleteTexture(_fragmentHeadTex);
			}

			_fragmentCountTex = CreateTexture(SizedInternalFormat.R32ui);
			_fragmentHeadTex = CreateTexture(SizedInternalFormat.R32ui);

			_fbo.Generate(_viewport);
		}

		private void SetupNodeBuffers()
		{
			if(_nodeBuffer != 0)
			{
				GL.DeleteBuffer(_nodeBuffer);
				GL.DeleteBuffer(_nodeCounter);
			}

			_nodeBuffer = GL.GenBuffer();
			_nodeBufferSize = (int)Math.Floor(_maxSamples * _viewport.Width * _viewport.Height * _nodeSpace);
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _nodeBuffer);
			GL.BufferData(BufferTarget.ShaderStorageBuffer, 4 + (_nodeBufferSize * 16), 0, BufferUsageHint.DynamicDraw);
			GL.BufferSubData(BufferTarget.ShaderStorageBuffer, 0, sizeof(int), new int[] { _nodeBufferSize });
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

			_nodeCounter = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.AtomicCounterBuffer, _nodeCounter);
			GL.BufferData(BufferTarget.AtomicCounterBuffer, 4, 0, BufferUsageHint.DynamicDraw);
			GL.BindBuffer(BufferTarget.AtomicCounterBuffer, 0);
		}


		/// <summary>
		/// Sets up the buffer for specified viewport size and with the given node space.
		/// </summary>
		/// <param name="viewport">Size of the viewport.</param>
		/// <param name="nodeSpace">Fraction of the maximum size to use for the node buffer.</param>
		public void Setup(Size viewport, float nodeSpace = 0.4f)
		{
			nodeSpace = Math.Clamp(nodeSpace, 0, 1);

			if(viewport != _viewport)
			{
				_viewport = viewport;
				_nodeSpace = nodeSpace;

				SetupNodeBuffers();
				SetupViewportRelatedBuffers();
			}
			else if(nodeSpace != _nodeSpace)
			{
				_nodeSpace = nodeSpace;
				SetupViewportRelatedBuffers();
			}
		}

		/// <summary>
		/// Resets the buffers.
		/// </summary>
		public void Reset()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.ClearTexImage(_fragmentCountTex, 0, PixelFormat.RedInteger, PixelType.UnsignedInt, 0);
			GL.ClearTexImage(_fragmentHeadTex, 0, PixelFormat.RedInteger, PixelType.UnsignedInt, new uint[] { uint.MaxValue });

			GL.BindBuffer(BufferTarget.AtomicCounterBuffer, _nodeCounter);
			GL.ClearBufferData(BufferTarget.AtomicCounterBuffer, PixelInternalFormat.R32ui, PixelFormat.RedInteger, PixelType.UnsignedInt, 0);
			GL.BindBuffer(BufferTarget.AtomicCounterBuffer, 0);
		}

		/// <summary>
		/// Binds frame buffers and textures for use in shaders.
		/// </summary>
		public void Use()
		{
			_fbo.BindFrameBuffer();
			GL.BindImageTexture(0, _fragmentHeadTex, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.R32ui);
			GL.BindImageTexture(1, _fragmentCountTex, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.R32ui);
			GL.BindBufferBase(BufferRangeTarget.AtomicCounterBuffer, 2, _nodeCounter);
			GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 3, _nodeBuffer);
		}

		/// <summary>
		/// Passes buffers and uniforms to a shader for compositing.
		/// </summary>
		/// <param name="shader"></param>
		public void BindComposite(Shader shader)
		{
			shader.Use();

			shader.SetUniform("rFragColor", 0);
			GL.ActiveTexture(TextureUnit.Texture0);
			_fbo.BindColorTexture();

			shader.SetUniform("rFragDepth", 1);
			GL.ActiveTexture(TextureUnit.Texture1);
			_fbo.BindDepthStencilTexture();

			shader.SetUniform("rFragListHead", 2);
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, _fragmentHeadTex);

			shader.SetUniform("rFragListCount", 3);
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, _fragmentCountTex);

			GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, _nodeBuffer);
		}

		/// <summary>
		/// Performs the composite via a screen blit.
		/// </summary>
		public void CompositeBlit()
		{
			BindComposite(Shaders.Shaders.Composite);
			Blit.Render();
		}


		/// <summary>
		/// Serves debugging purposes. 
		/// <br/> Reads the node buffer and orders them in the same way the composite shader would.
		/// </summary>
		public unsafe Node[,][] ReadNodeBuffer(bool depthCheck, bool sortDepth)
		{
			GL.Flush();

			int[] counters = new int[1];
			GL.BindBuffer(BufferTarget.AtomicCounterBuffer, _nodeCounter);
			GL.GetBufferSubData(BufferTarget.AtomicCounterBuffer, 0, 4, counters);
			GL.BindBuffer(BufferTarget.AtomicCounterBuffer, 0);
			int nodeCount = int.Min(_nodeBufferSize, counters[0]);

			Node[] buffer = new Node[nodeCount];
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _nodeBuffer);
			GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, 4, sizeof(Node) * nodeCount, buffer);
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

			uint[,] fragmentCounter = new uint[_viewport.Width, _viewport.Height];
			GL.BindTexture(TextureTarget.Texture2D, _fragmentCountTex);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.RedInteger, PixelType.UnsignedInt, fragmentCounter);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			uint[,] fragmentHeader = new uint[_viewport.Width, _viewport.Height];
			GL.BindTexture(TextureTarget.Texture2D, _fragmentHeadTex);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.RedInteger, PixelType.UnsignedInt, fragmentHeader);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			Node[,][] result = new Node[_viewport.Width, _viewport.Height][];

			uint[,] depthStencilBuffer = new uint[_viewport.Width, _viewport.Height];
			if(depthCheck)
			{
				_fbo.BindDepthStencilTexture();
				GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, depthStencilBuffer);
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}

			for(int y = 0; y < _viewport.Height; y++)
			{
				for(int x = 0; x < _viewport.Width; x++)
				{
					uint fragCount = fragmentCounter[x, y];
					if(fragCount == 0)
					{
						continue;
					}

					Node[] nodes = new Node[fragCount];

					uint index = fragmentHeader[x, y];
					int counter = 0;
					float depth = depthCheck ? (depthStencilBuffer[x, y] >> 8) / (float)0xFFFFFF : 1;
					while(index != Node.FragmentListNull)
					{
						nodes[counter] = buffer[index];
						index = nodes[counter].Next;

						if(nodes[counter].Depth <= depth)
						{
							counter++;
						}
					}

					if(counter > 0)
					{
						if(counter != nodes.Length)
						{
							Array.Resize(ref nodes, counter);
						}

						if(sortDepth)
						{
							Array.Sort(nodes, (a, b) => a.Depth.CompareTo(b.Depth));
						}

						result[x, y] = nodes;
					}
				}
			}

			return result;
		}

		private static Vector4 BlendColors(uint blendmode, Vector4 sourceColor, Vector4 destinationColor)
		{
			static Vector4 GetBlendFactor(uint mode, Vector4 source, Vector4 destination)
			{
				return mode switch
				{
					1 => new(1), //BLEND_ONE
					2 => source, //BLEND_OTHER
					3 => new Vector4(1) - source, //BLEND_OTHER_INV
					4 => new(source.W), //BLEND_SRC
					5 => new(1 - source.W),  //BLEND_SRC_INV
					6 => new(destination.W), //BLEND_DST
					7 => new(1 - destination.W), //BLEND_DST_INV
					_ => new(0), //BLEND_ZERO
				};
			}

			uint srcBlend = blendmode & 7;
			uint dstBlend = (blendmode >> 3) & 7;

			Vector4 src = GetBlendFactor(srcBlend, sourceColor, destinationColor);
			Vector4 dst = GetBlendFactor(dstBlend, sourceColor, destinationColor);
			return (sourceColor * src) + (destinationColor * dst);
		}

		/// <summary>
		/// Serves debugging purposes.
		/// <br/> Performs composite on the CPU.
		/// </summary>
		public unsafe Texture CPUComposite()
		{
			Node[,][] nodes = ReadNodeBuffer(true, true);

			Color[,] pixels = new Color[_viewport.Width, _viewport.Height];
			_fbo.BindColorTexture();
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			byte[] pixelData = new byte[_viewport.Width * _viewport.Height * 4];

			for(int y = 0; y < _viewport.Height; y++)
			{
				for(int x = 0; x < _viewport.Width; x++)
				{
					Node[] pixelNodes = nodes[x, y];

					if(pixelNodes != null)
					{

						int lastNodeIndex = pixelNodes.Length - 1;
						Vector4 col = pixels[x, y].FloatVector;

						for(int i = 0; i < lastNodeIndex; i++)
						{
							int swapIndex = i;
							Node node = pixelNodes[swapIndex];

							for(int j = i + 1; j < pixelNodes.Length; j++)
							{
								Node checkNode = pixelNodes[j];
								if(checkNode.Depth > node.Depth)
								{
									swapIndex = j;
									node = checkNode;
								}
							}

							col = BlendColors(node.Flags, node.Color.FloatVector, col);

							if(swapIndex != i)
							{
								pixelNodes[swapIndex] = pixelNodes[i];
							}
						}

						Node lastNode = pixelNodes[lastNodeIndex];
						pixels[x, y] = new(BlendColors(lastNode.Flags, lastNode.Color.FloatVector, col))
						{
							Alpha = 0xFF
						};
					}
				}
			}

			for(int y = 0; y < _viewport.Height; y++)
			{
				fixed(Color* colorPtr = pixels)
				{
					int stride = 4 * _viewport.Width;
					int copyOffset = y * stride;
					int pasteOffset = (_viewport.Height - y - 1) * stride;
					Marshal.Copy((nint)colorPtr + copyOffset, pixelData, pasteOffset, stride);
				}
			}

			return new ColorTexture(_viewport.Width, _viewport.Height, pixelData);
		}
	}
}
