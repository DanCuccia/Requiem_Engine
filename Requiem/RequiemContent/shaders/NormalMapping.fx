//--------------------------------------------------------------------------------------
//Light Array and Material Indices
//--------------------------------------------------------------------------------------
#define LIGHT1_COLOR			0
#define LIGHT1_DIRECTION		1
#define LIGHT2_COLOR			2
#define LIGHT2_DIRECTION		3
#define LIGHT3_COLOR			4
#define LIGHT3_DIRECTION		5
#define LIGHT4_COLOR			6
#define LIGHT4_DIRECTION		7

#define MATERIAL_AMBIENT		0
#define MATERIAL_DIFFUSE		1
#define MATERIAL_SPECULAR		2
#define MATERIAL_SHINE			3

float4			LightArray[8] =
{
//	   color                 direction.xyz      intensity.w
	float4(1,1,1,1), float4(normalize(float3(1,1,-1)),1),
	float4(1,1,1,1), float4(normalize(float3(1,1,-1)),1),
	float4(1,1,1,1), float4(normalize(float3(1,1,-1)),1),
	float4(1,1,1,1), float4(normalize(float3(1,1,-1)),1)
};

float Material[4] = 
{
// ambient diffuse specular shine
	0.1f,   1.0f,    0.5f,   64
};

float4x4		World				: WORLD;
float4x4		View				: VIEW;
float4x4		Projection			: PROJECTION;
float4x4		ViewInverse			: VIEWINVERSE;

float4			CameraPosition		: VIEWPOSITION;
float4			AmbientColor		= float4(1, 1, 1, 1);

float4x4		FinalTransforms		[56];
int				NumVertInfluences	= 1;

texture Diffuse	: register(t0);
sampler diffuseSampler : register(s0) = sampler_state
{
    Texture = (Diffuse);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};

texture Normals : register(t1);
sampler normalsSampler : register(s1) = sampler_state
{
	Texture = (Normals);
	MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};


struct VERTEX_VS_OUTPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Eye : TEXCOORD1;
	float3 Tangent : TEXCOORD2;
	float3 Normal : TEXCOORD3;
	float3 BiNormal : TEXCOORD4;
};

//-------------------------------------------------------------
//	Normal Mapping Vertex Shader for non-animating objects
//-------------------------------------------------------------
VERTEX_VS_OUTPUT NormalMap_vs_main( float4 inPosition : POSITION0,
										float2 inTexcoord : TEXCOORD0,
										float3 inNormal : NORMAL0,
										float3 inTangent : TANGENT0,
										float3 inBinormal : BINORMAL0 )
{
	VERTEX_VS_OUTPUT output;
	output.Position = mul(mul(mul(inPosition, World), View), Projection);
	output.TexCoord = inTexcoord;
	output.Eye = normalize(CameraPosition-mul(World, inPosition));
	output.Tangent = inTangent;
	output.Normal = inNormal;
	output.BiNormal = inBinormal;
	return output;
}


// --------------------------------------------------------
//		Normal Map Vertex Shader for animating objects
// --------------------------------------------------------
VERTEX_VS_OUTPUT AnimNormalMap_vs_main( float4 inPosition	: POSITION0,
										float3 inNormal		: NORMAL0,
										float3 inTangent	: TANGENT0,
										float3 inBinormal	: BINORMAL0,
										float2 inTexCoord	: TEXCOORD0,
										float4 inWeights	: BLENDWEIGHT0,
										int4  inBoneIndices : BLENDINDICES0 )
{
	VERTEX_VS_OUTPUT output = (VERTEX_VS_OUTPUT)0;
	float lastWeight = 0.0f;
	float4 pos = float4(0,0,0,1);
	float3 normal = float3(0,0,0);
	float3 tangent = float3(0,0,0);
	float3 binormal = float3(0,0,0);
	int num = NumVertInfluences -1;
	for(int i=0; i < num; ++i)
	{
		lastWeight += inWeights[i];
		pos + inWeights[i] * mul(inPosition, FinalTransforms[inBoneIndices[i]]);
		normal += inWeights[i] * mul(inNormal, FinalTransforms[inBoneIndices[i]]);
		tangent += inWeights[i] * mul(inTangent, FinalTransforms[inBoneIndices[i]]);
		binormal += inWeights[i] * mul(inBinormal, FinalTransforms[inBoneIndices[i]]);
	}
	lastWeight = 1.0f - lastWeight;

	pos += lastWeight * mul(inPosition, FinalTransforms[inBoneIndices[num]]);
	normal += lastWeight * mul(inNormal, FinalTransforms[inBoneIndices[num]]);
	tangent += inWeights * mul(inTangent, FinalTransforms[inBoneIndices[num]]);
	binormal += inWeights * mul(inBinormal, FinalTransforms[inBoneIndices[num]]);

	output.Eye = normalize(CameraPosition - mul(World, pos));
	output.Tangent = tangent;
	output.Normal = normal;
	output.BiNormal = binormal;

	pos.w = 1.0f;
	output.Position = mul(mul(mul(pos, World), View), Projection);
	output.TexCoord = inTexCoord;

	return output;
}


struct PointLightInput
{
	float3 normal;
	float3 eye;
	float3 lightDirection;
	float4 lightColor;
	float4 diffuseColor;
};

float4 BlinnPhongPointLight(PointLightInput input)
{
	float3 reflection = -normalize(2.0f * input.normal * dot( input.normal, input.lightDirection) - input.lightDirection);
	float dotV = max( dot( reflection, input.eye ), 0.0f);
	float4 specular = (input.lightColor * pow(dotV, Material[MATERIAL_SHINE])) * Material[MATERIAL_SPECULAR];
	float4 diffuse = float4(input.diffuseColor.rgb * max(0.0f, dot( input.normal, -input.lightDirection)), 1.0f) * Material[MATERIAL_DIFFUSE];
	return float4( diffuse.rgb + specular.rgb, 1.0f );
}

//-------------------------------------------------------------
//		NormalMap Pixel Shader shared for both (non)animating objects
//-------------------------------------------------------------
float4 NormalMap_ps_main( VERTEX_VS_OUTPUT input ) : COLOR
{
	PointLightInput light = (PointLightInput)0;

	light.normal = normalize(tex2D(normalsSampler, input.TexCoord) * 2 - 1);
	light.diffuseColor = tex2D(diffuseSampler, input.TexCoord);
	light.eye = input.Eye;

	float3x3 tangentSpace;
	tangentSpace[0] = input.Tangent;
	tangentSpace[1] = input.BiNormal;
	tangentSpace[2] = input.Normal;
	
	light.lightDirection = normalize(mul(tangentSpace, LightArray[LIGHT1_DIRECTION].xyz + input.Eye)).xyz;
	light.lightColor = LightArray[LIGHT1_COLOR];
	float4 output = BlinnPhongPointLight(light) * LightArray[LIGHT1_DIRECTION].w;

	light.lightDirection = normalize(mul(tangentSpace, LightArray[LIGHT2_DIRECTION].xyz + input.Eye)).xyz;
	light.lightColor = LightArray[LIGHT2_COLOR];
	output += BlinnPhongPointLight(light) * LightArray[LIGHT2_DIRECTION].w;

	light.lightDirection = normalize(mul(tangentSpace, LightArray[LIGHT3_DIRECTION].xyz + input.Eye)).xyz;
	light.lightColor = LightArray[LIGHT3_COLOR];
	output += BlinnPhongPointLight(light) * LightArray[LIGHT3_DIRECTION].w;

	light.lightDirection = normalize(mul(tangentSpace, LightArray[LIGHT4_DIRECTION].xyz + input.Eye)).xyz;
	light.lightColor = LightArray[LIGHT4_COLOR];
	output += BlinnPhongPointLight(light) * LightArray[LIGHT4_DIRECTION].w;

	return output + (light.diffuseColor * Material[MATERIAL_AMBIENT]);
}

technique NormalMap
{
	pass P0
	{
		VertexShader = compile vs_3_0 NormalMap_vs_main();
		PixelShader = compile ps_3_0 NormalMap_ps_main();
	}
}

technique AnimatedNormalMap
{
	pass P0
	{
		VertexShader = compile vs_3_0 AnimNormalMap_vs_main();
		PixelShader = compile ps_3_0 NormalMap_ps_main();
	}
}