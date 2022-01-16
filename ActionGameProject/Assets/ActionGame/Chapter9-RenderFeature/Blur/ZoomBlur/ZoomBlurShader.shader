Shader "URPShader/Chapter9/ZoomBlurShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        float4 _MainTex_ST;
        float _Iteration;
        float _BlurDistance;
        float _BlurIntensity;
        float _XPos;
        float _YPos;
        float2 _CenterPos;

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_SourceTex);
        SAMPLER(sampler_SourceTex);

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
                //float2 dir = (float2(_XPos, _YPos) - input.uv) * _BlurDistance * 0.01;
                float2 dir = (_CenterPos - input.uv) * _BlurDistance * 0.01;
                float4 accumulateColor = 0;

                float4 source = SAMPLE_TEXTURE2D(_SourceTex, sampler_SourceTex, input.uv);

                for(int i = 0; i <_Iteration; i++)
                {
                    accumulateColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + dir * i) / _Iteration;
                } 
                //return accumulateColor / _Iteration; 
                return lerp(source, accumulateColor, _BlurIntensity);
            }
            ENDHLSL
        }        
    }
}
