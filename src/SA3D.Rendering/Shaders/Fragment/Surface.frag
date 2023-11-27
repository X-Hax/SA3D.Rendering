#version 430 core

#include Common

in vec3 vFragpos;
in vec3 vNormal;
in vec2 vUV0;
in vec4 vColor0;

out vec4 FragColor;

uniform sampler2D texture0;

void main()
{
	vec4 color = vColor0 * sampleTexture(vUV0, vNormal, texture0);

	FragColor = CalculateLighting(color, vFragpos, vNormal);
}