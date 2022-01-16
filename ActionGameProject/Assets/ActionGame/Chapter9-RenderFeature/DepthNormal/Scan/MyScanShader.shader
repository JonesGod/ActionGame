Shader "URPShader/Chapter9/MyScanShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline"}

        Cull Off ZTest Always ZWrite Off

        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        CBUFFER_START(UnityPerMaterial)

        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;

        CBUFFER_END

        float _ScanDistance;
        float4 _ScannerPos;
        float _ScanWidth;

        TEXTURE2D (_MainTex);
        SAMPLER (sampler_MainTex);
        TEXTURE2D(_CameraDepthNormalsTexture);
        SAMPLER(sampler_CameraDepthNormalsTexture);
        float4x4 Matrix;

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct Varyings
        {   
            float4 positionCS: SV_POSITION;
            float2 uv : TEXCOORD0;     
            float4 interpolatedRay : TEXCOORD1;       
        };

        ENDHLSL

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;

                int index = 0;
                if(input.texcoord.x < 0.5 && input.texcoord.y < 0.5)//左下
                    index = 0;
                else if(input.texcoord.x > 0.5 && input.texcoord.y < 0.5)//右下
                    index = 1;
                else if(input.texcoord.x > 0.5 && input.texcoord.y > 0.5)//右上
                    index = 2;
                else//左上
                    index = 3;
                #if UNITY_UV_STARTS_AT_TOP
                    if(_MainTex_TexelSize.y < 0)
                        index = 3 - index;
                #endif        

                output.interpolatedRay = Matrix[index];

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                float linearDepth = LinearEyeDepth(SampleSceneDepth(input.uv), _ZBufferParams);
                //return linearDepth;
                float3 worldPos = _WorldSpaceCameraPos + linearDepth * input.interpolatedRay.xyz;
                //return half4(worldPos, 1);

                float dist = distance(worldPos, _ScannerPos.xyz);
                //return dist;

				if (dist < _ScanDistance && dist > _ScanDistance - _ScanWidth && SampleSceneDepth(input.uv) > 0)
				{
                    float diff = 1 - (_ScanDistance - dist) / (_ScanWidth);
                    return diff;
                }
                return tex;
            }

            ENDHLSL
        }
    }
}
