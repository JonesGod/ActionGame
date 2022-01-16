Shader "URPShader/Chapter9/GaussianBlurShader"
{
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Blur ("Blur", Float) = 1.0
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
            float2 uv[5] : TEXCOORD0;                
        };
		    
		ENDHLSL
		
		Pass 
		{
			NAME "GAUSSIAN_BLUR_VERTICAL"
			
			HLSLPROGRAM
			  
			#pragma vertex vertBlurVertical  
			#pragma fragment fragBlur

			Varyings vertBlurVertical(Attributes v) 
			{
				Varyings o;
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				
				half2 uv = v.texcoord;
				
				o.uv[0] = uv;
				o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _Blur;
				o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _Blur;
				o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _Blur;
				o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _Blur;
						
				return o;
			}

			half4 fragBlur(Varyings i) : SV_Target 
			{
				float weight[3] = {0.4026, 0.2442, 0.0545};
				
				half3 sum = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv[0]).rgb * weight[0];
				
				for (int it = 1; it < 3; it++) {
					sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[it*2-1]).rgb * weight[it];
					sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[it*2]).rgb * weight[it];
				}
				
				return half4(sum, 1.0);
			}			  
			ENDHLSL  
		}
		
		Pass 
		{  
			NAME "GAUSSIAN_BLUR_HORIZONTAL"
			
			HLSLPROGRAM  
			
			#pragma vertex vertBlurHorizontal  
			#pragma fragment fragBlur

			Varyings vertBlurHorizontal(Attributes v) 
			{
				Varyings o;
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				
				half2 uv = v.texcoord;
				
				o.uv[0] = uv;
				o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _Blur;
				o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _Blur;
				o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _Blur;
				o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _Blur;
						
				return o;
			}

			half4 fragBlur(Varyings i) : SV_Target 
			{
				float weight[3] = {0.4026, 0.2442, 0.0545};
				
				half3 sum = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv[0]).rgb * weight[0];
				
				for (int it = 1; it < 3; it++) {
					sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[it*2-1]).rgb * weight[it];
					sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[it*2]).rgb * weight[it];
				}
				
				return half4(sum, 1.0);
			}		
			ENDHLSL
		}
	} 
}
