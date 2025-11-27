// Blood Effect Shader for URP
// Custom Inputs are X = Pan Offset, Y = UV Warp Strength, Z = Gravity
// Specular Alpha is used like a metalness control. High values are more like dielectrics, low are more like metals

Shader "Particles/Blood Effect URP"
{
    Properties
    {
        [Header(Color Controls)]
        [HDR] _BaseColor ("Base Color Mult", Color) = (1,1,1,1)
        _LightStr ("Lighting Strength", float) = 0.85
        _AlphaMin ("Alpha Clip Min", Range (-0.01, 1.01)) = 0.1
        _AlphaSoft ("Alpha Clip Softness", Range (0,1)) = 0.022
        _EdgeDarken ("Edge Darkening", float) = 1.0
        _ProcMask ("Procedural Mask Strength", float) = 1.0

        [Header(Mask Controls)]
        _MainTex ("Mask Texture", 2D) = "white" {}
        _MaskStr ("Mask Strength", float) = 0.7
        _Columns ("Flipbook Columns", Int) = 1
        _Rows ("Flipbook Rows", Int) = 1
        _ChannelMask ("Channel Mask", Vector) = (1,0,0,0)
        [Toggle] _FlipU("Flip U Randomly", float) = 0
        [Toggle] _FlipV("Flip V Randomly", float) = 0

        [Header(Noise Controls)]
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseAlphaStr ("Noise Strength", float) = 0.8
        _ChannelMask2 ("Channel Mask",Vector) = (1,0,0,0)
        _Randomize ("Randomize Noise", float) = 1.0

        [Header(UV Warp Controls)]
        _WarpTex ("Warp Texture", 2D) = "gray" {}
        _WarpStr ("Warp Strength", float) = 0.1

        [Header(Vertex Physics)]
        _FallOffset ("Gravity Offset", range(-1,0)) = -1.0
        _FallRandomness ("Gravity Randomness", float) = 0.25

        [Header(Specular Reflection)]
        [Toggle(SPECULAR_REFLECTION)] _UseSpecular ("Use Specular Reflection", Float) = 0
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Normal ("Normal Map", 2D) = "bump" {}
        _FlattenNormal ("Flatten Normal", Range(0,1)) = 0.5
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _ReflectionSat ("Reflection Saturation", Range(0,1)) = 1.0

        // URP specific properties
        _Surface("Surface Type", Float) = 0
        _Blend("Blend Mode", Float) = 0
        _Cull("Cull Mode", Float) = 0
        _AlphaClip("Alpha Clipping", Float) = 0
        _SrcBlend("Source Blend", Float) = 1
        _DstBlend("Destination Blend", Float) = 0
        _ZWrite("Z Write", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "ShaderModel" = "4.5"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            // Feature keywords
            #pragma shader_feature_local SPECULAR_REFLECTION

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Properties
            CBUFFER_START(UnityPerMaterial)
                // Color Controls
                half4 _BaseColor;
                half _LightStr;
                half _AlphaMin;
                half _AlphaSoft;
                half _EdgeDarken;
                half _ProcMask;

                // Mask Controls
                float4 _MainTex_ST;
                half _MaskStr;
                half _Columns;
                half _Rows;
                half4 _ChannelMask;
                half _FlipU;
                half _FlipV;

                // Noise Controls
                float4 _NoiseTex_ST;
                half _NoiseAlphaStr;
                half4 _ChannelMask2;
                half _Randomize;

                // UV Warp Controls
                float4 _WarpTex_ST;
                half _WarpStr;

                // Vertex Physics
                half _FallOffset;
                half _FallRandomness;

                // Specular Reflection
                half4 _SpecularColor;
                half _FlattenNormal;
                half _ReflectionSat;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_WarpTex); SAMPLER(sampler_WarpTex);
            
            #ifdef SPECULAR_REFLECTION
                TEXTURE2D(_Normal); SAMPLER(sampler_Normal);
                TEXTURE2D(_ReflectionTex); SAMPLER(sampler_ReflectionTex);
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 texcoord0 : TEXCOORD0; // Z is Random, W is Lifetime
                float3 texcoord1 : TEXCOORD1; // X is Pan Offset, Y is UV Warp Strength, Z is Gravity
                float4 color : COLOR;
                #ifdef SPECULAR_REFLECTION
                    half4 tangentOS : TANGENT;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 customData : TEXCOORD3; // XY is custom (panDistanceOffset & warpStrength), Z is stable random
                
                #ifdef SPECULAR_REFLECTION
                    float3 viewDirWS : TEXCOORD4;
                    float3 tangentWS : TEXCOORD5;
                    float3 bitangentWS : TEXCOORD6;
                #endif
                
                half3 vertLight : TEXCOORD7;
                float fogFactor : TEXCOORD8;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                // Vertex animation with gravity
                float lifetime = input.texcoord0.w;
                lifetime = lifetime * lifetime + (_FallOffset + ((input.texcoord0.z - 0.5) * _FallRandomness)) * lifetime;
                float3 fallPos = lifetime * float3(0, input.texcoord1.z, 0);

                // UV flipping
                float2 UVflip = round(frac(float2(input.texcoord0.z * 13, input.texcoord0.z * 8)));
                UVflip = UVflip * 2 - 1;
                UVflip = lerp(1, UVflip, float2(_FlipU, _FlipV));

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz + fallPos);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                #ifdef SPECULAR_REFLECTION
                    normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                #endif

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.color = input.color;
                output.color.a *= output.color.a;
                output.color.a += _AlphaMin;
                output.customData = float3(input.texcoord1.xy, input.texcoord0.z);

                // UV setup
                output.uv.xy = TRANSFORM_TEX(input.texcoord0.xy * UVflip, _MainTex);
                output.uv.zw = output.uv.xy * half2(_Columns, _Rows) + input.texcoord0.z * half2(3, 8) * _Randomize;

                #ifdef SPECULAR_REFLECTION
                    output.tangentWS = normalInput.tangentWS;
                    output.bitangentWS = normalInput.bitangentWS;
                    output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                #endif

                // Vertex lighting
                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                output.vertLight = vertexLight + SampleSH(normalInput.normalWS);
                output.vertLight = lerp(1, output.vertLight, _LightStr);

                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                // Sample UV warp
                float4 uvWarp = SAMPLE_TEXTURE2D(_WarpTex, sampler_WarpTex, 
                    input.uv.zw * _WarpTex_ST.xy + _WarpTex_ST.zw * (input.customData.x + 1) + (float2(5, 8) * input.customData.z));
                float2 warp = (uvWarp.xy * 2) - 1;
                warp *= _WarpStr * input.customData.y;

                // Sample mask texture
                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy * _MainTex_ST.xy + warp);
                mask = saturate(lerp(1, mask, _MaskStr));

                // Edge mask to prevent spill
                half2 tempUV = frac(input.uv.xy * half2(_Columns, _Rows)) - 0.5;
                tempUV *= tempUV * 4;
                half edgeMask = saturate(tempUV.x + tempUV.y);
                edgeMask *= edgeMask;
                edgeMask = 1 - edgeMask;
                edgeMask = lerp(1.0, edgeMask, _ProcMask);

                mask *= edgeMask;
                half4 col = max(0.001, input.color);
                col.a = saturate(dot(mask, _ChannelMask));

                // Sample noise
                half4 noise4 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, 
                    input.uv.zw * _NoiseTex_ST.xy + _NoiseTex_ST.zw * input.customData.x + warp);
                half noise = dot(noise4, _ChannelMask2);
                noise = saturate(lerp(1, noise, _NoiseAlphaStr));

                // Alpha clipping
                col.a *= noise;
                half preClipAlpha = col.a;
                half clippedAlpha = saturate((preClipAlpha * input.color.a - _AlphaMin) / _AlphaSoft);
                col.a = clippedAlpha;

                // Base lighting
                float3 baseLighting = max(0.01, input.vertLight);

                #ifdef SPECULAR_REFLECTION
                    // Sample and transform normal
                    half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, 
                        input.uv.zw * _NoiseTex_ST.xy + _NoiseTex_ST.zw * input.customData.x + warp));
                    
                    // Flatten normals near alpha edge
                    normalTS.z = _FlattenNormal * (saturate((preClipAlpha * input.color.a - _AlphaMin) / (_AlphaSoft + 0.2)) - 0.1) * 1.2;
                    normalTS = normalize(normalTS);

                    // Transform to world space
                    float3x3 TBN = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                    half3 normalWS = TransformTangentToWorld(normalTS, TBN, true);
                    half3 combinedNormals = normalize(input.normalWS + normalWS);

                    // Reflection calculation
                    float3 reflectionVector = reflect(-input.viewDirWS, combinedNormals);
                    reflectionVector.x = atan2(reflectionVector.x, reflectionVector.z) * 0.31831;
                    reflectionVector = reflectionVector * 0.5;
                    float2 reflectionUVs = reflectionVector.xy;
                    
                    float3 reflectionTex = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, reflectionUVs).rgb;

                    // Specular reflection
                    float desatReflection = dot(reflectionTex, float3(1, 1, 1)) * 0.333;
                    float3 spec = lerp(desatReflection, reflectionTex, _ReflectionSat);
                    float3 spec0 = spec;
                    float3 spec1 = spec0 * spec0 * spec0 * spec0;
                    spec = clamp(lerp(spec0, spec1, _SpecularColor.w * preClipAlpha), 0, 10);

                    float fresnel = 1 - dot(input.viewDirWS, combinedNormals) * _SpecularColor.w;
                    spec *= clamp(fresnel, 0.2, 1);
                #endif

                // Edge detection and darkening
                half edge = 1 - saturate(preClipAlpha * clippedAlpha);
                edge *= edge;
                edge = 1 - edge;
                edge = saturate(lerp(0.71, edge * edge, _EdgeDarken));

                // Edge alpha
                col.a *= saturate(lerp(1.25, _BaseColor.a, edge));

                #ifndef SPECULAR_REFLECTION
                    edge *= 2;
                #endif

                col.rgb *= lerp(min(col.rgb * col.rgb * col.rgb * 0.3, 1.0), 0.71, edge);

                // Apply base color and lighting
                col.rgb *= max(0, baseLighting * _BaseColor.rgb);

                #ifdef SPECULAR_REFLECTION
                    col.rgb += baseLighting * spec * _SpecularColor.rgb;
                #endif

                // Apply fog
                col.rgb = MixFog(col.rgb, input.fogFactor);

                return col;
            }
            ENDHLSL
        }

        // Shadow pass for URP
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}