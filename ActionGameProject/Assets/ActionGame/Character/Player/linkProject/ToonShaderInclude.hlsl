#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 texcoord : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
    float3 positionVS : TEXCOORD1;
    float4 positionNDC : TEXCOORD2;
    float3 normalWS : TEXCOORD3;
    //float3 bitangentWS : TEXCOORD3;
#ifdef _UseFringeShadow  
    float4 scrPos : TEXCOORD4;
#endif
    float4 uv : TEXCOORD5;
};

CBUFFER_START(UnityPerMaterial)

float _IsFace;

//BaseColor
float4 _BaseMap_ST;
half4 _BaseColor;

//AlphaClip
float _CutOff;

//Outline
float _OutlineWidth;
half4 _OutlineColor;
float _OutlineZOffsetAmount;

//Lighting
half _CelShadeGradient;
half _CelShadeArea;

//Shadow
half _ReceiveShadowMappingAmount;
float _ReceiveShadowMappingOffset;
half3 _ShadowPartColor;
float _FringeShadowOffset;
float _FringeShadowThreshold;

//RimLight
half4 _RimColorFront;
half _RimColorBrightness;
half4 _RimColorBack;
float _RimWidth;
float _Threshold;
float _FresnelPow;
// (basicway)
// float _RimPower;
// float _RimScale;

//StrandSpecular
float _SpecularExponent;
float _SpecularScale;

//Dissolve
half4 _DissolveColor;
float4 _DissolveMap_ST;
float _DissolveAmount;
float _DissolveWidth;
CBUFFER_END

//a special uniform for applyShadowBiasFixToHClipPos() only, it is not a per material uniform, 
//so it is fine to write it outside our UnityPerMaterial CBUFFER
float _LightDirection;

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
TEXTURE2D(_HairSolidColor);
SAMPLER(sampler_HairSolidColor);
TEXTURE2D(_DissolveMap);
SAMPLER(sampler_DissolveMap);

///////////////////////////
//vertex shader part
///////////////////////////

float GetCameraFOV()
{
    //https://answers.unity.com/questions/770838/how-can-i-extract-the-fov-information-from-the-pro.html
    float t = unity_CameraProjection._m11;
    float Rad2Deg = 180 / 3.1415;
    float fov = atan(1.0f / t) * 2.0 * Rad2Deg;
    return fov;
}

float OutlineWidthWithDistanceFix(float OutlineWidth, float positionVS_Z)
{
    float OutlineFix;    
    if(unity_OrthoParams.w == 0)
    {
        //For Perspective Camera
        OutlineFix = abs(positionVS_Z);//隨攝影機距離保持寬度(依遠近乘大到小的值)
        
        OutlineFix = saturate(OutlineFix);

        OutlineFix = OutlineFix * GetCameraFOV();//隨FOV大小保持寬度        
    }
    return OutlineWidth * OutlineFix * 0.00005;//乘上一個小數維持結果
}

float4 GetNewClipPosWithZOffset(float4 originalPositionCS, float viewSpaceZOffsetAmount)
{
    if(unity_OrthoParams.w == 0)
    {
        //For Perspective Camera
        float2 ProjM_ZRow_ZW = UNITY_MATRIX_P[2].zw;
        float modifiedPositionVS_Z = -originalPositionCS.w + -viewSpaceZOffsetAmount;
        float modifiedPositionCS_Z = modifiedPositionVS_Z * ProjM_ZRow_ZW[0] + ProjM_ZRow_ZW[1];
        originalPositionCS.z = modifiedPositionCS_Z * originalPositionCS.w / (-modifiedPositionVS_Z);//乘上原本Z值和修改過後Z值的比例 

        return originalPositionCS;               
    }
    else
    {
        //For Orthographic Camera        
        return originalPositionCS;
    }
}

Varyings vert(Attributes input)
{
    Varyings output;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    float3 positionWS = vertexInput.positionWS;

#ifdef OutlinePass
    positionWS = vertexInput.positionWS + vertexNormalInput.normalWS * OutlineWidthWithDistanceFix(_OutlineWidth, vertexInput.positionVS.z);
#endif

    output.positionWS = positionWS;
    output.positionNDC = vertexInput.positionNDC;
    output.normalWS = vertexNormalInput.normalWS;
    float3 tangentWS = vertexNormalInput.tangentWS;
    //output.bitangentWS = vertexNormalInput.bitangentWS;
    output.positionVS = TransformWorldToView(output.positionWS);

    output.uv.xy = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.uv.zw = TRANSFORM_TEX(input.texcoord, _DissolveMap);

    output.positionCS = TransformWorldToHClip(output.positionWS);   

//瀏海陰影
#ifdef _UseFringeShadow     
    output.scrPos = ComputeScreenPos(vertexInput.positionCS);
#endif    

#ifdef OutlinePass
    output.positionCS = GetNewClipPosWithZOffset(output.positionCS, _OutlineZOffsetAmount + 0.03 * _IsFace);
#endif

    // ShadowCaster pass needs special process to positionCS, else shadow artifact will appear
#ifdef ApplyShadowBiasFix
    //in URP/Shaders/ShadowCasterPass.hlsl
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(output.positionWS, output.normalWS, _LightDirection));
    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #else
        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #endif
        output.positionCS = positionCS;
#endif

    return output;
}

//////////////////////////////////////
//fragment shader part1 - DataPrepare
//////////////////////////////////////

half3 ShadeDirectionalLight(Light inputlight, half3 normalWS, half occlusion, bool isAdditionalLight)
{
    half3 N = normalWS;
    half3 L = inputlight.direction;
    half NdotL = dot(N, L);
    half Occlusion = occlusion;
    half distanceAttenuation = min(4, inputlight.distanceAttenuation);

    half litOrShadowArea = smoothstep(_CelShadeArea - _CelShadeGradient, _CelShadeArea + _CelShadeGradient, NdotL);
    //occlusion
    litOrShadowArea *= occlusion;

    //face ignore celshade since it is usually very ugly using NoL method
    litOrShadowArea = _IsFace? lerp(0.5, 1, litOrShadowArea) : litOrShadowArea;

    //shadow map
    litOrShadowArea *= lerp(1, inputlight.shadowAttenuation, _ReceiveShadowMappingAmount);
    
    half3 ShadowAreaColor = lerp(_ShadowPartColor, 1, litOrShadowArea);

    half3 lightAttenuationColor = ShadowAreaColor * distanceAttenuation;

    //AdditionalLight乘上0.25防止過亮
    return saturate(inputlight.color) * lightAttenuationColor * (isAdditionalLight? 0.25 : 1);    
}

half3 ShadeIndirectionalLight(half occlusion, half3 normalWS)
{
    //不使用normalWS而只用0是因為只需要一點環境光，用normalWS會太有立體感
    half3 averageSH = SampleSH(0);

    //averageSH = max(_IndirectLightMinColor,averageSH);

    half indirectOcclusion = lerp(1, occlusion, 0.5);

    return averageSH * indirectOcclusion;    
}

half3 CompositeAllLights(half3 indirectLightResult, half3 mainLightResult, half3 additionalLightsResult, half3 albedo)
{
    half3 LightResult = max(indirectLightResult, mainLightResult + additionalLightsResult);
    return albedo * LightResult;
}

//basic way - FresnelRim
// half CaculateRimLight(half3 normalWS, half3 viewDirectionWS, float _RimScale, float _RimPower)
// {
//     half FresnelRim = 1 - saturate(pow(abs(dot(viewDirectionWS, normalWS) + _RimScale), _RimPower));

//     return FresnelRim;
// }

//Screen Space Constant Width Rim Light
float CaculateRimLight(half3 normalWS, float3 positionVS, float4 positionNDC, half3 viewDirectionWS, float _RimWidth, float _Threshold, float _FresnelMask )
{
    float3 normalVS = TransformWorldToViewDir(normalWS, true);

    float3 samplePositionVS = float3(positionVS.xy + normalVS.xy * _RimWidth, positionVS.z);
    float4 samplePositionCS = TransformWViewToHClip(samplePositionVS);
    //獲取採樣點的ViewPort座標
    float4 samplePositionVP = ComputeScreenPos(samplePositionCS) / samplePositionCS.w;

    //獲取偏移過後的深度
        //跟下一行一樣float offsetDepth = SampleSceneDepth(samplePositionVP);
    float offsetDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, samplePositionVP.xy).r; 
    float linearEyeOffsetDepth = LinearEyeDepth(offsetDepth, _ZBufferParams);

    //獲取原本深度
    //float linearEyeOriginalDepth = positionNDC.w;
    float OriginalDepth = positionNDC.z / positionNDC.w;    
    float linearEyeOriginalDepth = LinearEyeDepth(OriginalDepth, _ZBufferParams); 

    //偏移後的深度減去原本深度取差值獲得邊緣部分
    float depthDifference = saturate(linearEyeOffsetDepth - linearEyeOriginalDepth);
    float rimIntensity = step(_Threshold, depthDifference);
    
    //配合菲涅爾使用
    float rimRatio = 1 - saturate(dot(viewDirectionWS, normalWS));
    rimRatio = pow(rimRatio, _FresnelPow);
    rimIntensity = lerp(0, rimIntensity, rimRatio);
    
    return rimIntensity;
}

float CaculateFringeShadow(float4 positionCS, float3 mainLightDirection, float2 scrPos, float _FringeShadowOffset )
{
    float OriginalDepth = positionCS.z / positionCS.w;
    float linearEyeOriginalDepth = LinearEyeDepth(OriginalDepth, _ZBufferParams);

    float3 LightDirVS = normalize(TransformWorldToViewDir(mainLightDirection)) * (1 / (min(positionCS.w, 1))) * min(1, 5 / linearEyeOriginalDepth);
    float2 samplerpoint = scrPos + _FringeShadowOffset * LightDirVS.xy;
    float hairDepth = SAMPLE_TEXTURE2D(_HairSolidColor, sampler_HairSolidColor, samplerpoint).g;
    hairDepth = LinearEyeDepth(hairDepth, _ZBufferParams); 

    //0.01 is bias
    float depthContrast = linearEyeOriginalDepth > hairDepth - _FringeShadowThreshold ? 0: 1;
    return depthContrast;
}

//各項異性高光
// float StrandSpecular(float3 bitangentWS, float3 viewDirectionWS, float3 LightDir, float _SpecularExponent, float _SpecularScale)
// {
// 	float3 H = SafeNormalize(LightDir + viewDirectionWS);
// 	float dotTH = dot(bitangentWS, H);
// 	float sinTH = sqrt(1.0 - dotTH * dotTH);
// 	float dirAtten = smoothstep(-1.0, 0.0, dotTH);
// 	return dirAtten * pow(sinTH, _SpecularExponent) * _SpecularScale;
// }

///////////////////////////////////////////////////////////
//fragment shader part2 - calculate lighting & final color
///////////////////////////////////////////////////////////

half4 frag(Varyings input) : SV_Target
{
    //SurfaceData
    half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
    half3 albedo = baseColor.rgb;
    half alpha = baseColor.a;
    half occlusion = 1;
#if _UseAlphaClip
    clip(alpha - _CutOff);
#endif 

    //LightingCalculateData
    float3 positionWS = input.positionWS;
    float3 positionVS = input.positionVS;
    float4 positionCS = input.positionCS;
    float4 positionNDC = input.positionNDC;
    //float3 bitangentWS = input.bitangentWS;
#ifdef _UseFringeShadow
    float2 scrPos = input.scrPos.xy / input.scrPos.w;
#endif
    half3 normalWS = normalize(input.normalWS);
    half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

    //Indirect Light
    half3 indirectLightResult = ShadeIndirectionalLight(occlusion, normalWS);
    
    //MainLight
    Light mainLight = GetMainLight();
    half3 shadowTestPosWS = positionWS + mainLight.direction * (_ReceiveShadowMappingOffset + _IsFace);

    //Dissolve
    half3 dissolveMap = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, input.uv.zw).rgb;  
    clip(step(_DissolveAmount, dissolveMap.b) - 0.01);
    float edge = step(saturate(_DissolveAmount + _DissolveWidth), dissolveMap.b);

//瀏海投影
#ifdef _UseFringeShadow  
    float FringeShadow = CaculateFringeShadow(positionCS, mainLight.direction, scrPos, _FringeShadowOffset);
#endif

#ifdef _MAIN_LIGHT_SHADOWS    
    float4 shadowCoord = TransformWorldToShadowCoord(shadowTestPosWS);
    mainLight.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
#endif    
    half3 mainLightResult = ShadeDirectionalLight(mainLight, normalWS, occlusion, false);
#ifdef _UseFringeShadow      
    mainLightResult = lerp(_ShadowPartColor , mainLightResult, FringeShadow);
#endif

    //AdditionalLight  
    half3 additionalLightsResult = 0;
#ifdef _ADDITIONAL_LIGHTS
    int additionalLightsCount = GetAdditionalLightsCount();
    for (int i = 0; i < additionalLightsCount; ++i)
    {
        //下兩行等於Light additionalLight = GetAdditionalLight(i, positionWS);
        int perObjectLightIndex = GetPerObjectLightIndex(i);
        Light additionalLight = GetAdditionalPerObjectLight(perObjectLightIndex, positionWS);        
        additionalLight.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, shadowTestPosWS);
        
        additionalLightsResult += ShadeDirectionalLight(additionalLight, normalWS, occlusion, true);
    }
#endif

    //各項異性高光
    //float Specular = StrandSpecular(bitangentWS, viewDirectionWS, mainLight.direction, _SpecularExponent, _SpecularScale); 

    half3 FinalColorResult = CompositeAllLights(indirectLightResult, mainLightResult, additionalLightsResult, albedo);         

    #ifdef OutlinePass
        return half4(FinalColorResult, alpha) * _OutlineColor;
    #endif
    
    //rim light
    #ifdef _UseRim        
        half Rim = CaculateRimLight(normalWS, positionVS, positionNDC, viewDirectionWS, _RimWidth, _Threshold, _FresnelPow);
        //half Rim  = CaculateRimLight(normalWS, viewDirectionWS, _RimScale, _RimPower);
        half lightDirFront = saturate(dot(normalWS, mainLight.direction)) * Rim;
        half lightDirBack = saturate(dot(normalWS, -mainLight.direction)) * Rim;
        half3 FinalColorWithRim = lerp(FinalColorResult.rgb, baseColor.rgb * _RimColorFront.rgb * _RimColorBrightness, lightDirFront);
        #ifdef _UseBackRim 
            FinalColorWithRim = lerp(FinalColorWithRim, _RimColorBack.rgb, lightDirBack);
        #endif      
        //return half4(FinalColorWithRim , alpha);
        half3 finalColor = lerp(_DissolveColor.rgb, FinalColorWithRim, edge);  
     
        return half4(finalColor , alpha);
    #endif  
    
    half3 finalResult = lerp(_DissolveColor.rgb, FinalColorResult, edge);  

    //return half4(FinalColorResult, alpha);
    return half4(finalResult, alpha);
}


/////////////////////////////////////////////////////////////////////////
//fragment shader part3 - alphTest for shadowcaster and depthonly pass
/////////////////////////////////////////////////////////////////////////
void AlphaClipTest(Varyings input)
{
    half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
    half alpha = baseColor.a;

    #if _UseAlphaClip
        clip(alpha - _CutOff);
    #endif 
}