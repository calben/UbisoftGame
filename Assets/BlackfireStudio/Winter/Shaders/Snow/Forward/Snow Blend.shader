﻿// Blackfire Studio
// Matthieu Ostertag

Shader "Blackfire Studio/Snow/Forward/Snow Advanced Blend" {
	Properties {
		_Ramp				("Shade (RGB)", 2D) 						= "white" {}
		_RampPower			("Shade Intensity", Range (0.0, 1.0))		= 1.0
		_MainTex			("Diffuse (RGB)", 2D) 						= "white" {}
		_GlitterTex			("Specular (RGB)", 2D)						= "black" {}
		_Specular			("Specular Intensity", Range (0.0, 5.0))	= 1.0
		_Shininess			("Shininess", Range (0.01, 1.0))			= 0.08
		_Aniso				("Anisotropic Mask", Range (0.0, 1.0))		= 0.0
		_Glitter			("Anisotropic Intensity", Range (0.0, 15.0))= 0.5
		_BumpTex			("Normal (RGB)", 2D)						= "bump" {}
		_DepthTex			("Depth (R) Spread (G)", 2D)				= "white" {}
		_Depth				("Translucency", Range(-2.0, 1.0))			= 1.0
		_Coverage			("Coverage", Range (-0.01, 1.001))			= 0.5
		_SubNormal			("SubNormal (RGB)", 2D)						= "bump" {}
		_Spread				("Spread", Range (0.0, 1.0))				= 1.0
		_Smooth				("Smooth", Range (0.01, 5.0))				= 0.5
		_Transition			("Transition", Range (-1.0, 1.0))			= 0.5
		_TransitionSmooth	("Transition Smoothness", Range (0.0, 2.0))	= 0.5
		_Direction			("Direction", Vector)						= (0, 1, 0)
	}
	
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue" = "AlphaTest" "RenderType" = "Transparent" "IgnoreProjector"="True" }
		Offset 0, -1
		LOD 400
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface SnowSurface Snow fullforwardshadows
		
		#ifdef SHADER_API_OPENGL	
			#pragma glsl
		#endif
		#pragma exclude_renderers d3d11
		
		#define SNOW_BLEND_ADVANCED
		
		#include "../SnowInputs.cginc"
		#include "../SnowLighting.cginc"
		#include "../SnowSurface.cginc"

		ENDCG
	}
	FallBack "Diffuse"
}