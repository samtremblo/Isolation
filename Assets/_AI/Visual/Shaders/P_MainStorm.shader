Shader "Agent/MainStorm"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5
        [Toggle] _SampleGI("SampleGI", float) = 0.0
        _BumpMap("Normal Map", 2D) = "bump" {}

		_Color1("Color", Color) = (1, 1, 1, 1)
	    _Color2("Color", Color) = (1, 1, 1, 1)
        // BlendMode
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("Src", Float) = 1.0
        [HideInInspector] _DstBlend("Dst", Float) = 0.0
        [HideInInspector] _ZWrite("ZWrite", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "IgnoreProjectors" = "True" "RenderPipeline" = "LightweightPipeline" }
        LOD 100
		//Blend SrcAlpha one
        Blend [_SrcBlend][_DstBlend]
        //ZWrite [_ZWrite]
		ZWrite Off
        Cull [_Cull]

        Pass
        {
            Name "MainPass"
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ _SAMPLE_GI _SAMPLE_GI_NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            // Lighting include is needed because of GI
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "../Librairies/UnlitInput.hlsl"

			struct Particles
			{
				float colorLife;
				float3 position;
				float3 localPos;
				float life;
			};


            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                float2 lightmapUV       : TEXCOORD1;
                float3 normalOS         : NORMAL;
                float4 tangentOS        : TANGENT;
				uint vertex_id			: SV_VertexID;
				uint instance_id		: SV_InstanceID;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float3 uv0AndFogCoord           : TEXCOORD0; // xy: uv0, z: fogCoord
				#if defined(_SAMPLE_GI) || defined(_SAMPLE_GI_NORMALMAP)
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
                half3 normal                    : TEXCOORD2;
				#if defined(_SAMPLE_GI_NORMALMAP)
                half3 tangent                   : TEXCOORD3;
                half3 bitangent                 : TEXCOORD4;
				 #endif
				#endif
                float4 vertex					: SV_POSITION;
				half4 particleColor				: COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };


			StructuredBuffer<Particles> particleBuffer;

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                //VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                //output.vertex = vertexInput.positionCS;
				VertexPositionInputs particlesPosition = GetVertexPositionInputs(particleBuffer[input.instance_id].position);
				output.vertex = particlesPosition.positionCS;

                //output.uv0AndFogCoord.xy = TRANSFORM_TEX(input.uv, _MainTex);
                //output.uv0AndFogCoord.z = ComputeFogFactor(particlesPosition.positionCS.z);

				#if defined(_SAMPLE_GI) || defined(_SAMPLE_GI_NORMALMAP)
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normal = normalInput.normalWS;
				#if defined(_SAMPLE_GI_NORMALMAP)
                output.tangent = normalInput.tangentWS;
                output.bitangent = normalInput.bitangentWS;
				#endif
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normal, output.vertexSH);
				#endif

				float alphaLevel = particleBuffer[input.instance_id].life * 0.25f;
				//float colorlerp = clamp( distance(particleBuffer[input.instance_id].position, attractor)/2+0.3, 0, 1);
				float colorlerp = 1- particleBuffer[input.instance_id].colorLife;
				float4 color = _Color1* (1 - colorlerp) + _Color2*(colorlerp);
				output.particleColor = half4(color.x, color.y, color.z, alphaLevel);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
			/*
                half2 uv = input.uv0AndFogCoord.xy;
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half3 color = texColor.rgb * _Color.rgb;
                half alpha = texColor.a * _Color.a;
                AlphaDiscard(alpha, _Cutoff);

				#ifdef _ALPHAPREMULTIPLY_ON
                color *= alpha;
				#endif


				#if defined(_SAMPLE_GI) || defined(_SAMPLE_GI_NORMALMAP)
				 #if defined(_SAMPLE_GI_NORMALMAP)
                half3 normalTS = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv).xyz;
                half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangent, input.bitangent, input.normal));
				#else
                half3 normalWS = input.normal;
				#endif
                normalWS = NormalizeNormalPerPixel(normalWS);
                color *= SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
				#endif
                color = MixFog(color, input.uv0AndFogCoord.z);
				*/
				
                return input.particleColor;
            }
            ENDHLSL
        }
		/*
        Pass
        {
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "../Librairies/UnlitInput.hlsl"
            #include "../Librairies/DepthOnlyPass.hlsl"
            ENDHLSL
        }*/

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex LightweightVertexMeta
            #pragma fragment LightweightFragmentMetaUnlit

            #include "../Librairies/UnlitInput.hlsl"
            #include "../Librairies/UnlitMetaPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
    CustomEditor "UnityEditor.Experimental.Rendering.LightweightPipeline.UnlitShaderGUI"
}
