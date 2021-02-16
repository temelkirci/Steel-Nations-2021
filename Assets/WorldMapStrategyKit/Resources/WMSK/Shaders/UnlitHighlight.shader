Shader "WMSK/Unlit Highlight" {
Properties {
    _Color ("Color", Color) = (1,1,1,0.5)
}
SubShader {
    Tags {
        "Queue"="Geometry+5"
        "IgnoreProjector"="True"
        "RenderType"="Transparent"
    }
			Cull Off		
			ZWrite Off		
			Offset -5, -5
			Color [_Color]
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off
			Pass {
			}
	}	
}
