float4x4		World				: WORLD;
float4x4		View				: VIEW;
float4x4		ViewInverse			: VIEWINVERSE;
float4x4		Projection			: PROJECTION;
float			Time				: TIME;

texture WaterNormalMap : register(t0);
sampler waterNormalSampler : register(s0) = sampler_state
{
	texture = <WaterNormalMap>;
	MipFilter 	= LINEAR;
	MinFilter 	= LINEAR;
	MagFilter 	= LINEAR;
	AddressU	= WRAP; 
	AddressV	= WRAP; 
};

texture SkyBoxTexture : register(t1);
samplerCUBE skyBoxSampler : register(s1) = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = MIRROR; 
   AddressV = MIRROR; 
};

// water controllers
//-------------------
float	BumpHeight			= 0.5f;
float2	TextureScale		= { 4.0f, 4.0f };
float2	BumpSpeed			= { 0.0f, 0.0f };
float	FresnelBias			= 0.025f;
float	FresnelPower		= 1.0f;
float	HDRMultiplier		= 1.0f;
float4	DeepColor			= { 0.0f, 0.40f, 0.50f, 1.0f };
float4	ShallowColor		= { 0.55f, 0.75f, 0.75f, 1.0f };
float4	ReflectionColor		= { 1.0f, 1.0f, 1.0f, 1.0f };
float	ReflectionAmount	= 0.5f;
float	WaterAmount			= 0.5f;

struct Water_VS_OUTPUT
{
	float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
	float3 TanToCube[3]	: TEXCOORD1;
	float2 Bump0 : TEXCOORD4;
	float2 Bump1 : TEXCOORD5;
	float2 Bump2 : TEXCOORD6;
	float3 View	: TEXCOORD7;
};

Water_VS_OUTPUT Water_vs_main( float4 inPosition : POSITION0,
								float3 inNormal : NORMAL0,
								float3 inTangent : TANGENT,
								float3 inBinormal : BINORMAL,
								float2 inTexCoord : TEXCOORD0 )
{
	Water_VS_OUTPUT output = (Water_VS_OUTPUT)0;

	output.Position = mul(mul(mul(inPosition, World), View), Projection);
	output.TexCoord = inTexCoord * TextureScale;

	float time = fmod( Time, 100.0 );
	output.Bump0 = inTexCoord * TextureScale + time * BumpSpeed;
	output.Bump1 = inTexCoord * TextureScale * 2.0f + time * BumpSpeed * 4.0;
	output.Bump2 = inTexCoord * TextureScale * 4.0f + time * BumpSpeed * 8.0;

	float3x3 tangentSpace;
	tangentSpace[0] = BumpHeight * normalize(inTangent);
	tangentSpace[1] = BumpHeight * normalize(inBinormal);
	tangentSpace[2] = normalize(mul(inNormal, World));

	output.TanToCube[0] = mul( tangentSpace, World[0].xyz );
	output.TanToCube[1] = mul( tangentSpace, World[1].xyz );
	output.TanToCube[2] = mul( tangentSpace, World[2].xyz );

	float4 worldPos = mul( inPosition, World );
	output.View = ViewInverse[3].xyz - worldPos;

	return output;
}


float4 Water_ps_main( float2 inTexCoord : TEXCOORD0,
						float3 inTanToCube[3] : TEXCOORD1,
						float2 inBump0 : TEXCOORD4,
						float2 inBump1 : TEXCOORD5,
						float2 inBump2 : TEXCOORD6,
						float3 inView : TEXCOORD7 ) : COLOR0
{
	float4 t0 = tex2D( waterNormalSampler, inBump0 ) * 2.0f - 1.0f;
    float4 t1 = tex2D( waterNormalSampler, inBump1 ) * 2.0f - 1.0f;
    float4 t2 = tex2D( waterNormalSampler, inBump2 ) * 2.0f - 1.0f;

	float3 vN = t0.xyz + t1.xyz + t2.xyz;

	float3x3 tanToWorld;
    tanToWorld[0] = inTanToCube[0];
    tanToWorld[1] = inTanToCube[1];
    tanToWorld[2] = inTanToCube[2];

	float3 worldNormal = mul( tanToWorld, vN );
    worldNormal = normalize( worldNormal );

	inView = normalize( inView );
    float3 vR = reflect( -inView, worldNormal );

	float4 reflect = texCUBE( skyBoxSampler, vR.zyx );    
    reflect = texCUBE( skyBoxSampler, vR );
	reflect.rgb *= ( 1.0 + reflect.a * HDRMultiplier );

	float facing  = 1.0 - max( dot( inView, worldNormal ), 0 );
    float fresnel = FresnelBias + ( 1.0 - FresnelBias ) * pow( facing, FresnelPower);

	float4 waterColor = lerp( DeepColor, ShallowColor, facing );

	return float4((waterColor * WaterAmount + reflect * ReflectionColor * ReflectionAmount * fresnel).rgb, 1);
}

technique Water
{
	pass P0
	{
		VertexShader = compile vs_2_0 Water_vs_main();
		PixelShader = compile ps_2_0 Water_ps_main();
	}
}