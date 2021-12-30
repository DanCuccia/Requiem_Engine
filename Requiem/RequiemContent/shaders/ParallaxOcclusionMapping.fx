//**************************************************************//
//  Effect File exported by RenderMonkey 1.6
//**************************************************************//
// -------------------------------------------------------------//
//	Edited By Dan Cuccia 5/30/12: NEIT Senior Project
// -------------------------------------------------------------//

float4x4 World			: WORLD;
float4x4 View			: VIEW;
float4x4 Projection		: PROJECTION;
float4x4 ViewInverse	: VIEWINVERSE;
float3 CameraPosition	;

float    fBaseTextureRepeat_x = 2.0f;
float    fBaseTextureRepeat_y = 2.0f;
float    fHeightMapRange = float( 0.02 );
float4   vLightPosition = float4( -100, 100, 100, 1.00 );

float fSpecular = 1.0f;
float fShadowSoftening = float( 0.58 );
float fSpecularExponent = float( 100.00 );
float fHeightMapRange_ps = float( 0.02 );

float4 cAmbientColor = float4( 0.53, 0.53, 0.55, 1.00 );
float4 cDiffuseColor = float4( 0.40, 0.39, 0.46, 1.00 );
float4 cSpecularColor = float4( 0.96, 1.00, 0.99, 1.00 );

int nMinSamples = int( 0 );//12
int nMaxSamples = int( 0 );//50

texture DiffuseTexture;
sampler tBaseMap = sampler_state
{
   Texture = (DiffuseTexture);
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
   AddressU = Wrap;
   AddressV = Wrap;
};

texture NormalMap;
sampler tNormalMap = sampler_state
{
   Texture = (NormalMap);
   MIPFILTER = LINEAR;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
   AddressU = Wrap;
   AddressV = Wrap;
};


struct VS_INPUT
{
   float4 positionWS  : POSITION;
   float2 texCoord    : TEXCOORD0;
   float3 vNormalWS   : NORMAL;
   float3 vBinormalWS : BINORMAL;
   float3 vTangentWS  : TANGENT;
};

struct VS_OUTPUT
{
   float4 position          : POSITION;
   float2 texCoord          : TEXCOORD0;
   float3 vLightTS          : TEXCOORD1;
   float3 vViewTS           : TEXCOORD2;
   float2 vParallaxOffsetTS : TEXCOORD3;
   float3 vNormalWS         : TEXCOORD4;
   float3 vViewWS           : TEXCOORD5;
};

VS_OUTPUT VS_Parallax_Occlusion_Mapping_main( VS_INPUT i )
{
   VS_OUTPUT Out = (VS_OUTPUT) 0;
   Out.position = mul(mul(mul(i.positionWS, World), View), Projection);
   Out.texCoord = float2(i.texCoord.x * fBaseTextureRepeat_x, i.texCoord.y * fBaseTextureRepeat_y);
   Out.vNormalWS = i.vNormalWS;

   float3 vNormalWS   = mul( i.vNormalWS,   (float3x3) World );
   float3 vBinormalWS = mul( i.vBinormalWS, (float3x3) World );
   float3 vTangentWS  = mul( i.vTangentWS,  (float3x3) World );

   float3 vPositionWS = mul(i.positionWS, World);
   Out.vViewWS = CameraPosition - vPositionWS;

   float3 vLightWS = vLightPosition.xyz - vPositionWS;
   vLightWS.z = -vLightWS.z;
   float3x3 mWorldToTangent = float3x3(vTangentWS, vBinormalWS, vNormalWS );
   Out.vLightTS = mul( mWorldToTangent, vLightWS );
   Out.vViewTS  = mul( mWorldToTangent, Out.vViewWS );
   float2 vParallaxDirection = normalize( Out.vViewTS.xy );
   
   float fLength = length( Out.vViewTS );
   float fParallaxLength = sqrt( fLength * fLength - Out.vViewTS.z * Out.vViewTS.z ) / Out.vViewTS.z;
   
   Out.vParallaxOffsetTS = vParallaxDirection * fParallaxLength;
   Out.vParallaxOffsetTS *= fHeightMapRange;
   return Out;
}

// -------------------------------------------------------------------------------------------------
//	PIXEL SHADING
// -------------------------------------------------------------------------------------------------

struct PS_INPUT
{
   float2 texCoord          : TEXCOORD0;
   float3 vLightTS          : TEXCOORD1_centroid;   // light vector in tangent space, denormalized
   float3 vViewTS           : TEXCOORD2_centroid;   // view vector in tangent space, denormalized
   float2 vParallaxOffsetTS : TEXCOORD3_centroid;   // Parallax offset vector in tangent space
   float3 vNormalWS         : TEXCOORD4_centroid;   // Normal vector in world space
   float3 vViewWS           : TEXCOORD5_centroid;   // View vector in world space
};

float4 ComputeIllumination( float2 texCoord, float3 vLightTS, float3 vViewTS, float fOcclusionShadow )
{
   float3 vNormalTS = normalize( tex2D( tNormalMap, texCoord ) * 2 - 1 );
   float4 cBaseColor = tex2D( tBaseMap, texCoord );
   float4 cDiffuse = saturate( dot( vNormalTS, vLightTS )) * cDiffuseColor;
   float3 vReflectionTS = normalize( 2 * dot( vViewTS, vNormalTS ) * vNormalTS - vViewTS );
   float fRdotL = dot( vReflectionTS, vLightTS );
   float4 cSpecular = saturate( pow( fRdotL, fSpecularExponent )) * cSpecularColor;
   float4 cFinalColor = (( cAmbientColor + cDiffuse ) * cBaseColor + (cSpecular*fSpecular) ) * fOcclusionShadow; 
   return cFinalColor;
} 

float4 PS_Parallax_Occlusion_Mapping_main( PS_INPUT i ) : COLOR
{
   float3 vViewTS   = normalize( i.vViewTS  );
   float3 vViewWS   = normalize( i.vViewWS  );
   float3 vLightTS  = normalize( i.vLightTS );
   float3 vNormalWS = normalize( i.vNormalWS );
     
   float4 cResultColor = float4( 0, 0, 0, 1 );
   float2 dx = ddx( i.texCoord );
   float2 dy = ddy( i.texCoord );

   int nNumSteps = (int) lerp( nMaxSamples, nMinSamples, dot( vViewWS, vNormalWS ) );

   float fCurrHeight = 0.0;
   float fStepSize   = 1.0 / (float) nNumSteps;
   float fPrevHeight = 1.0;
   float fNextHeight = 0.0;

   int    nStepIndex = 0;
   bool   bCondition = true;

   float2 vTexOffsetPerStep = fStepSize * i.vParallaxOffsetTS;
   float2 vTexCurrentOffset = i.texCoord;
   float  fCurrentBound     = 1.0;
   float  fParallaxAmount   = 0.0;

   float2 pt1 = 0;
   float2 pt2 = 0;
   float2 texOffset2 = 0;

   while ( nStepIndex < nNumSteps ) 
   {
      vTexCurrentOffset -= vTexOffsetPerStep;
      fCurrHeight = tex2Dgrad( tNormalMap, vTexCurrentOffset, dx, dy ).a;
      fCurrentBound -= fStepSize;
      if ( fCurrHeight > fCurrentBound ) 
      {     
         pt1 = float2( fCurrentBound, fCurrHeight );
         pt2 = float2( fCurrentBound + fStepSize, fPrevHeight );
         texOffset2 = vTexCurrentOffset - vTexOffsetPerStep;
         nStepIndex = nNumSteps + 1;
      }
      else
      {
         nStepIndex++;
         fPrevHeight = fCurrHeight;
      }
   }

   float fDelta2 = pt2.x - pt2.y;
   float fDelta1 = pt1.x - pt1.y;
   fParallaxAmount = (pt1.x * fDelta2 - pt2.x * fDelta1 ) / ( fDelta2 - fDelta1 );
   
   float2 vParallaxOffset = i.vParallaxOffsetTS * (1 - fParallaxAmount );
   float2 texSample = i.texCoord - vParallaxOffset;
   float2 vLightRayTS = vLightTS.xy * fHeightMapRange_ps;

   float sh0 =  tex2Dgrad( tNormalMap, texSample, dx, dy ).a;
   float shA = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.88, dx, dy ).a - sh0 - 0.88 ) *  1 * fShadowSoftening;
   float sh9 = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.77, dx, dy ).a - sh0 - 0.77 ) *  2 * fShadowSoftening;
   //float sh8 = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.66, dx, dy ).a - sh0 - 0.66 ) *  4 * fShadowSoftening;
   //float sh7 = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.55, dx, dy ).a - sh0 - 0.55 ) *  6 * fShadowSoftening;
   //float sh6 = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.44, dx, dy ).a - sh0 - 0.44 ) *  8 * fShadowSoftening;
   //float sh5 = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.33, dx, dy ).a - sh0 - 0.33 ) * 10 * fShadowSoftening;
   //float sh4 = (tex2Dgrad( tNormalMap, texSample + vLightRayTS * 0.22, dx, dy ).a - sh0 - 0.22 ) * 12 * fShadowSoftening;
   
   float fOcclusionShadow = 1 - max(shA, sh9);//max( max( max( max( max( max( shA, sh9 ), sh8 ), sh7 ), sh6 ), sh5 ), sh4 );
   fOcclusionShadow = fOcclusionShadow * 0.6 + 0.4;    
   return float4(ComputeIllumination( texSample, vLightTS, vViewTS, fOcclusionShadow ).rgb, 1);
}


//--------------------------------------------------------------//
// Techniques
//--------------------------------------------------------------//
technique Parallax_Occlusion_Mapping
{
   pass P0
   {
      VertexShader = compile vs_3_0 VS_Parallax_Occlusion_Mapping_main();
      PixelShader = compile ps_3_0 PS_Parallax_Occlusion_Mapping_main();
   }
}
