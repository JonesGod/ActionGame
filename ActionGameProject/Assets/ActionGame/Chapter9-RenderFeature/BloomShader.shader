Shader "URPShader/Chapter9/BloomShader"
{
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Blur ("Blur", Float) = 1.0
        _Bloom ("Bloom (RGB)", 2D) = "black" {}
		_LuminanceThreshold ("Luminance Threshold", Float) = 0.5
	}
	SubShader 
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        ZTest Always Cull Off ZWrite Off 

		HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		
		CBUFFER_START(UnityPerMaterial)

        float4 _MainTex_TexelSize;
        float _Blur;
        float _LuminanceThreshold;

        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_BloomTex);
        SAMPLER(sampler_BloomTex);

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

        struct VaryingsBloom 
        {
			float4 positionCS : SV_POSITION; 
			half4 uv : TEXCOORD0;
		};		  		
		
		half luminance(half3 color) 
        {
			return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 
		}
		
		ENDHLSL
		
		Pass //pass0 
        {  
			HLSLPROGRAM 

			#pragma vertex vertExtractBright  
			#pragma fragment fragExtractBright  

            Varyings vertExtractBright(Attributes v) 
            {
                Varyings o;
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);                
                o.uv = v.texcoord;
                        
                return o;
            }

            half4 fragExtractBright(Varyings i) : SV_Target 
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half val = clamp(luminance(c.rgb) - _LuminanceThreshold, 0.0, 1.0);
                
                return c * val;
            }
			
			ENDHLSL  
		}
		
		UsePass "URPShader/Chapter9/GaussianBlurShader/GAUSSIAN_BLUR_VERTICAL"   //pass1
		
		UsePass "URPShader/Chapter9/GaussianBlurShader/GAUSSIAN_BLUR_HORIZONTAL" //pass2
		
		Pass //pass3
        {  

			HLSLPROGRAM

			#pragma vertex vertBloom  
			#pragma fragment fragBloom  

            VaryingsBloom vertBloom(Attributes v) 
            {
                VaryingsBloom o;
                
                o.positionCS = TransformObjectToHClip (v.positionOS.xyz);
                o.uv.xy = v.texcoord;		
                o.uv.zw = v.texcoord;
                
                #if UNITY_UV_STARTS_AT_TOP			
                if (_MainTex_TexelSize.y < 0.0)
                    o.uv.w = 1.0 - o.uv.w;
                #endif
                                
                return o; 
            }

            half4 fragBloom(VaryingsBloom i) : SV_Target 
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy) + SAMPLE_TEXTURE2D(_BloomTex, sampler_BloomTex, i.uv.zw);
            } 
			
			ENDHLSL  
		}
	}
}

