Shader "Nyx/OutlinedDifuuse"
{

	// Shader properties
    Properties
    {
		_Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1.0, 1.0, 0.0, 1.0)
		_OutlineThickness("Outline Thickness", Range(0.0, 10.0)) = 0.1
    }

	// CG includes
	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 position : POSITION;
		float3 normal : NORMAL;
	};

	float _OutlineThickness;
	float4 _OutlineColor;

	// Extrudes verts
	v2f vert(appdata inVert)
	{
		v2f outVert;

		inVert.vertex.xyz *= 1.0 + _OutlineThickness;
		outVert.position = UnityObjectToClipPos(inVert.vertex);

		return outVert;
	}

	ENDCG

    SubShader
    {
		// Ensure outline renders on top of transparent objects
		Tags { "Queue" = "Transparent" }

		// Renders the outline
		Pass
		{
			// Render outline on top
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Colors the vertice to the outline color
			half4 frag(v2f i) : COLOR
			{
				half4 outColor = _OutlineColor;
				return _OutlineColor;
			}
			ENDCG
		}

		// Normal render
		Pass
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}

			Lighting On

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
		}
    }
}
