#ifndef MATRICES
#define MATRICES

layout(std140, binding = 4) uniform Matrices
{
	mat4 uWorld;
	mat4 uNormalWorld;
	mat4 uMVP;
};

#endif