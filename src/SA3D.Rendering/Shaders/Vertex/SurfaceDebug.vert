#version 430 core

#include Matrices

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec4 aCol0;
layout(location = 3) in vec2 aUV0;
layout(location = 4) in float aWeightColor;

out vec3 vFragpos;
out vec3 vNormal;
out vec2 vUV0;
out vec4 vColor0;
out float vWeightColor;

void main()
{
	vUV0 = aUV0;
	vColor0 = aCol0;
	vWeightColor = aWeightColor;

	gl_Position = uMVP * vec4(aPosition, 1.0f);

	vNormal = normalize(mat3(uNormalWorld) * aNormal);
	vFragpos = vec3(uWorld * vec4(aPosition, 1.0f));
}