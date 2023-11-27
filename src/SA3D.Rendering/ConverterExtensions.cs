using OpenTK.Graphics.OpenGL4;
using SA3D.Modeling.Mesh;
using SA3D.Modeling.Mesh.Buffer;
using System;

namespace SA3D.Rendering
{
	internal static class ConverterExtensions
	{
		public static BlendingFactor ToGLBlend(this BlendMode instr)
		{
			return instr switch
			{
				BlendMode.One => BlendingFactor.One,
				BlendMode.Other => BlendingFactor.SrcColor,
				BlendMode.OtherInverted => BlendingFactor.OneMinusSrcColor,
				BlendMode.SrcAlpha => BlendingFactor.SrcAlpha,
				BlendMode.SrcAlphaInverted => BlendingFactor.OneMinusSrcAlpha,
				BlendMode.DstAlpha => BlendingFactor.DstAlpha,
				BlendMode.DstAlphaInverted => BlendingFactor.OneMinusDstAlpha,
				BlendMode.Zero or _ => BlendingFactor.Zero,
			};
		}

		public static TextureMinFilter ToGLMinFilter(this FilterMode filter)
		{
			return filter switch
			{
				FilterMode.Nearest => TextureMinFilter.NearestMipmapNearest,
				FilterMode.Bilinear => TextureMinFilter.LinearMipmapNearest,
				FilterMode.Trilinear => TextureMinFilter.LinearMipmapLinear,
				FilterMode.Blend or _ => throw new InvalidCastException($"{filter} has no corresponding OpenGL filter"),
			};
		}

		public static TextureMagFilter ToGLMagFilter(this FilterMode filter)
		{
			return filter switch
			{
				FilterMode.Nearest => TextureMagFilter.Nearest,
				FilterMode.Bilinear
				or FilterMode.Trilinear => TextureMagFilter.Linear,
				FilterMode.Blend or _ => throw new InvalidCastException($"{filter} has no corresponding OpenGL filter"),
			};
		}

		public static int WrapModeU(this BufferMaterial mat)
		{
			if(mat.ClampU)
			{
				return mat.MirrorU
					? (int)All.MirroredRepeat
					: (int)TextureWrapMode.ClampToEdge;
			}
			else
			{
				return mat.MirrorU
					? (int)TextureWrapMode.MirroredRepeat
					: (int)TextureWrapMode.Repeat;
			}
		}

		public static int WrapModeV(this BufferMaterial mat)
		{
			if(mat.ClampV)
			{
				return mat.MirrorV
					? (int)All.MirroredRepeat
					: (int)TextureWrapMode.ClampToEdge;
			}
			else
			{
				return mat.MirrorV
					? (int)TextureWrapMode.MirroredRepeat
					: (int)TextureWrapMode.Repeat;
			}
		}

	}
}
