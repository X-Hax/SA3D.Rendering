#ifndef COMMON_FUNC
#define COMMON_FUNC

#define OIT_WRITE
#include OIT
#include GlobalInputs

vec3 getViewDirection(vec3 fragmentPosition)
{
	if(uViewDirection.x == 0 && uViewDirection.y == 0 && uViewDirection.z == 0)
	{
		return normalize(uViewPosisition - fragmentPosition);
	}
	return uViewDirection;
}

#ifndef LIGHT_FACTOR_FUNC
#define LIGHT_FACTOR_FUNC

float surfaceLighting(int index, vec3 normal)
{
	return max(0, dot(normal, uLights[index].direction));
}

#endif

float surfaceSpecular(vec3 lightDirection, vec3 fragmentPosition, vec3 normal, float specularExponent)
{
	vec3 viewDirection = getViewDirection(fragmentPosition);
	vec3 reflectDir = reflect(-lightDirection, normal);
	return pow(max(dot(viewDirection, reflectDir), 0.0f), specularExponent);
}

vec4 sampleTexture(vec2 uv, vec3 normal, sampler2D sampler)
{
	if((uSurfaceFlags & SF_USE_TEXTURE) == 0)
	{
		return vec4(1.0f);
	}

	if((uSurfaceFlags & SF_NORMAL_MAPPING) != 0)
	{
		vec3 reflected = reflect(uViewDirection, normal);
		float m = 2.8284271247461903f * sqrt( reflected.z + 1.0f );
		uv = reflected.xy / m + 0.5f;
	}

	return texture(sampler, uv);
}

void PerformOIT(vec4 color)
{
	if((uSurfaceFlags & SF_BLEND_MASK) != 0)
	{
		AddOIT(color, uSurfaceFlags);
	}
}

vec4 CalculateLighting(vec4 color, vec3 fragmentPosition, vec3 normal)
{
	color *= uSurfaceDiffuse;
	if((uSurfaceFlags & SF_HAS_NORMALS) != 0)
	{
		if(!uDisableLighting && (uSurfaceFlags & SF_NO_LIGHTING) == 0)
		{
			vec3 ambientColor = vec3(0.0f);
			vec3 lightColor = vec3(0.0f);
			vec3 specularColor = vec3(0.0f);

			for(int i = 0; i < 4; i++)
			{
				Light light = uLights[i];

				ambientColor += light.ambientColor.rgb * light.ambientIntensity;

				if(!uDisableSurfaceAmbient)
				{
					ambientColor *= uSurfaceAmbient.rgb;
				}

				vec3 diffuseCol = light.color.rgb * light.intensity;
			
				lightColor = max(lightColor, diffuseCol * surfaceLighting(i, normal));

				float exponent = surfaceSpecular(light.direction, fragmentPosition, normal, uSurfaceExponent);
				specularColor = max(specularColor, diffuseCol * uSurfaceSpecular.rgb * exponent * 0.5f);
			}

			
			if((uSurfaceFlags & SF_NO_AMBIENT) == 0)
			{
				lightColor += ambientColor;
			}

			color.rgb *= clamp(lightColor, 0.0f, 1.0f);
	
			if((uSurfaceFlags & SF_NO_SPECULAR) == 0 && !uDisableSpecular)
			{
				color.rgb += specularColor;
			}
		}
	}

	PerformOIT(color);

	return color;
}

#endif