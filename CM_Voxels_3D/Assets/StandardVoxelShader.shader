Shader "Voxel/StandardVoxelShader"
{
    Properties
    {
        _MainTex ("Voxel Texture Atlas", 2D) = "white" {}
    }

    SubShader
    {
        Tags {
			"RenderType"="TransparentCutout"
			"Queue"="AlphaTest"
			"IgnoreProjector"="True"
		}
        LOD 100
		Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 colour : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 colour : COLOR;
            };

            sampler2D _MainTex;
			float SkyLightLevel;
			float4 SkyLightColour;
			float MinLightLevel;
			float MaxLightLevel;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.colour = v.colour;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				// Alpha clipping
				clip(col.a - 1);

				// Lighting
				float shade = ((MaxLightLevel - MinLightLevel) * (SkyLightLevel)) + MinLightLevel;
				shade *= i.colour.a;
				shade = clamp(shade, MinLightLevel, MaxLightLevel);

				//float4 minLightTint = float4(i.colour.r * i.colour.a * 0.5, i.colour.g * i.colour.a * 0.5, i.colour.b * i.colour.a * 0.5, 1);

				col = lerp(float4(0, 0, 0, 1), col, shade);

				float illuminationR = i.colour.r + (SkyLightColour.r * shade) / (1 + shade);
				float illuminationG = i.colour.g + (SkyLightColour.g * shade) / (1 + shade);
				float illuminationB = i.colour.b + (SkyLightColour.b * shade) / (1 + shade);

				col.r = min(col.r + (0.5 * MinLightLevel * illuminationR), MaxLightLevel);
				col.g = min(col.g + (0.5 * MinLightLevel * illuminationG), MaxLightLevel);
				col.b = min(col.b + (0.5 * MinLightLevel * illuminationB), MaxLightLevel);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
