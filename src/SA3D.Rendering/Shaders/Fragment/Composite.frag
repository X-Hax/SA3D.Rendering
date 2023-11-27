#version 430 core

#define OIT_READ
#include OIT

out vec4 FragColor;
out float gl_FragDepth;

void main()
{
	FragColor = compositeOIT(ivec2(gl_FragCoord.xy), gl_FragDepth);
}