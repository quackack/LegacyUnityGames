Shader "Storm/RainFallAndParralax" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Bump ("Bump", 2D) = "bump" {}
		_SpecMag ("Amount of specular", Range(0, 2)) = .5
		_SpecPower ("Rate of change between no specular and all specular", Range(2, 200)) = 4
		_HeightMap ("Height map, ambient map, and wet height of Item", 2D) = "black" {}
		_HeightMid ("Middle height in Height Map", Range(0, 1)) = .5
		_HeightMag ("Magnitude of height", Range(0, .2)) = .05
		_AmbientMag ("Amount of ambient light recieved", Range(0, 2)) = 1
		_FlowingTex ("Texture of Build up", 2D) = "white" {}
		_FlowingDir ("Direction and speed of flow", Vector) = (0, 1, 0, 0)
		_FlowingMag ("How much has Built up based on Normal", Vector) = (0, 0, .5, 0)
		_FlowingNorm ("Normal of Build Up", 2D) = "bump" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        Pass {
 
            Tags { "LightMode"="ForwardBase" }
            Cull Back
            Lighting On
 
            CGPROGRAM
			// Upgrade NOTE: excluded shader from Xbox360; has structs without semantics (struct v2f members lightDirection)
			#pragma exclude_renderers xbox360
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            uniform float4 _LightColor0;
 
            sampler2D _MainTex;
            sampler2D _Bump;
            float _SpecMag;
            float _SpecPower;
            sampler2D _HeightMap;
            float _HeightMid;
            float _HeightMag;
            float _AmbientMag;
            float _ScatterMag;
            float4 _MainTex_ST;
            float4 _Bump_ST;
            
            float4 _FlowingDir;
            float4 _FlowingMag;
            sampler2D _FlowingTex;
            sampler2D _FlowingNorm;
            
 
            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 tangent : TANGENT;
 
            };
 
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 lightDir;
                float3 viewDir;
                float3 lightMag;
                float3 lightDir2;
                float3 lightMag2;
                float2 uv3;
                float BuildMag;
            };
 
            v2f vert (a2v v)
            {
                v2f o;
                
                o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex); 
                o.uv2 = TRANSFORM_TEX (v.texcoord, _Bump);
                
                TANGENT_SPACE_ROTATION;
                float4x4 InvModel = _World2Object;
 
                o.lightDir = normalize(mul(rotation, ObjSpaceLightDir(v.vertex)));
                o.viewDir = mul(rotation,normalize(ObjSpaceViewDir(v.vertex)));
                
                float DirSqr = dot(o.lightDir, o.lightDir);
                float Bright = 1.0 / (1.0 + DirSqr);
                o.lightMag = Bright*_LightColor0.rgb;
                
                
                float4 WLPos = float4(unity_4LightPosX0[0], unity_4LightPosY0[0], unity_4LightPosZ0[0], 1.0);
                float3 TLPos = mul(rotation, mul(InvModel, WLPos).rgb-v.vertex.rgb);
                DirSqr = dot(TLPos, TLPos);
                Bright = 1.0 / (1.0 + DirSqr);
                float3 OtherLights = normalize(TLPos)*Bright;
                float3 OtherLightMag = Bright*unity_LightColor[0].rgb;
                
                for (int light = 1; light < 4; light++) {
                	WLPos = float4(unity_4LightPosX0[light], unity_4LightPosY0[light], unity_4LightPosZ0[light], 1.0);
                	TLPos = mul(rotation, mul(InvModel, WLPos).rgb-v.vertex.rgb);
                	DirSqr = dot(TLPos, TLPos);
                	Bright = 1.0 / (1.0 + DirSqr);
                	OtherLightMag += Bright*unity_LightColor[light].rgb;
                	OtherLights += normalize(TLPos)*Bright;
                }
                
                o.lightDir2 = normalize(OtherLights);
                o.lightMag2 = OtherLightMag;
                
                
                float4 WorldCenter = mul(InvModel, float4(0, 0, 0, 1));
                float4 FlowDir = mul(InvModel, _FlowingDir) - WorldCenter;
                float3 TanFlowDir = mul(rotation, FlowDir.rgb);
                o.uv3 = o.uv2+TanFlowDir.xy*_Time.yy;
                
                o.BuildMag = saturate(dot(mul(InvModel, _FlowingMag)-WorldCenter, v.normal));
                
                return o;
            }
 
            float4 frag(v2f i) : COLOR 
            {
            	float4 UVRaw = float4(i.uv.x, i.uv.y, i.uv2.x, i.uv.y);
            	
            	float4 HtRaw = tex2D(_HeightMap, UVRaw.xy);
            	float4 ActHeight = float4(_HeightMag*(HtRaw.r-_HeightMid), HtRaw.g*_AmbientMag, 1-HtRaw.b, 1);
            	
            	float4 UVFin = UVRaw + ActHeight.x*i.viewDir.xyxy;
            
                float4 MyTex = tex2D (_MainTex, UVFin.xy); 
                float3 MyNorm =  UnpackNormal(tex2D (_Bump, UVFin.zw));
                
                
                float BuildMag = saturate(i.BuildMag*ActHeight.z);
                float InvBuild = 1 - BuildMag;
                float4 BuildTex = tex2D(_FlowingTex, i.uv3);
                float3 BuildNorm = UnpackNormal(tex2D(_FlowingNorm, i.uv3));
                
                float4 FinTex = MyTex*InvBuild+BuildTex*BuildMag;
                float3 FinNorm = MyNorm+BuildNorm*BuildMag;
                
                
                float3 Ambiant = UNITY_LIGHTMODEL_AMBIENT.xyz*ActHeight.y;
 

                float F2LAngle = saturate (dot (FinNorm, i.lightDir));
                float3 Albedo1 = i.lightMag * F2LAngle;
                

                float F2LAngle2 = saturate (dot (FinNorm, i.lightDir2));
                float3 Albedo2 = i.lightMag2.rgb * F2LAngle2;
                
                
                float3 Albedo = Albedo1+Albedo2;
                
				//Normal if this was a perfect reflection
                float3 LVRefNorm = normalize(i.viewDir + i.lightDir);
                float BaseReflect = saturate (dot (FinNorm, LVRefNorm));
                float3 Reflect = pow(BaseReflect, _SpecPower)*i.lightMag;
                
                LVRefNorm = normalize(i.viewDir + i.lightDir2);
                BaseReflect = saturate (dot (FinNorm, LVRefNorm));
                float3 Reflect2 = pow(BaseReflect, _SpecPower)*i.lightMag2;
                
                float4 FinalColor = float4(0, 0, 0, 1);
                FinalColor.rgb = (Albedo*2 + Ambiant)*FinTex.rgb + (Reflect + Reflect2)*_SpecMag;
                return FinalColor;
 
            }
 
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
