#ifndef GLOBAL_INPUT
#define GLOBAL_INPUT

layout(std140, binding = 5) uniform RenderSettings
{
									// alignment	offset
	bool uDisableLighting;			// 4			0
	bool uDisableSpecular;			// 4			4
	bool uDisableSurfaceAmbient;	// 4			8
};

layout(std140, binding = 6) uniform Camera
{
							// alignment	offset
	vec3 uViewPosisition;	// 16			0
	vec3 uViewDirection;	// 16			16
};

struct Light
{
							// alignment	offset
	vec3 direction;			// 16			0
	vec4 color;				// 16			16
	vec4 ambientColor;		// 16			32
	float intensity;		// 4			48
	float ambientIntensity;	// 4			52
};

layout(std140, binding = 7) uniform Lighting
{
	Light uLights[4];
};

layout(std140, binding = 8) uniform Surface
{
							// alignment	offset
	vec4 uSurfaceDiffuse;	// 16			0	
	vec4 uSurfaceSpecular;	// 16			16
	vec4 uSurfaceAmbient;	// 16			32
	float uSurfaceExponent;	// 4			48
	uint uSurfaceFlags;		// 4			52
};

// Surface flag values
#define SF_BLEND_MASK            0xFF
#define SF_USE_TEXTURE           0x100
#define SF_ANISOTROPIC_FITLERING 0x200
#define SF_CLAMP_U               0x400
#define SF_CLAMP_V               0x800
#define SF_MIRROR_U              0x1000
#define SF_MIRROR_V              0x2000
#define SF_NORMAL_MAPPING	     0x4000
#define SF_NO_LIGHTING		     0x8000
#define SF_NO_AMBIENT		     0x10000
#define SF_NO_SPECULAR		     0x20000
#define SF_FLAT				     0x40000
#define SF_USE_ALPHA		     0x80000
#define SF_HAS_COLORS		     0x40000000
#define SF_HAS_NORMALS		     0x80000000

#endif