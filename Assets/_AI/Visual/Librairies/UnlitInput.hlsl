#ifndef LIGHTWEIGHT_UNLIT_INPUT_INCLUDED
#define LIGHTWEIGHT_UNLIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
half4 _Color;
half _Cutoff;
half _Glossiness;
half _Metallic;
float4 _Color1;
float4 _Color2;
float _Solid[10];
Texture2DArray<float4> _ObjTexArray_ST;
float4 _Colors[10];
CBUFFER_END

#endif
