#ifndef SPRITEINFO
#define SPRITEINFO

layout(std140, binding = 4) uniform SpriteInfo
{
	mat4 uMP;
	vec4 uUVOffset;
	vec4 uColor;
};

#endif