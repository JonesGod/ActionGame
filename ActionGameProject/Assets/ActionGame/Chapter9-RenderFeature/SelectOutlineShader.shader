Shader "URPShader/Chapter9/SelectOutline"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white"{}
        _SolidColor("SolidColor", Color) = (1, 1, 1, 1)
        _Blur("Blur", float) = 3
        //[KeywordEnum(_InColorOn,_InColorOff)]_ColorType("ColorType", float) = 1
    }
    SubShader
    {
        Tags{ "RenderPipeline"="UniversalRenderPipeline" }
        
        Cull Off ZWrite Off ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #pragma multi_compile_local _InColorOn _InColorOff

        CBUFFER_START(UnityPerMaterial)

        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        half4 _SolidColor;
        float _Blur;

        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);   
        TEXTURE2D(_BlurTex);
        SAMPLER(sampler_BlurTex);
        TEXTURE2D(_SourTex);
        SAMPLER(sampler_SourTex);
        
        struct a2v
        {
            float4 positionOS : POSITION;
            float2 texcoord : TEXCOORD0;
        };
        
        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float4 uv[4] : TEXCOORD0;
        };
        ENDHLSL
        
        pass//上純色
        {
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                return _SolidColor;
            }
            ENDHLSL            
        }
        pass//雙重模糊DownPass
        {
            NAME"Down"            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv[2].xy = i.texcoord;
                o.uv[0].xy = i.texcoord + float2(1,1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[0].zw = i.texcoord + float2(-1,1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[1].xy = i.texcoord + float2(1,-1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[1].zw = i.texcoord + float2(-1,-1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[2].xy) * 0.5;
                for(int t = 0; t<2; t++)
                {
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[t].xy) * 0.125;
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[t].zw) * 0.125;
                }
                return tex;
            }
            ENDHLSL
        }
        pass//雙重模糊UpPass
        {
            NAME"Up"
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv[0].xy = i.texcoord + float2(1,1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[0].zw = i.texcoord + float2(-1,1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[1].xy = i.texcoord + float2(1,-1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[1].zw = i.texcoord + float2(-1,-1) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[2].xy = i.texcoord + float2(0,2) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[2].zw = i.texcoord + float2(0,-2) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[3].xy = i.texcoord + float2(-2,0) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                o.uv[3].zw = i.texcoord + float2(2,0) * _MainTex_TexelSize.xy * (1 + _Blur) * 0.5;
                
                return o;
            }
            
            half4 frag(v2f i) : SV_TARGET
            {
                half4 tex = 0;
                
                for(int t=0; t<2; t++)
                {
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[t].xy)/6;
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[t].zw)/6;
                }
                
                for(int k = 2; k<4; k++)
                {
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[k].xy)/12;
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[k].zw)/12;
                }
                return tex;
            }
            ENDHLSL
        }
        pass//合併所有圖像
        {
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(a2v i)
            {
                v2f o;

                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv[0].xy = i.texcoord.xy;

                return o;
            }
            half4 frag(v2f i) : SV_TARGET
            {
                half4 blur = SAMPLE_TEXTURE2D(_BlurTex, sampler_BlurTex, i.uv[0].xy);
                half4 source = SAMPLE_TEXTURE2D(_SourTex, sampler_SourTex, i.uv[0].xy);
                half4 solid = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[0].xy);                
                half4 color;
                
                #ifdef _InColorOn                
                    color = abs(blur - solid) + source;
                #elif _InColorOff
                    color = saturate(blur - solid) + source;
                #endif
                return color;
            }
            ENDHLSL
        }
    }
}    