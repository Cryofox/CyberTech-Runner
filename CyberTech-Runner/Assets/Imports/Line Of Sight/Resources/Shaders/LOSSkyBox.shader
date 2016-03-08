Shader "Hidden/Line Of Sight Skybox"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "Queue"="Background" "RenderType"="Background" }

		Cull Off ZWrite Off Fog { Mode Off }

		CGINCLUDE
		#include "UnityCG.cginc"

		uniform float _FarPlane;

		struct appdata_t
		{
			float4 vertex : POSITION;
		};
		struct v2f
		{
			float4 vertex : POSITION;
		};
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			return o;
		}
		float4 skybox_frag (v2f i)
		{
			float farPlane = _FarPlane;
			return farPlane;
		}
		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			float4 frag (v2f i) : COLOR { return skybox_frag(i); }
			ENDCG
		}
	}
}