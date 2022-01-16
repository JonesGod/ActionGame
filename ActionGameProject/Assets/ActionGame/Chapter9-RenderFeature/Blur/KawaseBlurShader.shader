Shader "URPShader/Chapter9/KawaseBlurShader"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
        _Blur ( "Blur", Float) = 1  
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        ZTest Always Cull Off ZWrite Off 

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)

        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        float _Blur;

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
                float2 res = _MainTex_TexelSize.xy;
                float i = _Blur;

                half4 col;                
                col.rgb = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, input.uv ).rgb;
                col.rgb += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(i, i) * res).rgb;
                col.rgb += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(i, -i) * res).rgb;
                col.rgb += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-i, i) * res).rgb;
                col.rgb += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-i, -i) * res).rgb;
                col.rgb /= 5.0f;

                return half4(col);
            }
            ENDHLSL
        }
    }
}
