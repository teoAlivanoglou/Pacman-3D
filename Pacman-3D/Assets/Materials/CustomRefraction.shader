// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CustomRefraction"
{
	Properties
	{
		_BrushedMetalNormal("BrushedMetalNormal", 2D) = "bump" {}
		_Distortion("Distortion", Range( 0 , 1)) = 0.292
		_Tint("Tint", Color) = (0,0,0,0)
		_RotateSpeed("RotateSpeed", Range( -1 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float4 _Tint;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _BrushedMetalNormal;
		uniform float _RotateSpeed;
		uniform float _Distortion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos40 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale40 = -1.0;
			#else
			float scale40 = 1.0;
			#endif
			float halfPosW40 = ase_screenPos40.w * 0.5;
			ase_screenPos40.y = ( ase_screenPos40.y - halfPosW40 ) * _ProjectionParams.x* scale40 + halfPosW40;
			ase_screenPos40.xyzw /= ase_screenPos40.w;
			float2 uv_TexCoord44 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float mulTime45 = _Time.y * _RotateSpeed;
			float cos43 = cos( mulTime45 );
			float sin43 = sin( mulTime45 );
			float2 rotator43 = mul( uv_TexCoord44 - float2( 0.5,0.5 ) , float2x2( cos43 , -sin43 , sin43 , cos43 )) + float2( 0.5,0.5 );
			float4 screenColor8 = tex2D( _GrabTexture, ( (ase_screenPos40).xy + (( UnpackNormal( tex2D( _BrushedMetalNormal, rotator43 ) ) * _Distortion * _Distortion )).xy ) );
			o.Emission = ( _Tint * screenColor8 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14001
1963;122;1612;732;1402.46;-32.89424;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;46;-1663.46,394.8942;Float;False;Property;_RotateSpeed;RotateSpeed;3;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;45;-1354.145,382.8734;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-1425.145,215.8734;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;43;-1164.145,327.8734;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;29;-855.48,221.599;Float;True;Property;_BrushedMetalNormal;BrushedMetalNormal;0;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Misc/PortalNormal.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-878.9375,451.5321;Float;False;Property;_Distortion;Distortion;1;0;0.292;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-441.6739,287.2988;Float;False;3;3;0;FLOAT3;0.0,0,0;False;1;FLOAT;0.0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GrabScreenPosition;40;-447.4607,49.29217;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;36;-248.5805,285.0987;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;39;-191.7806,65.19897;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;36.62508,137.2995;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT2;0.0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenColorNode;8;224.0004,85.8997;Float;False;Global;_ScreenGrab0;Screen Grab 0;-1;0;Object;-1;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;194.8546,-135.1266;Float;False;Property;_Tint;Tint;2;0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;587.8546,7.873444;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;866.7999,-57.8;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;CustomRefraction;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Translucent;0.5;True;True;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;46;0
WireConnection;43;0;44;0
WireConnection;43;2;45;0
WireConnection;29;1;43;0
WireConnection;32;0;29;0
WireConnection;32;1;31;0
WireConnection;32;2;31;0
WireConnection;36;0;32;0
WireConnection;39;0;40;0
WireConnection;30;0;39;0
WireConnection;30;1;36;0
WireConnection;8;0;30;0
WireConnection;42;0;41;0
WireConnection;42;1;8;0
WireConnection;0;2;42;0
ASEEND*/
//CHKSM=E423B987AC2ED0859B2C6DD41786C4B8CE9AD91B