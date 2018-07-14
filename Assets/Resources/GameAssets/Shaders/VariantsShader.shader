// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/VariantsShader"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#pragma  shader_feature RED GREEN BLUE 

			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,1);
				#ifdef RED
				col+=fixed4(1,0,0,1);
				#endif

				#ifdef GREEN
				col+=fixed4(0,1,0,1);
				#endif

				#ifdef BLUE
				col+=fixed4(0,0,1,1);
				#endif

				return col;
			}
			ENDCG
		}
	}
	CustomEditor "ColorsGUI"////编辑ShaderGUI
}
