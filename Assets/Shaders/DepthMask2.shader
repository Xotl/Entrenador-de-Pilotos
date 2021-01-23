Shader "Custom/DepthMask2" {
 	SubShader
    {
        Tags {"Queue" = "Geometry-1" }
        Lighting Off
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0     
        }
    }
}