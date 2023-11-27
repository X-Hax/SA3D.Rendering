#version 430 core

#define OIT_WRITE
#define OIT_READ
#include OIT
#include GlobalInputs

out vec4 FragColor;

void main()
{
	float t;
	FragColor = compositeOIT(ivec2(gl_FragCoord.xy), t) * uSurfaceDiffuse;
	AddOIT(FragColor, 0xAC);
}