Shader "Agent/Dissolve"
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
			#pragma require 2darray

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

			struct ParticlesDisolve
			{
				float3 originVertex;
				float3 position;
				float3 uv;
				float life;
			};

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float3 uv               : TEXCOORD0;
				uint instance_id		: SV_InstanceID;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 uv0			            : TEXCOORD0; // xy: uv0 zw: tosample from
                float4 vertex					: SV_POSITION;
				half4 particleColor				: COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

			Texture2DArray<float4> _ObjTexArray;
			StructuredBuffer<ParticlesDisolve> disolveBuffer;

			SamplerState sampler_ObjTexArray;
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);


				VertexPositionInputs particlesPosition = GetVertexPositionInputs(disolveBuffer[input.instance_id].position);
				output.vertex = particlesPosition.positionCS;

				output.uv0.xyz = disolveBuffer[input.instance_id].uv;

				float alphaLevel = clamp( disolveBuffer[input.instance_id].life * 0.5f , 0, 1);
				output.uv0.w = alphaLevel;
				output.particleColor = _Colors[disolveBuffer[input.instance_id].uv.z];
				output.particleColor.a = alphaLevel;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
			
                half4 uv = input.uv0;
				half4 texColor = _ObjTexArray.Sample(sampler_ObjTexArray, uv.xyz);
				texColor = (texColor * (1-_Solid[uv.z])) + (input.particleColor*_Solid[uv.z]);
				texColor.a = input.particleColor.a;
                return texColor;
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
