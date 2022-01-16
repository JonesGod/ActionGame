Shader "URPShader/Chapter9/BokehBliurShader"
{
    Properties
    {
        [HideinInspector]_MainTex("MainTex",2D) = "white"{}
        _NearDist ("NearDist", Float) = 0.0
        _loop ("Loop", Float) = 1.0
    }
    SubShader
    {
        Tags{ "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Transparent" }
        Cull Off ZWrite Off ZTest Always
        
        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        CBUFFER_START(UnityPerMaterial)
        
        float4 _MainTex_TexelSize;
        
        CBUFFER_END

        float _NearDist;
        float _FarDist;
        float _BlurSmoothness;
        float _loop;
        float _radius;

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_SourceTex);
        SAMPLER(sampler_SourceTex);
        
        struct a2v
        {
            float4 positionOS : POSITION;
            float2 texcoord : TEXCOORD;
        };
        
        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD;
            float4 scrPos : TEXCOORD1;
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
                return o;
            }
            
            half4 frag(v2f i):SV_TARGET
            {
                //137.5度 => 137.5/360*2*3.14 = 2.3398
                float a = 2.3398;
                float2x2 rotate = float2x2(cos(a), -sin(a), sin(a), cos(a));

                float2 UVpos = float2(_radius, 0);

                float2 uv;
                float r;
                real4 tex = 0;

                for(int t = 1; t<_loop; t++)
                {
                    r = sqrt(t);
                    UVpos = mul(rotate, UVpos);
                    uv = i.uv + _MainTex_TexelSize.xy * UVpos * r;
                    tex += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                }                
                return tex / (_loop - 1);
            }
            ENDHLSL
        }

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
                o.scrPos = ComputeScreenPos(o.positionCS);
                
                return o;
            }
            
            half4 frag(v2f i):SV_TARGET
            {
                float2 scrUV = i.scrPos.xy / i.scrPos.w;
                float depth = Linear01Depth(SampleSceneDepth(i.uv), _ZBufferParams);

                half4 blur = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 Source = SAMPLE_TEXTURE2D(_SourceTex, sampler_SourceTex, i.uv);
                
                _NearDist *= _ProjectionParams.w;
                _FarDist *= _ProjectionParams.w;
                
                // float dis = 1 - smoothstep(_NearDist, saturate(_NearDist + _BlurSmoothness), depth);//計算近處                
                // dis += smoothstep(_FarDist, saturate(_FarDist + _BlurSmoothness), depth);//計算遠處
                // float dis = saturate(abs(depth - _NearDist) * 1);//類似掃光效果

                float dis = smoothstep(_NearDist, saturate(_NearDist + _BlurSmoothness), depth);//只計算近處(單純景深) 
                
                half4 combine = lerp(Source, blur, dis);
                return combine;
            }
            ENDHLSL
        }
    }
}