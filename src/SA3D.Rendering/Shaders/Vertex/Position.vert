#version 430 core

#include Matrices

layout(location = 0) in vec3 aPosition;

void main()
{
	gl_Position = uMVP * vec4(aPosition, 1.0f);
}