// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Isometric/InstancedUnlit"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[Enum(Isometric,0,Dimetric,1,Military,2, Dimetric42x7,3)] Projection ("Projection", Int) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType"="TransparentCutout"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
				"DisableBatching" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha
			ZTest LEqual 
			

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma instancing_options assumeuniformscaling nolightmap nolightprobe
				#include "UnityCG.cginc"

				#define Identity float4x4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1)
				#define Isometric float4x4(0.70711,0.41187,0.57477,0.00000,0.00000,0.81285,-0.58248,0.00000,-0.70711,0.41187,0.57477,0.00000,0.00000,0.00000,0.00000,1.00000)
				#define Dimetric1x2 float4x4(0.70711,0.35355,0.61237,0.00000,0.00000,0.86603,-0.50000,0.00000,-0.70711,0.35355,0.61237,0.00000,0.00000,0.00000,0.00000,1.00000)
				#define Military float4x4(0.70711,0.50000,0.50000,0.00000,0.00000,0.70711,-0.70711,0.00000,-0.70711,0.50000,0.50000,0.00000,0.00000,0.00000,0.00000,1.00000)
				#define Dimetric42x7 float4x4(0.34202,0.32139,0.88302,0.00000,0.00000,0.93969,-0.34202,0.00000,-0.93969,0.11698,0.32139,0.00000,0.00000,0.00000,0.00000,1.00000)
				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;
				//isometric matrix
				float4x4 _isoMat;
				int Projection;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(IN);
					//Isometric rotation * Model * View * Projection
					float4x4 rotMat;
					if(Projection == 0)
						rotMat = Isometric;
					else if(Projection == 1)
						rotMat = Dimetric1x2;
					else if(Projection == 2)
						rotMat = Military;
					else if(Projection == 3)
						rotMat = Dimetric42x7;
					else
						rotMat = Identity;

					OUT.vertex = UnityObjectToClipPos(mul(rotMat, IN.vertex));
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}
			
				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _AlphaSplitEnabled;

				fixed4 SampleSpriteTexture(float2 uv)
				{
					fixed4 color = tex2D(_MainTex, uv);

	#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
	#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.rgb *= c.a;
					return c;
				}
			ENDCG
			}
		}
}