#version 430 core

#include SpriteInfo

layout(location = 0) in vec2 aPos;

out vec2 vUV;

void main()
{
	gl_Position = uMP * vec4(aPos, 0.0f, 1.0f);
	vUV = uUVOffset.xy + aPos * uUVOffset.zw;
}