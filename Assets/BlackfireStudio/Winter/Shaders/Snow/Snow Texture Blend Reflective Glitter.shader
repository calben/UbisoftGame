﻿// Blackfire Studio
// Matthieu Ostertag

Shader "Blackfire Studio/Snow/Reflective/Snow Texture Blend Glitter" {
	Properties {
		_Ramp				("Shade (RGB)", 2D) 						= "white" {}
		_RampPower			("Shade Intensity", Range (0.0, 1.0))		= 1.0
		_MainTex			("Diffuse (RGB) Coverage (A)", 2D) 			= "white" {}
		_Specular			("Specular Intensity", Range (0.0, 5.0))	= 1.0
		_Shininess			("Shininess", Range (0.01, 1.0))			= 0.08
		_Aniso				("Anisotropic Mask", Range (0.0, 1.0))		= 0.0
		_Glitter			("Anisotropic Intensity", Range (0.0, 15.0))= 0.5
		_GlitterTex			("Noise (RGB)", 2D)							= "bump" {}
		_SpecularColor		("Glitter (RGB) Interpolator (A)", Color)	= (1, 1, 1, 0)
		_Speed				("Glitter Speed", Range (0.0, 10.0))		= 3.0
		_Density			("Glitter Density", Range (1.25, 0.0))		= 0.95
		_DensityStatic		("Glitter Static Density", Range (5.0, 1.0))= 4
		_Power				("Glitter Power", Range (0.0, 1.0))			= 0.2
		_BumpTex			("Normal (RGB)", 2D)						= "bump" {}
		_DepthTex			("Depth (R) Spread (G)", 2D)				= "white" {}
		_Depth				("Translucency", Range(-2.0, 1.0))			= 1.0
		_SubNormal			("SubNormal (RGB)", 2D)						= "bump" {}
		_Spread				("Spread", Range (0.0, 1.0))				= 1.0
		_Smooth				("Smooth", Range (0.01, 5.0))				= 0.5
		_Transition			("Transition", Range (-1.0, 1.0))			= 0.5
		_TransitionSmooth	("Transition Smoothness", Range (0.0, 2.0))	= 0.5
		_Cube				("Cubemap (RGB)", CUBE)						= "" {}
		_Reflection			("Reflection Intensity", Range(0.0, 1.0))	= 0.5
		_Falloff			("Reflection Falloff", Range(0.1, 3.0))		= 0.5
	}
	
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector"="True" }
		Offset 0, -1
		ZWrite Off
		LOD 400
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface SnowSurface Snow alpha
		
		#ifdef SHADER_API_OPENGL	
			#pragma glsl
		#endif
		
		#define SNOW_BLEND_TEXTURE
		#define SNOW_REFLECTION
		#define SNOW_GLITTER
		
		#include "SnowCore.cginc"
		#include "SnowInputs.cginc"
		#include "SnowLighting.cginc"
		#include "SnowSurface.cginc"

		ENDCG
	}
	FallBack Off
}