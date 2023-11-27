#version 430 core

#include SpriteInfo

out vec4 FragColor;

in vec2 vUV;

uniform sampler2D sprite;

void main()
{
	FragColor = texture(sprite, vUV) * uColor;
}