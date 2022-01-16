Shader "Unlit/HairShadowSolidColorShader"
{
    Properties 
    {
        _BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        half4 _BaseColor;

        CBUFFER_END      

        ENDHLSL
        
        Pass 
        {
            Name "HairSolidColor"
            Tags { "LightMode"="UniversalForward" }

            Cull Off
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM            

            struct Attributes 
            {
                float4 positionOS : POSITION;
                float4 color : Color;
            };

            struct Varyings 
            {
                float4 positionCS : SV_POSITION;
                half4 color : Color;
                float4 positionNDC : TEXCOORD0;
            };
            
            #pragma vertex vert
            #pragma fragment frag     

            Varyings vert(Attributes input) 
            {
                Varyings output;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = positionInputs.positionCS;
                output.positionNDC = positionInputs.positionNDC;
                output.color = input.color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target 
            {
                //In DirectX, z/w from [0, 1], and use reversed Z
                //So, it means we aren't adapt the sample for OpenGL platform
                float depth = (input.positionCS.z / input.positionCS.w);
                return float4(0, depth, 0, 1);
            }
            ENDHLSL
        }

        Pass 
        {
            Name "FaceDepthOnly"
            Tags { "LightMode"="UniversalForward" }

            ColorMask 0
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM            

            struct Attributes 
            {
                float4 positionOS : POSITION;
            };

            struct Varyings 
            {
                float4 positionCS : SV_POSITION;
            };
            
            #pragma vertex vert
            #pragma fragment frag     

            Varyings vert(Attributes input) 
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                return output;
            }

            half4 frag(Varyings input) : SV_Target 
            {
                return (0, 0, 0, 1);
            }
            ENDHLSL
        }

    }
}