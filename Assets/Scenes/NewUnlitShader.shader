Shader "Unlit/NewUnlitShader"
{
	Properties{
		  _MainTex("Texture", 2D) = "white" { }
	}

	SubShader{
		////////////////////////////////////////////////////////////
		// Pass #1 

		Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}


		CGPROGRAM

		#pragma target 3.0
		#pragma surface surf BlinnPhong

		struct Input {
		   float2 uv_MainTex;
		};

		sampler2D _MainTex;

		void surf(Input IN, inout SurfaceOutput o) {
		   o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		}

		ENDCG

			////////////////////////////////////////////////////////////
			// Pass #2 
			ZTest Greater

			CGPROGRAM

			#pragma target 3.0
			#pragma surface surf Lambert //lambertian reflectance lighting version
			//#pragma surface surf NoLighting //no lighting version

			struct Input {
			  float4 color : COLOR;
			};

		/*
		//Lambertian reflectance lighting version
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		*/

		 void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = float3(1.0, 0.0, 0.5);
			o.Alpha = 0;
		 }

		 ENDCG
	}

	Fallback "Diffuse"
}