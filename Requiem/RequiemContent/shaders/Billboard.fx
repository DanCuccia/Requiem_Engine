//===================================================================
//Basic Shader used for billboards
//Daniel Cuccia 2012
//NEIT - Bachelor Senior Project
//===================================================================

float4x4 World;
float4x4 View;
float4x4 Projection;

// 0 = use Texture's Color : 1 = use specified Color
float ColorBlendAmount = 0.5;

texture Texture : register(t0);
sampler TextureSampler : register(s0) = sampler_state 
{ 
	texture = <Texture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


//===================================================================
// Sprite Pixel Shader
//===================================================================

struct Sprite_PS_INPUT
{
	float4 Color : COLOR0;
	float2 NormalizedTextureCoordinate : TEXCOORD0;
};

float4 SpritePixelShader(Sprite_PS_INPUT input) : COLOR0
{
	float4 Color = tex2D(TextureSampler, input.NormalizedTextureCoordinate);
	if (Color.a > 0.0)
	{
		Color.rgb /= Color.a;
		Color.rgb = (ColorBlendAmount * input.Color.rgb) + ((1 - ColorBlendAmount) * Color.rgb);
		Color.a *= input.Color.a;
		Color.rgb *= Color.a;
	}
	return Color;
}


//===================================================================
// Quad Shader Structures and Methods
//===================================================================

struct Quad_VS_INPUT
{
	float4 Position				: POSITION0;
	float4 Color				: COLOR0;
};

struct Quad_VS_OUTPUT
{
	float4 Position				: POSITION0;
	float4 Color				: COLOR0;
};

Quad_VS_OUTPUT QuadVertexShader(Quad_VS_INPUT input)
{
	Quad_VS_OUTPUT Output;
	Output.Position = mul(input.Position, mul(World, mul(View, Projection)));
	input.Color.rgb *= input.Color.a;
	Output.Color = input.Color;
	return Output;
}

float4 QuadPixelShader(Quad_VS_OUTPUT input) : COLOR0
{    
	return input.Color;
}


//===================================================================
// Textured Quad Shader Structures and Methods
//===================================================================

struct TexturedQuad_VS_INPUT
{
	float4 Position						: POSITION;
	float2 TexCoord						: TEXCOORD;
	float4 Color						: COLOR;
};

struct TexturedQuad_VS_OUTPUT
{
	float4 Position						: POSITION;
	float2 TexCoord						: TEXCOORD;
	float4 Color						: COLOR;
};

TexturedQuad_VS_OUTPUT TexturedQuadVertexShader(TexturedQuad_VS_INPUT input)
{
	TexturedQuad_VS_OUTPUT Output;
	Output.Position = mul(float4(input.Position.xyz, 1), mul(World, mul(View, Projection)));
	Output.TexCoord = input.TexCoord;
	Output.Color = input.Color;
	return Output;
}

float4 TexturedQuadPixelShader(TexturedQuad_VS_OUTPUT input) : COLOR0
{
	float4 Color = tex2D(TextureSampler, input.TexCoord);
	return Color * input.Color;
}

//===================================================================
// Techniques
//===================================================================
technique Quads
{
	pass P0
	{
		//CullMode = NONE;
		VertexShader = compile vs_2_0 QuadVertexShader();
		PixelShader = compile ps_2_0 QuadPixelShader();
	}
}

technique TexturedQuads
{
	pass P0
	{
		//CullMode = NONE;
		VertexShader = compile vs_2_0 TexturedQuadVertexShader();
		PixelShader = compile ps_2_0 TexturedQuadPixelShader();
	}
}