float4x4 World : WORLD;
float4x4 View : VIEW;
float4x4 Projection : PROJECTION;

struct VertexInput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};
struct VertexOutput
{
	float4 Position : SV_POSITION;
	float4 Color : TEXCOORD0;
};

VertexOutput RenderSceneVS(VertexInput IN)
{
	VertexOutput OUT;

	float4x4 worldViewProj = mul(mul(World, View), Projection);
	OUT.Position = mul(IN.Position, worldViewProj);
	OUT.Color = IN.Color;

	return OUT;
}
float4 RenderScenePS(VertexOutput IN) : SV_TARGET
{
	return IN.Color;
}

technique RenderScene
{
	pass p0
	{
		VertexShader = compile vs_2_0 RenderSceneVS();
		PixelShader = compile ps_2_0 RenderScenePS();
	}
}