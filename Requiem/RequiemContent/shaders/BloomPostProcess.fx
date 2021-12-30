float2 ScreenHalfPixel;
float BloomThreshold;
float BloomIntensity;
float BaseIntensity;
float BloomSaturation;
float BaseSaturation;

#define SAMPLE_COUNT 15
float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

texture Texture : register(t0);
sampler sceneSampler : register(s0) = sampler_state
{
	Texture = (Texture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture BloomTexture : register(t1);
sampler bloomSampler : register(s1) = sampler_state
{
	Texture = (BloomTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};


float4 AdjustSaturation(float4 color, float saturation)
{
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));
    return lerp(grey, color, saturation);
}

struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT default_vs_main( float3 inPos:POSITION0, 
							float2 inTexCoord:TEXCOORD0 )
{
	VS_OUTPUT output = (VS_OUTPUT)0;

	output.Position = float4(inPos,1);
	output.TexCoord = inTexCoord - ScreenHalfPixel;
	
	return output;
}


float4 GaussianBlur_ps_main(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(sceneSampler, texCoord + SampleOffsets[i]) * SampleWeights[i];
    }
    return c;
}


float4 BloomExtract_ps_main(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(sceneSampler, texCoord);
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


float4 BloomCombine_ps_main(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 bloom = tex2D(bloomSampler, texCoord);
    float4 base = tex2D(sceneSampler, texCoord);
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    
	base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    base *= (1 - saturate(bloom));
    
	return base + bloom;
}




technique GaussianBlur
{
    pass P0
    {
		VertexShader = compile vs_2_0 default_vs_main();
        PixelShader = compile ps_2_0 GaussianBlur_ps_main();
    }
}

technique BloomExtract
{
    pass P0
    {
		VertexShader = compile vs_2_0 default_vs_main();
        PixelShader = compile ps_2_0 BloomExtract_ps_main();
    }
}

technique BloomCombine
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 default_vs_main();
        PixelShader = compile ps_2_0 BloomCombine_ps_main();
    }
}

