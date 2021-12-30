//**************************************************************//
//  Effect File exported by RenderMonkey 1.6
//				-modified by Daniel Cuccia 9/16/12
//**************************************************************//

float4x4 World : World;
float4x4 view_proj_matrix	: ViewProjection;
float time_0_X : Time0_X;

float skyZBias = -720.0f;
float scale = 0.00f;
float cloudSpeed =  0.1f;
float noiseSpeed = 0.03f;
float noiseScale = 6.2f;
float noiseBias = -3.0f;
float skyScale = 996.0f;
float4 cloudColor = float4( 0.2f, 0.4f, 1, 1 );

texture Noise_Tex;
sampler NoiseSampler = sampler_state
{
   Texture = (Noise_Tex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   ADDRESSW = WRAP;
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

//--------------------------------------------------------------//
// Shading
//--------------------------------------------------------------//
struct VS_OUTPUT
{
   float4 Pos: POSITION;
   float3 texCoord: TEXCOORD0;
};

VS_OUTPUT vs_main(float4 Pos: POSITION)
{
   VS_OUTPUT Out;
   Pos.xyz = skyScale * normalize(Pos.xyz);
   Pos.y += skyZBias;
   Out.Pos = mul(view_proj_matrix, Pos);
   Out.texCoord = Pos.xyz;
   return Out;
}

float4 ps_main(float3 texCoord: TEXCOORD0) : COLOR 
{
   texCoord *= scale;
   texCoord.xy += cloudSpeed * time_0_X;
   texCoord.z  += noiseSpeed * time_0_X;
   float noisy = tex3D(NoiseSampler, texCoord).r;
   float lrp = noiseScale * noisy + noiseBias;
   return float4(cloudColor.xyz, lrp);
}


//--------------------------------------------------------------//
// Technique
//--------------------------------------------------------------//
technique Clouds
{
   pass P0
   {
      ZWRITEENABLE = TRUE;
      SRCBLEND = SRCALPHA;
      DESTBLEND = INVSRCALPHA;
      CULLMODE = CW;
      ALPHABLENDENABLE = TRUE;

      VertexShader = compile vs_2_0 vs_main();
      PixelShader = compile ps_2_0 ps_main();
   }

}