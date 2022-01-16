Shader "URPShader/Chapter9/DepthNormalScanShader"
{
    Properties
    {
        [HideInInspector]_MainTex("MainTex",2D) = "white"{}
        [HideInInspector][HDR]_ColorX("ColorX", Color) = (1, 1, 1, 1)
        [HideInInspector][HDR]_ColorY("ColorY", Color) = (1, 1, 1, 1)
        [HideInInspector][HDR]_ColorZ("ColorZ", Color) = (1, 1, 1, 1)
        [HideInInspector][HDR]_ColorEdge("ColorEdge",Color)=(1, 1, 1, 1)
        [HideInInspector]_width("Width", float) = 0.1
        [HideInInspector]_Spacing("Spacing", float) = 1
        [HideInInspector]_Speed("Speed", float) = 1
        [HideInInspector]_SampleDistance("SampleDistance", Range(0, 1)) = 1
        [HideInInspector]_NormalSensitivity("NormalSensitivity", float) = 1
        [HideInInspector]_DepthSensitivity("DepthSensitivity", float) = 1
        [HideInInspector][HDR]_OutlineColor("OutlineColr", Color) = (1, 1, 1, 1)
        [KeywordEnum(X,Y,Z)]_AXIS("Axis", float) = 1
    }
    SubShader
    {
        Tags{ "RenderPipeline"="UniversalRenderPipeline" }
        
        Cull Off ZWrite Off ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #pragma multi_compile_local_fragment _AXIS_X _AXIS_Y _AXIS_Z
        
        CBUFFER_START(UnityPerMaterial)

        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        
        CBUFFER_END

        half4 _ColorX;
        half4 _ColorY;
        half4 _ColorZ;
        half4 _ColorEdge;
        half4 _OutlineColor;
        float _width;
        float _Spacing;
        float _Speed;
        float _SampleDistance;
        float _NormalSensitivity;
        float _DepthSensitivity;   

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_CameraDepthNormalsTexture);
        SAMPLER(sampler_CameraDepthNormalsTexture);
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

        float Robert(v2f i)
        {
            half depth[4];
            half2 normal[4];

            float2 uv[4];//計算採樣需要的uv
            uv[0] = i.uv + float2(-1, -1) * _SampleDistance * _MainTex_TexelSize.xy;
            uv[1] = i.uv + float2(1, -1) * _SampleDistance * _MainTex_TexelSize.xy;
            uv[2] = i.uv + float2(-1, 1) * _SampleDistance * _MainTex_TexelSize.xy;
            uv[3] = i.uv + float2(1, 1) * _SampleDistance * _MainTex_TexelSize.xy;
                
            for(float t=0; t<4; t++)
            {
                half4 depthnormalTex = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uv[t]);
                normal[t] = depthnormalTex.xy;//得到臨時法線
                depth[t] = depthnormalTex.z * 1.0 + depthnormalTex.w / 255.0;//得到線性深度
            }                
            //depth檢測
            float Dep = abs(depth[0] - depth[3]) + abs(depth[1] - depth[2]) * _DepthSensitivity > 0.01 ? 1 : 0;
            //normal檢測
            float2 nor = abs(normal[0] - normal[3]) * abs(normal[1] - normal[2]) * _NormalSensitivity;
            float Nor = (nor.x + nor.y) > 0.01 ? 1 : 0;
                
            return saturate(Dep + Nor);
        }        
        
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
            
            half4 frag(v2f i) : SV_TARGET
            {
                half outline = Robert(i);
                //return outline;

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                //return lerp(tex, _OutlineColor, outline);

                //half4 depthnormal = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.uv);
                //float depth01 = depthnormal.z * 1.0 + depthnormal.w / 255.0;//得到01線性的深度
                float eyedepth = LinearEyeDepth(SampleSceneDepth(i.uv), _ZBufferParams);
                //return eyedepth;
                
                float3 WSpos = _WorldSpaceCameraPos + eyedepth * i.Direction + float3(0.1, 0.1, 0.1);//這樣也可以得到正確的世界座標
                return half4(WSpos, 1);
                //return half4(frac(WSpos), 1);

                float3 WSpos01 = WSpos * _ProjectionParams.w;
                float3 Line = step(1 - _width, frac(WSpos / _Spacing));//線框
                float4 Linecolor = Line.x * _ColorX + Line.y * _ColorY + Line.z * _ColorZ + outline * _OutlineColor;//計算線框顏色
                //return Linecolor + tex;

                float mask = 0;                
                #ifdef _AXIS_X
                    mask = saturate(pow(abs(frac(WSpos.x * 0.1 + _Time.y * 0.1 * _Speed) - 0.75), 10) * 30);//在X軸方向計算mask
                    mask += step(0.999, mask);
                #elif _AXIS_Y
                    mask = saturate(pow(abs(frac(WSpos.y * 0.1 - _Time.y * 0.1 * _Speed) - 0.25), 10) * 30);//在Y軸方向計算mask
                    mask += step(0.999, mask);
                #elif _AXIS_Z
                    mask = saturate(pow(abs(frac(WSpos.z * 0.1 + _Time.y * 0.1 * _Speed) - 0.75), 10) * 30);//在Z軸方向計算mask
                    mask += step(0.999, mask);
                #endif
                //return mask;
                
                //return tex * saturate(1 - mask) + (Linecolor + _ColorEdge) * mask;

                //用深度圖製作背景不要有限框
                float background = step(0.001, SampleSceneDepth(i.uv));
                half4 effectArea =  tex * saturate(1 - mask) + (Linecolor + _ColorEdge) * mask;
                return lerp(tex, effectArea, background);
            }             
            ENDHLSL
        }
    }
}