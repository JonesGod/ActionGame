Shader "URPShader/Practice/RockShieldShader"
{
    Properties
    {
        [MainTexture]_BaseMap ("BaseMap", 2D) = "white" {}
        [HDR][MainColor]_BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
        _FresnelScale ("Fresnel Scale", Float) = 1
        _FresnelPower("FresnelPower", Float) = 1
        _FresnelOutScale ("FresnelOut Scale", Float) = 1
        _FresnelOutPower ("FresnelOut Power", Float) = 1
        _FresnelBias ("Fresnel Bias", Float) = 0
        _AlphaScale ("AlphaScale", Range(0, 1)) = 1 
        _ScrollX ("Fresnel ScrollX", Float) = 1
        _ScrollY ("Fresnel ScrollY", Float) = 1   
        [Header(HitEffect)]
        _ColorGlitter ("ColorGlitter", Color) = (0, 0, 0, 0)     
        [Header(Dissolve)]
        [HDR]_DissolveColor ("DissolveColor", Color) = (1, 1, 1, 1)
        _DissolveMap ("DissolveTexture", 2D) = "white"{}
        _DissolveAmount ("DissolveAmount", Range(-1.0, 1.5)) = 0.0
        _DissolveWidth ("DissolveWidth", Range(-1.0, 1.0)) = 0.0
        _TextureFade ("TextureFade", Float) = 1.0
        _DissolvePlaneX ("DissolvePlaneX", Range(-1, 1)) = 0.0
        _DissolvePlaneY ("DissolvePlaneY", Range(-1, 1)) = 1.0
        _DissolvePlaneZ ("DissolvePlaneZ", Range(-1, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

        CBUFFER_START(UnityPerMaterial)

        float4 _BaseMap_ST;
        half4 _BaseColor;
        half _FresnelScale;
        half _FresnelPower;
        half _FresnelOutScale;
        half _FresnelOutPower;
        half _FresnelBias;
        half _AlphaScale;
        half _ScrollX;
        half _ScrollY;
        //HitBlink
        half4 _ColorGlitter;
        //Dissolve
        half4 _DissolveColor;
        float4 _DissolveMap_ST;
        float _DissolveAmount;
        float _DissolveWidth;
        float _TextureFade;
        float _DissolvePlaneX;
        float _DissolvePlaneY;
        float _DissolvePlaneZ;

        CBUFFER_END

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);
        TEXTURE2D(_DissolveMap);
        SAMPLER(sampler_DissolveMap);

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS : TEXCOORD0;
            float3 normalWS : TEXCOORD1;
            float4 uv : TEXCOORD2;
            float3 positionOS : TEXCOORD3;  
        };

        ENDHLSL

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off
            //Cull Off(雙面)
            Blend SrcAlpha OneMinusSrcAlpha 

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            Varyings vert (Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS;

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);

                output.normalWS = TransformObjectToWorldNormal(input.normalOS, true);
                
                output.uv.xy = TRANSFORM_TEX(input.texcoord, _BaseMap) + frac(float2(_ScrollX, _ScrollY) * _Time.y);
                output.uv.zw = TRANSFORM_TEX(input.texcoord, _DissolveMap);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half3 normalWS = input.normalWS;
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - input.positionWS);

                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv.xy) * _BaseColor;
                
                half3 ambient = _GlossyEnvironmentColor.rgb * albedo;

                Light mainLight = GetMainLight();
                half4 LightColor = half4(mainLight.color, 1);
                half3 LightDir = normalize(mainLight.direction);

                half3 diffuse = LightColor.rgb * albedo * saturate(dot(input.normalWS, LightDir));

                //Dissolve
                input.uv.w = input.uv.w - _Time.y * 0.2;
                half3 dissolveMap = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, input.uv.zw).rgb; 
                float dirDissolveMap = dot(-input.positionOS, float3(_DissolvePlaneX, _DissolvePlaneY, _DissolvePlaneZ)) + dissolveMap.b * _TextureFade; 
                clip(step(_DissolveAmount, dirDissolveMap) - 0.01);                
                float edge = step(_DissolveAmount + _DissolveWidth, dirDissolveMap);   

                half fresnel = _FresnelBias + 1 - saturate(pow(dot(viewDirectionWS, normalWS) + _FresnelScale, _FresnelPower));;
                //half fresnel = _FresnelBias + _FresnelScale * pow(1 - dot(viewDirectionWS, normalWS), _FresnelPower);
                half fresnelOut = _FresnelBias + saturate(pow(dot(viewDirectionWS, normalWS) + _FresnelOutScale, _FresnelOutPower));
                return half4(albedo * fresnel * fresnelOut) + _ColorGlitter;
            }
            ENDHLSL
        }
    }
}