#version 430 core

#include Matrices

layout(location = 0) in vec2 aPosition;
layout(location = 3) in vec2 aUV0;

out vec2 vUV0;

void main()
{
	vUV0 = aUV0;

	gl_Position = uMVP * vec4(aPosition, 0.0f, 1.0f);
}