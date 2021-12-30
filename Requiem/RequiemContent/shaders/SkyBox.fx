float4x4		World				: WORLD;
float4x4		View				: VIEW;
float4x4		Projection			: PROJECTION;
float3			CameraPosition;

texture SkyBoxTexture : register(t6);	//skybox
samplerCUBE skyBoxSampler : register(s6) = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = MIRROR; 
   AddressV = MIRROR; 
};

struct SkyBox_VS_OUTPUT
{
	float4 Position : POSITION0;
	float3 TexCoord : TEXCOORD0;
};

SkyBox_VS_OUTPUT SkyBox_vs_main( float4 inPosition : POSITION0 )
{
	SkyBox_VS_OUTPUT output = (SkyBox_VS_OUTPUT)0;

	output.Position = mul(mul(mul(inPosition, World), View), Projection);
	float4 VertexPosition = mul(inPosition, World);
	output.TexCoord = VertexPosition - CameraPosition;

    return output;
}

float4 SkyBox_ps_main( float3 inTexCoord : TEXCOORD0 ) : COLOR0
{
	return texCUBE(skyBoxSampler, normalize(inTexCoord));
}

technique SkyBox
{
	pass P0
	{
		VertexShader = compile vs_2_0 SkyBox_vs_main();
		PixelShader = compile ps_2_0 SkyBox_ps_main();
	}
}