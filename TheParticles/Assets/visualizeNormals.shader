Shader "Debug/visualizeNormals" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input {
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.worldNormal;
			o.Emission = IN.worldNormal;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
