﻿Shader "Unlit/TowerBodyShader"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader{
		Tags{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}

		Blend SrcAlpha OneMinusSrcAlpha


		Cull off

		Pass{
			Stencil {
					Ref 2
					Comp always
					Pass replace
				}

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;


			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;

			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;

			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				fixed4 col = tex2D(_MainTex, i.uv);

				return col;
			}

			ENDCG
		}
	}
}