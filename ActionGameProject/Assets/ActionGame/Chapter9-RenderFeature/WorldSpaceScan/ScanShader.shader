Shader "URPShader/Chapter9/ScanShader"
{
    Properties
    {
        [HideInInspector]_MainTex("MainTex", 2D)="white"{}
        [HDR]_colorX("ColorX", Color) = (1, 1, 1, 1)
        [HDR]_colorY("ColorY", Color) = (1, 1, 1, 1)
        [HDR]_ColorZ("ColorZ", Color) = (1, 1, 1, 1)
        [HDR]_ColorEdge("ColorEdge", Color) = (1, 1, 1, 1)
        _width("Width", float) = 0.02
        _Spacing("Spacing", float) = 1
        _Speed("Speed", float) = 1
    }
    SubShader
    {
        Tags{ "RenderPipeline"="UniversalRenderPipeline" }
        
        Cull Off ZWrite Off ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        
        CBUFFER_START(UnityPerMaterial)

        float4 _MainTex_TexelSize;

        CBUFFER_END

        half4 _colorX;
        half4 _colorY;
        half4 _ColorZ;
        half4 _ColorEdge;
        float _width;
        float _Spacing;
        float _Speed;

        TEXTURE2D( _MainTex);
        SAMPLER(sampler_MainTex);
        float4x4 Matrix;
        
        struct a2v
        {
            float4 positionOS : POSITION;
            float2 texcoord : TEXCOORD0;
        };
        
        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 Direction : TEXCOORD1;
        };
        
        ENDHLSL

        pass
        {
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv = i.texcoord;
                
                int index = 0;
                if(i.texcoord.x < 0.5 && i.texcoord.y < 0.5)
                    index = 0;
                else if(i.texcoord.x > 0.5 && i.texcoord.y < 0.5)
                    index = 1;
                else if(i.texcoord.x > 0.5 && i.texcoord.y > 0.5)
                    index = 2;
                else
                    index = 3;
                    
                o.Direction = Matrix[index].xyz;
                
                return o;
            }
            
            half4 frag(v2f i):SV_TARGET
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half depth = LinearEyeDepth(SampleSceneDepth(i.uv), _ZBufferParams);
                float3 WSpos = _WorldSpaceCameraPos + depth * i.Direction + float3(0.1, 0.1, 0.1);//得到世界座標
                //return half4(frac(WSpos), 1);                

                float3 Line = step(1 - _width, frac(WSpos / _Spacing));//線框
                //return half4(Line, 1);

                float4 Linecolor = Line.x * _colorX + Line.y * _colorY + Line.z * _ColorZ;//給線框上色
                //return Linecolor + tex;

                //用深度圖製作背景不要有限框
                float background = step(0.001, SampleSceneDepth(i.uv));
                return SampleSceneDepth(i.uv);
                return lerp(tex, Linecolor + tex, background);
            }
            ENDHLSL
        }
    }
}