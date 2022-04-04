Shader "Custom/Blob" {
	Properties {
		_Color ("Color", COLOR) = (1,1,1,1)
		_Spec ("Specular Color", COLOR) = (.5, .5, .5, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_NormalStrength ("Normal Map Strength", Float) = 1
		_Glossiness ("Smoothness", RANGE(0,1)) = 0.5
		_Refraction ("Refraction", FLOAT) = .1
		_Transition ("Transition", FLOAT) = 0
		_Morph ("Morph Vector", VECTOR) = (0, 0, 0, 0)
		_Dampen ("Dampen", RANGE(0, 1)) = .1
		_Body ("Body Thickness", RANGE(0, 1)) = 0
		_Blobbiness("Blobbiness", FLOAT) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		
		Grabpass {}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows addshadow vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _NormalMap, _GrabTexture;

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord: TEXCOORD0;
			float4 tangent: TANGENT;
		};

		struct Input {
			float2 uv_MainTex;
			float2 grabPos;
			float3 normal;
			float3 tangent;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color, _Spec;
		float _Refraction, _NormalStrength, _Transition, _Dampen, _Body, _Blobbiness;
		float4 _Morph;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void vert(inout appdata v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float4 osMorph = mul(unity_WorldToObject, _Morph);
			float3	normalizedMorph = normalize(osMorph.xyz),
					normalizedVert = normalize(v.vertex.xyz);

			float alpha = (dot(normalizedVert, normalizedMorph) + 1 + _Dampen) / (2 + _Dampen * 2);
			alpha = pow(alpha, pow(2, -_Transition));
			float alpha2 = 1 - abs(alpha - .5) * 2;
			alpha2 *= 1 - 1 / (1 + dot(osMorph, osMorph));
			alpha2 = pow(alpha2, pow(2, _Blobbiness));
			alpha2 *= 1 - _Body;
			float3 shrink = cross(normalizedMorph, cross(normalizedMorph, normalizedVert));
			v.vertex.xyz -= alpha2 * dot(v.vertex.xyz, shrink) * shrink;
			float4 v2 = osMorph + v.vertex;
			v.vertex = float4(lerp(v.vertex, v2, alpha).xyz, 1);

			//v.vertex += float4(0, alpha, 0, 0);
			float4 tempPos = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex));
			o.grabPos = tempPos.xy / tempPos.w;
			o.normal = UnityObjectToViewPos(v.normal);
			o.tangent = UnityObjectToViewPos(v.tangent);
		}

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 color = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = color * color.a;
			o.Smoothness = _Glossiness;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			o.Normal = normalize(float3(o.Normal.xy * _NormalStrength, 1));
			o.Specular = _Spec.rgb;
			float3 bitangent = cross(IN.tangent, IN.normal);
			float3 normal = normalize(o.Normal.x * IN.tangent + o.Normal.y * bitangent + o.Normal.z * IN.normal);
			o.Emission = (1 - o.Specular) * tex2D (_GrabTexture, IN.grabPos - normal.xy * _Refraction) * color * (1 - color.a);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
