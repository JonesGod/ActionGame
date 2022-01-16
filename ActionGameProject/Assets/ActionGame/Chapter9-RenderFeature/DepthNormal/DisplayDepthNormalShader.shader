Shader "URPShader/Chapter9/DisplayDepthNormalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }
        
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            ZTest Always
            ZWrite Off
            
            HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            #pragma vertex vert
            #pragma fragment frag

            
            CBUFFER_START(UnityPerMaterial)

            float4 _MainTex_TexelSize;

            CBUFFER_END

            TEXTURE2D(_CameraDepthNormalsTexture);    
            SAMPLER(sampler_CameraDepthNormalsTexture);
            
            struct Attributes
            {
                float4 positionOS: POSITION;
                float2 uv: TEXCOORD0;
            };
            
            struct Varyings
            {
                float2 uv: TEXCOORD0;
                float4 positionCS: SV_POSITION;
            };            

            float3 DecodeViewNormalStereo(float4 enc4)
            {
                float kScale = 1.7777;
                float3 nn = enc4.xyz * float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
                float g = 2.0 / dot(nn.xyz, nn.xyz);
                float3 n;
                n.xy = g * nn.xy;
                n.z = g - 1;
                return n;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                
                //當有多個RenderTarget時，需要自己處理UV翻轉問題
                #if UNITY_UV_STARTS_AT_TOP
                    if (_MainTex_TexelSize.y < 0) 
                    output.uv.y = 1 - output.uv.y; 
                #endif
                
                return output;
            }
            
            half4 frag(Varyings input): SV_Target
            {
                half4 normalDepth = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UnityStereoTransformScreenSpaceTex(input.uv));
                half3 normal = DecodeViewNormalStereo(normalDepth);
                return half4(normal * 0.5 + 0.5, 1);
            }
            ENDHLSL
            
        }
    }
}
