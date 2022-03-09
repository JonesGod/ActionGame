Shader "URPShader/Practice/ToonShaderDirDissolve"
{
    Properties
    {
        [ToggleUI]_IsFace ("IsFace", Float) = 0
        [Toggle(_UseFringeShadow)]_UseFringeShadow("UseFringeShadow", Float) = 0

        [Header(BaseColor)]
        [MainTexture]_BaseMap ("BaseMap", 2D) = "white" {}
        [HDR][MainColor]_BaseColor ("BaseColor", Color) = (1, 1, 1, 1)

        [Header(AlphaClip)]
        [Toggle(_UseAlphaClip)]_UseAlphaClip ("UseAlphaClip", Float) = 1 
        _CutOff ("AlphaCuteOff", Range(0.0, 1.0)) = 0.0

        [Header(Outline)]
        _OutlineWidth ("OutlineWidth", Range(0, 10)) = 1
        _OutlineColor ("OutlineColor", Color) = (0, 0, 0, 0)
        _OutlineZOffsetAmount ("OutlineZOffsetAmount", Range(0, 1)) = 0

        [Header(Lighting)]
        _CelShadeGradient ("CelShadeGradient", Range(0, 1)) = 0.5
        _CelShadeArea ("CelShadeArea", Range(-1, 1)) = 0

        [Header(Shadow)]
        _ReceiveShadowMappingAmount ("ReceiveShadowMappingAmount", Range(0, 1)) = 0.5
        _ReceiveShadowMappingOffset ("ReceiveShadowMappingOffset", Range(-2, 2)) = 0
        _ShadowPartColor ("ShadowPartColor", Color) = (0.5, 0.5, 0.5)
        _FringeShadowOffset("FringeShadowOffset", Range(-0.02, 0.02)) = 0
        _FringeShadowThreshold("FringeShadowThreshold", Range(0, 0.02)) = 0.001

        [Header(RimLight)]
        [Toggle(_UseRim)]_UseRim ("UseRim", Float) = 1
        [HDR]_RimColorFront ("RimColorFront", Color) = (1, 1, 1, 1)
        _RimColorBrightness ("RimColorBrightness", Range(1, 8)) = 4
        [Toggle(_UseBackRim)]_UseBackRim ("UseBackRim", Float) = 1
        [HDR]_RimColorBack ("RimColorBack", Color) = (1, 1, 1, 1)
        _RimWidth ("RimWidth", Range(0, 0.1)) = 0.01
        _Threshold ("Threshold", Range(0, 1)) = 0.09
        _FresnelPow ("FresnelPow", Range(0, 16)) = 0
        // BasicWay
        // _RimPower ("RimPower", Float) = 1
        // _RimScale ("RimScale", Float) = 1
        [Header(Dissolve)]
        [HDR]_DissolveColor ("DissolveColor", Color) = (1, 1, 1, 1)
        _DissolveMap ("DissolveTexture", 2D) = "white"{}
        _DissolveAmount ("DissolveAmount", Range(-1.0, 1.5)) = 0.0
        _DissolveWidth ("DissolveWidth", Range(-1.0, 1.0)) = 0.0
        _TextureFade ("TextureFade", Float) = 1.0
        _DissolvePlaneX ("DissolvePlaneX", Range(-1, 1)) = 0.0
        _DissolvePlaneY ("DissolvePlaneY", Range(-1, 1)) = 1.0
        _DissolvePlaneZ ("DissolvePlaneZ", Range(-1, 1)) = 0.0

        //[Header(StrandSpecular)]
        //_SpecularExponent ("SpecularExponent", Float) = 1
        //_SpecularScale ("SpecularScale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

        HLSLINCLUDE

        #pragma shader_feature_local_fragment _UseAlphaClip
        
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZTest LEqual
            ZWrite On
            Blend One Zero

            HLSLPROGRAM

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #pragma shader_feature_local_fragment _UseRim
            #pragma shader_feature_local_fragment _UseBackRim
            #pragma shader_feature _UseFringeShadow

            #include "ToonShaderIncludeDirDissolve.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            //Tags { "LightMode"="Outline" }

            Cull Front

            HLSLPROGRAM
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #define OutlinePass

            #include "ToonShaderIncludeDirDissolve.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            HLSLPROGRAM

            #define ApplyShadowBiasFix

            #include "ToonShaderIncludeDirDissolve.hlsl"

            #pragma vertex vert
            #pragma fragment AlphaClipTest

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            HLSLPROGRAM

            //#define OutlinePass

            #include "ToonShaderIncludeDirDissolve.hlsl"

            #pragma vertex vert
            #pragma fragment AlphaClipTest

            ENDHLSL
        }
    }
}
