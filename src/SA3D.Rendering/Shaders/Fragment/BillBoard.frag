#version 430 core

#include Common

in vec2 vUV0;

out vec4 FragColor;

uniform sampler2D texture0;

void main()
{
	vec4 color = uSurfaceAmbient * texture(texture0, vUV0);
	PerformOIT(color);
	FragColor = color;
}