Shader "ExperimentalToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
    //            fixed4 col = tex2D(_MainTex, i.uv);
				//float3 lightDir = _WorldSpaceLightPos0.xyz;
				//float intensity = max(0, dot(lightDir, i.normal));

				float3 diffuseLight = _LightColor0.xyz;

				_Color = float4(.5, .4, .3, 1);
				//float3 ambientLight = float3(.2, .35, .4);

				return float4( _LightColor0.xyz, 0);
            }
            ENDCG
        }
    }
}
