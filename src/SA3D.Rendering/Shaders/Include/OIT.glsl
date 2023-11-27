// Maximum number of fragments to be sorted per pixel
#define MAX_FRAGMENTS 16
#define FRAGMENT_LIST_NULL 0xFFFFFFFF

// Fragment list node.
struct OITNode
{
	float depth; // fragment depth
	uint  color; // 32-bit packed fragment color
	uint  flags; // source blend, destination blend
	uint  next;  // index of the next entry, or FRAGMENT_LIST_NULL
};

#ifdef OIT_WRITE

layout(binding = 0, r32ui) coherent uniform uimage2D wFragListHead;
layout(binding = 1, r32ui) coherent uniform uimage2D wFragListCount;
layout(binding = 2) uniform atomic_uint wFragListNodeCounter;
layout(std140, binding = 3) coherent buffer wFragNodeList
{
	int wFragListLimit;
	OITNode wFragListNodes[];
};

#define BLEND_MASK 0x3F
#define SRC_INVSRC 0x2C

void AddOIT(vec4 color, uint flags)
{
	if((flags & BLEND_MASK) == SRC_INVSRC && color.a > 0.99f)
		return;

	ivec2 coords = ivec2(gl_FragCoord.xy);
	
	uint fragmentCount = imageAtomicAdd(wFragListCount, coords, 1);
	if (fragmentCount >= MAX_FRAGMENTS)
		discard;

	uint newIndex = atomicCounterIncrement(wFragListNodeCounter);
	if(newIndex >= wFragListLimit)
		discard;

	uint oldIndex = imageAtomicExchange(wFragListHead, coords, newIndex);

	OITNode n;
	n.depth = gl_FragCoord.z;
	n.color = packUnorm4x8(color);
	n.flags = flags;
	n.next = oldIndex;
	
	wFragListNodes[newIndex] = n;
	discard;
}

#endif

#ifdef OIT_READ

uniform sampler2D rFragColor;
uniform sampler2D rFragDepth;
uniform usampler2D rFragListHead;
uniform usampler2D rFragListCount;
layout(std140, binding = 4) readonly buffer rFragNodeList
{
	int rFragListLimit;
	OITNode rFragListNodes[];
};

// SA3D enum
#define BLEND_ZERO      0
#define BLEND_ONE       1
#define BLEND_OTHER     2
#define BLEND_OTHER_INV	3
#define BLEND_SRC       4
#define BLEND_SRC_INV   5
#define BLEND_DST       6
#define BLEND_DST_INV   7

vec4 get_blend_factor(uint mode, vec4 source, vec4 destination)
{
	switch (mode)
	{
		case BLEND_ZERO:
			return vec4(0.0f);
		case BLEND_ONE:
			return vec4(1.0f);
		case BLEND_OTHER:
			return source;
		case BLEND_OTHER_INV:
			return 1.0f - source;
		case BLEND_SRC:
			return source.aaaa;
		case BLEND_SRC_INV:
			return 1.0f - source.aaaa;
		case BLEND_DST:
			return destination.aaaa;
		case BLEND_DST_INV:
			return 1.0f - destination.aaaa;
	}
}

vec4 blend_colors(uint blendmode, vec4 sourceColor, vec4 destinationColor)
{
	uint srcBlend = blendmode & 7;
	uint dstBlend = (blendmode >> 3) & 7;

	vec4 src = get_blend_factor(srcBlend, sourceColor, destinationColor);
	vec4 dst = get_blend_factor(dstBlend, sourceColor, destinationColor);
	return (sourceColor * src) + (destinationColor * dst);
}

vec4 compositeOIT(ivec2 coords, out float opaqueDepth)
{
	vec4 col = texelFetch(rFragColor, coords, 0);
	uint nodeCount = texelFetch(rFragListCount, coords, 0).r;
	opaqueDepth = texelFetch(rFragDepth, coords, 0).r;

	if(nodeCount > 0u)
	{
		OITNode fragments[MAX_FRAGMENTS];
		uint index = texelFetch(rFragListHead, coords, 0).r;
		int count = 0;

		while(index != FRAGMENT_LIST_NULL)
		{
			fragments[count] = rFragListNodes[index];
			index = fragments[count].next;

			if(fragments[count].depth <= opaqueDepth)
			{
				count++;
			}
		}

		if(count > 0)
		{
			int lastNodeIndex = count - 1;
			for (int i = 0; i < lastNodeIndex; i++)
			{
				int swapIndex = i;
				OITNode node = fragments[swapIndex];

				for(int j = i + 1; j < count; j++)
				{
					OITNode checkNode = fragments[j];
					if(checkNode.depth > node.depth)
					{
						swapIndex = j;
						node = checkNode;
					}
				}

				col = blend_colors(node.flags, unpackUnorm4x8(node.color), col);

				if (swapIndex != i)
				{
					fragments[swapIndex] = fragments[i];
				}
			}

			OITNode lastNode = fragments[lastNodeIndex];
			col = blend_colors(lastNode.flags, unpackUnorm4x8(lastNode.color), col);
		}

	};

	return col;
}

#endif
