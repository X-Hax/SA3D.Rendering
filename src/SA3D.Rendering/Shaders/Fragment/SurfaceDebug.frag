#version 430 core

#include GlobalInputs

uniform int debugMode;

#define DBM_SMOOTH		1
#define DBM_FALLOFF		2
#define DBM_NORMALS		3
#define DBM_COLORS		4
#define DBM_WEIGHTS		5
#define DBM_TEXCOORDS	6
#define DBM_TEXTURES	7
#define DBM_LIGHTING	8
#define DBM_CULLING		9

#define LIGHT_FACTOR_FUNC
float surfaceLighting(int index, vec3 normal)
{
	if(debugMode == DBM_FALLOFF)
	{
		if(index > 0)
			return 0.0f;
		return max(0.0f, dot(normal, -uViewDirection));
	}

	float fac = dot(normal, uLights[index].direction);
	if(debugMode == DBM_SMOOTH)
	{
		return (fac + 1.0f) * 0.5f;
	}

	return max(0.0f, fac);
}

#include Common

in vec3 vFragpos;
in vec3 vNormal;
in vec2 vUV0;
in vec4 vColor0;
in float vWeightColor;

out vec4 FragColor;

uniform sampler2D texture0;

vec4 calcWeightColor()
{
	float hue = mod(vWeightColor * -0.625f + 0.666f, 1.0f) * 6.0f;
    int index = int(hue);
    float ff = hue - index;
    float q = 1.0f - ff;

	vec4 result = vec4(0.0f, 0.0f, 0.0f, 1.0f);
	if(index == 0)
	{
		result.r = 1.0f;	
		result.g = ff;
	}
	else if(index == 1)
	{
		result.r = q;
		result.g = 1.0f;	
	}
	else if(index == 2)
	{
		result.g = 1.0f;	
		result.b = ff;
	}
	else if(index == 3)
	{
		result.g = q;
		result.b = 1.0f;	
	}
	else if(index == 4)
	{
		result.b = 1.0f;	
		result.r = ff;
	}
	else
	{
		result.b = q;
		result.r = 1.0f;	
	}

	return result;
}

void main()
{
	vec4 color;

	if(debugMode == DBM_NORMALS)
	{
		color = vec4(vNormal * 0.5f + 0.5f, 1.0f);
	}
	else if(debugMode == DBM_TEXCOORDS)
	{
		color = vec4(mod(vUV0, 1.0f), 1.0f, 1.0f);
	}
	else if(debugMode == DBM_COLORS)
	{
		color = vColor0;
		PerformOIT(color);
	}
	else if(debugMode == DBM_WEIGHTS)
	{
		color = calcWeightColor();
	}
	else if(debugMode == DBM_CULLING)
	{
		if(gl_FrontFacing)
			color = vec4(0.0f, 0.0f, 1.0f, 1.0f);
		else color = vec4(1.0f, 0.0f, 0.0f, 1.0f);
	}
	else if(debugMode == DBM_TEXTURES)
	{
		color = sampleTexture(vUV0, vNormal, texture0);
		PerformOIT(color);
	}
	else if(debugMode == DBM_LIGHTING)
	{
		color = vColor0 * sampleTexture(vUV0, vNormal, texture0);
		color.rgb = vec3(1.0f);
		color = CalculateLighting(color, vFragpos, vNormal);
	}
	else
	{
		color = vColor0 * sampleTexture(vUV0, vNormal, texture0);
		color = CalculateLighting(color, vFragpos, vNormal);
	}

	FragColor = color;
}