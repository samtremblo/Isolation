Shader "Hidden/AspectBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AspectRatio ("Aspect Ratio", float) = 1
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			    UNITY_VERTEX_OUTPUT_STEREO
            };
			
            //sampler2D _MainTex;
			half4 _MainTex_ST;
            float _AspectRatio;
            fixed4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                
                //float aspectXScale = max(1/_AspectRatio, 1);
                //float aspectYScale = max(_AspectRatio, 1);
                
                //float aspectXOffset = -(aspectXScale-1) / 2;
                //float aspectYOffset = -(aspectYScale-1) / 2;
                
				o.uv = v.uv;//float2(v.uv.x * aspectXScale + aspectXOffset, v.uv.y * aspectYScale + aspectYOffset);
                return o;
            }

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				//UNITY_SETUP_INSTANCE_ID(i);
				fixed4 original = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
				//fixed4 original= tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
                if(i.uv.x < 0 || i.uv.x > 1 || i.uv.y < 0 || i.uv.y > 1)
                    return _BackgroundColor;
				original = 1 - original;
				return original;
            }
            ENDCG
        }
    }
}
