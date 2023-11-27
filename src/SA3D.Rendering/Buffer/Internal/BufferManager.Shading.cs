using OpenTK.Graphics.OpenGL4;
using SA3D.Modeling.Mesh;
using SA3D.Modeling.Mesh.Buffer;
using SA3D.Modeling.Structs;
using SA3D.Rendering.Buffer.Internal;
using SA3D.Rendering.Structs;
using System;
using System.IO;
using System.Numerics;

namespace SA3D.Rendering.Buffer
{
	internal partial class BufferManager
	{
		public UniformBuffer MatrixUBO { get; }
		public UniformBuffer SettingsUBO { get; }
		public UniformBuffer CameraUBO { get; }
		public UniformBuffer LightingUBO { get; }
		public UniformBuffer SurfaceUBO { get; }
		public UniformBuffer SpriteInfoUBO { get; }

		private RenderSettings _activeSettings;

		public void SetRenderSettings(RenderSettings settings)
		{
			_activeSettings = settings;

			SettingsUBO.ResetPosition();

			SettingsUBO.BufferWriter.WriteInt(_activeSettings.DisableLighting ? 1 : 0);
			SettingsUBO.BufferWriter.WriteInt(_activeSettings.DisableSpecular ? 1 : 0);
			SettingsUBO.BufferWriter.WriteInt(_activeSettings.DisableSurfaceAmbient ? 1 : 0);

			SettingsUBO.BufferData();
		}

		public void SetShaderCamera(Camera camera)
		{
			CameraUBO.ResetPosition();

			CameraUBO.BufferWriter.WriteVector3(camera.Realposition);
			CameraUBO.BufferWriter.Stream.Seek(4, SeekOrigin.Current);

			CameraUBO.BufferWriter.WriteVector3(camera.Forward);

			CameraUBO.BufferData();
		}

		public void SetShaderLighting(Lighting[] lighting)
		{
			if(lighting.Length != 4)
			{
				throw new ArgumentException("4 Lights need to be provided!");
			}

			LightingUBO.ResetPosition();

			foreach(Lighting light in lighting)
			{
				LightingUBO.BufferWriter.WriteVector3(light.Direction);
				LightingUBO.BufferWriter.Stream.Seek(4, SeekOrigin.Current);

				LightingUBO.WriteColor(light.Color);
				LightingUBO.WriteColor(light.Ambient);
				LightingUBO.BufferWriter.WriteFloat(light.ColorIntensity);
				LightingUBO.BufferWriter.WriteFloat(light.AmbientIntensity);
				LightingUBO.BufferWriter.WriteEmpty(8);
			}

			LightingUBO.BufferData();
		}

		public void SetSurfaceMaterial(BufferMaterial material, bool hasNormals, bool hasColors)
		{
			SurfaceUBO.ResetPosition();

			Color color = material.Diffuse;

			color.AlphaF = material.UseAlpha
				? MathF.Max(0, color.AlphaF - _activeSettings.TransparencySubtract)
				: MathF.Max(0, 1 - _activeSettings.TransparencySubtract);

			SurfaceUBO.WriteColor(color);
			SurfaceUBO.WriteColor(material.Specular);
			SurfaceUBO.WriteColor(material.Ambient);

			SurfaceUBO.BufferWriter.WriteFloat(material.SpecularExponent);

			uint matFlags = (uint)material.Attributes;
			if(ActiveTextures == null || material.TextureIndex > ActiveTextures.Textures.Count)
			{
				matFlags &= ~(uint)MaterialAttributes.UseTexture;
			}

			matFlags <<= 8;

			if(material.UseAlpha)
			{
				matFlags |= (uint)material.SourceBlendMode;
				matFlags |= (uint)material.DestinationBlendmode << 3;
				matFlags |= 0x80; // mark as alpha
			}
			else if(_activeSettings.TransparencySubtract > 0)
			{
				matFlags |= (uint)BlendMode.SrcAlpha;
				matFlags |= (uint)BlendMode.SrcAlphaInverted << 3;
				matFlags |= 0x80; // mark as alpha
			}

			if(hasNormals)
			{
				matFlags |= 0x80000000;
			}

			if(hasColors)
			{
				matFlags |= 0x40000000;
			}

			SurfaceUBO.BufferWriter.WriteUInt(matFlags);

			SurfaceUBO.BufferData();

			if(material.UseTexture && BindTexture(TextureUnit.Texture0, (int)material.TextureIndex))
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)material.TextureFiltering.ToGLMinFilter());
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)material.TextureFiltering.ToGLMagFilter());
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, material.WrapModeU());
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, material.WrapModeV());
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, material.AnisotropicFiltering ? 4 : 0);
			}

			if(material.BackfaceCulling && !_activeSettings.DisableBackfaceCulling)
			{
				GL.Enable(EnableCap.CullFace);
			}
			else
			{
				GL.Disable(EnableCap.CullFace);
			}
		}

		public void SetSpriteInfo(Matrix4x4 matrix, Vector2 offset, Vector2 scale, Color color)
		{
			SpriteInfoUBO.ResetPosition();
			SpriteInfoUBO.WriteMatrix(matrix);
			SpriteInfoUBO.BufferWriter.WriteVector2(offset);
			SpriteInfoUBO.BufferWriter.WriteVector2(scale);
			SpriteInfoUBO.WriteColor(color);
			SpriteInfoUBO.BufferData();
		}

		public void BindShaderBuffers()
		{
			MatrixUBO.Bind(4);
			SettingsUBO.Bind(5);
			CameraUBO.Bind(6);
			LightingUBO.Bind(7);
			SurfaceUBO.Bind(8);
		}

		public void BindSpriteBuffers()
		{
			SpriteInfoUBO.Bind(4);
			GL.BindVertexArray(SpriteHandle.VertexArrayObject);
		}

	}
}
