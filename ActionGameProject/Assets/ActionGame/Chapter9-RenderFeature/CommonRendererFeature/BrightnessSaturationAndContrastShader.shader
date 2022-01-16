Shader "URPShader/Chapter9/BrightnessSaturationAndContrastShader"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 3)) = 1.0
        _Saturation ("Saturation", Range(0, 3)) = 1.0
        _Contrast ("Contrast", Range(0, 3)) = 1.0        
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        ZTest Always Cull Off ZWrite Off 

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

        CBUFFER_START(UnityPerMaterial)

        float4 _BaseMap_ST;
        float _Brightness;
        float _Saturation;
        float _Contrast;

        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;                
        };

        ENDHLSL

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag            

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 renderTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex , input.uv);;

                // Apply brightness
                half3 finalColor = renderTex.rgb * _Brightness;

                // Apply saturation
                half luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
                half3 luminanceColor = half3(luminance, luminance, luminance);
                finalColor = lerp(luminanceColor, finalColor, _Saturation);

                // Apply contrast
                half3 avgColor = half3(0.5, 0.5, 0.5);
                finalColor = lerp(avgColor, finalColor, _Contrast);

                return half4(finalColor, renderTex.a);
            }
            ENDHLSL
        }
    }
}
