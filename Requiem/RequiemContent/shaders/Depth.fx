float4x4 World		: WORLD;
float4x4 View		: VIEW;
float4x4 Projection : PROJECTION;

float4x4		FinalTransforms		[56];
int				NumVertInfluences	= 1;

struct Depth_VS_OUTPUT
{
    float4 Position : POSITION0;
};

Depth_VS_OUTPUT Depth_vs_main( float4 inPosition : POSITION0,
									float2 inTexcoord : TEXCOORD0 )
{
    Depth_VS_OUTPUT output;

    output.Position = mul(mul(mul(inPosition, World), View), Projection);
    
    return output;    
}

Depth_VS_OUTPUT DepthLine_vs_main(	float3 inPos : POSITION0)
{
	Depth_VS_OUTPUT output = (Depth_VS_OUTPUT)0;
	output.Position = mul(mul(mul(float4(inPos,1), World), View), Projection);
	return output;
}

Depth_VS_OUTPUT DepthAnimated_vs_main( float4 inPosition : POSITION0,
										float4 inWeights	: BLENDWEIGHT0,
										int4  inBoneIndices : BLENDINDICES0 )
{
    Depth_VS_OUTPUT output;

	float lastWeight = 0.0f;
	float4 pos = float4(0,0,0,1);
	int num = NumVertInfluences -1;

	for(int i=0; i < num; ++i)
	{
		lastWeight += inWeights[i];
		pos + inWeights[i] * mul(inPosition, FinalTransforms[inBoneIndices[i]]);
	}

	lastWeight = 1.0f - lastWeight;
	pos += lastWeight * mul(inPosition, FinalTransforms[inBoneIndices[num]]);
	pos.w = 1.0f;

	output.Position = mul(mul(mul(pos, World), View), Projection);
    
    return output;    
}

float4 Depth_ps_main(float4 Position : TEXCOORD0) : COLOR0
{
    return float4(0,0,0,0);
}

technique Depth
{
	pass P0
	{
		VertexShader = compile vs_2_0 Depth_vs_main();
		PixelShader = compile ps_2_0 Depth_ps_main();
	}
}

technique AnimatedDepth
{
	pass P0
	{
		VertexShader = compile vs_2_0 DepthAnimated_vs_main();
		PixelShader = compile ps_2_0 Depth_ps_main();
	}
}

technique LineDepth
{
	pass P0
	{
		VertexShader = compile vs_2_0 DepthLine_vs_main();
		PixelShader = compile ps_2_0 Depth_ps_main();
	}
}