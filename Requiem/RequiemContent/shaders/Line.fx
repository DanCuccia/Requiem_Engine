float4x4		World				: WORLD;
float4x4		View				: VIEW;
float4x4		Projection			: PROJECTION;

struct line_VS_OUTPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

line_VS_OUTPUT line3D_vs_main(	float3 inPos : POSITION0,
								float4 inColor : COLOR0 )
{
	line_VS_OUTPUT output;
	output.Position = mul(mul(mul(float4(inPos,1), World), View), Projection);
	output.Color = inColor;
	return output;
}

float4 line3D_ps_main( float4 inColor : COLOR0 ) : COLOR0
{
	return inColor;
}

technique Line
{
	pass P0
	{
		VertexShader = compile vs_2_0 line3D_vs_main();
		PixelShader = compile ps_2_0 line3D_ps_main();
	}
}