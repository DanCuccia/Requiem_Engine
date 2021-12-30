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

Texture2D DiffuseTexture;
SamplerState diffuseSampler
{
	Texture = (DiffuseTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4					AmbientColor		= float4(1,1,1,1);
float3					CameraPosition;
float4					ModelColor			= float4(1,1,1,1);

float4x4				FinalTransforms		[59];
int						NumVertInfluences	= 1;

//--------------------------------------------------------------------------------------
//VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VS_INPUT
{
	float4 position : POSITION;
	float2 texCoord : TEXCOORD; 
	float3 normal	: NORMAL;
};

struct VS_INPUT_ANIMATED
{
	float4 position : POSITION;
	float2 texCoord : TEXCOORD;
	float3 normal   : NORMAL;
	float4 weight   : BLENDWEIGHT;
	int4   indices  : BLENDINDICES;
};

struct PS_INPUT
{
	float4 position : POSITION;
	float2 texCoord : TEXCOORD;	
	float3 normal	: TEXCOORD1;
	float3 eye		: TEXCOORD2;
};

//--------------------------------------------------------------------------------------
// Vertex Shaders
//--------------------------------------------------------------------------------------
PS_INPUT VS_BlinnPhong_main( VS_INPUT input )
{
	PS_INPUT output = (PS_INPUT)0;

	input.position = mul(input.position, World);
	output.position = mul(mul(input.position, View), Projection);
	output.texCoord = input.texCoord;
	output.eye = normalize( CameraPosition - input.position.xyz );
	output.normal = mul(input.normal, (float3x3)World);
    
	return output;  
}

PS_INPUT VS_BlinnPhong_animated_main( VS_INPUT_ANIMATED input )
{
	PS_INPUT output;

	float lastWeight = 0.0f;
	float4 pos = float4(0,0,0,1);
	float3 normal = float3(0,0,0);
	int num = NumVertInfluences -1;

	for(int i=0; i < num; ++i)
	{
		lastWeight += input.weight[i];
		pos + input.weight[i] * mul(input.position, FinalTransforms[input.indices[i]]);
		normal += input.weight[i] * mul(input.normal, FinalTransforms[input.indices[i]]);
	}
	lastWeight = 1.0f - lastWeight;

	pos += lastWeight * mul(input.position, FinalTransforms[input.indices[num]]);
	normal += lastWeight * mul(input.normal, FinalTransforms[input.indices[num]]);
	pos.w = 1.0f;

	output.position = mul(mul(mul(pos, World), View), Projection);
	output.eye = normalize(CameraPosition - mul(pos, World));
	output.normal = normalize(normal);
	output.texCoord = input.texCoord;

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
	float dotV = max( dot( -normalize(2.0f * input.normal * dot( input.normal, input.lightDirection) - input.lightDirection), input.eye ), 0.0f);
	float4 specular = (input.lightColor * pow(dotV, Material[MATERIAL_SHINE])) * Material[MATERIAL_SPECULAR];
	float4 diffuse = float4(input.lightColor.rgb * max(0.0f, dot( input.normal, -input.lightDirection)), 1.0f) * Material[MATERIAL_DIFFUSE];
	return float4( diffuse.rgb + specular.rgb, 1.0f );
}

//--------------------------------------------------------------------------------------
// Shared Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS_BlinnPhong_main( uniform bool textured, PS_INPUT input ) : COLOR
{
	PointLightInput light = (PointLightInput)0;
	light.normal = normalize(input.normal);
	light.eye = input.eye;
	light.diffuseColor = textured ? tex2D(diffuseSampler, input.texCoord) : ModelColor;

	light.lightDirection = normalize(LightArray[LIGHT1_DIRECTION].xyz);
	light.lightColor = LightArray[LIGHT1_COLOR];
	float4 output = BlinnPhongPointLight(light) * LightArray[LIGHT1_DIRECTION].w;
	
	light.lightDirection = normalize(LightArray[LIGHT2_DIRECTION].xyz);
	light.lightColor = LightArray[LIGHT2_COLOR];
	output += BlinnPhongPointLight(light) * LightArray[LIGHT2_DIRECTION].w;

	light.lightDirection = normalize(LightArray[LIGHT3_DIRECTION].xyz);
	light.lightColor = LightArray[LIGHT3_COLOR];
	output += BlinnPhongPointLight(light) * LightArray[LIGHT3_DIRECTION].w;

	light.lightDirection = normalize(LightArray[LIGHT4_DIRECTION].xyz);
	light.lightColor = LightArray[LIGHT4_COLOR];
	output += BlinnPhongPointLight(light) * LightArray[LIGHT4_DIRECTION].w;

	return float4(output.rgb + (light.diffuseColor * Material[MATERIAL_AMBIENT]), ModelColor.a);
}


//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique BlinnPhong_Color
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_BlinnPhong_main();
        PixelShader = compile ps_3_0 PS_BlinnPhong_main(false);
    }    
}

technique BlinnPhong_AnimatedColor
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_BlinnPhong_animated_main();
		PixelShader = compile ps_3_0 PS_BlinnPhong_main(false);
	}
}

technique BlinnPhong_Texture
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_BlinnPhong_main();
        PixelShader = compile ps_3_0 PS_BlinnPhong_main(true);
    }    
}

technique BlinnPhong_AnimatedTexture
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_BlinnPhong_animated_main();
		PixelShader = compile ps_3_0 PS_BlinnPhong_main(true);
	}
}