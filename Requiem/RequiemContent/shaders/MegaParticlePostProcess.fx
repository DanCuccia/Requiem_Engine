texture SceneTexture;
sampler2D SceneSampler = sampler_state
{
	Texture = <SceneTexture>;
    ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture Texture;
sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
    ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float Scale = 1.5f;
float2 ScreenHalfPixel;

struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT vs_main( float3 position : POSITION0, float2 texCoord : TEXCOORD0 )
{
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Position = float4(position, 1);
	output.TexCoord = texCoord - ScreenHalfPixel;
	return output;
}

float4 Noise_ps_main(float2 TexCoord :TEXCOORD0) : COLOR
{
    TexCoord += Scale * (tex2D(TextureSampler, TexCoord).xy - 0.5f)/15;
	TexCoord = clamp(TexCoord, 0, 1);
    
    return tex2D(SceneSampler, TexCoord);
} 

float4 Merge_ps_main(float2 TexCoord :TEXCOORD0) : COLOR0
{
    float4 colorD = tex2D(SceneSampler, TexCoord);
    float4 colorS = tex2D(TextureSampler, TexCoord);
    return (colorS * colorS.a + colorD * (1.0-colorS.a));
}

technique FractalNoise
{
    pass P0
    {
		VertexShader = compile vs_2_0 vs_main();
		PixelShader = compile ps_2_0 Noise_ps_main();
    }
}

technique MegaMerge
{
	pass P0
	{
		VertexShader = compile vs_2_0 vs_main();
		PixelShader = compile ps_2_0 Merge_ps_main();
	}
}