Shader "Codfish/Basics/Lesson06/Surface"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1.0, 0.64706, 0.0)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma editor_sync_compilation

        #pragma target 4.5

        #include "Lesson06.hlsl"

        struct Input
        {
            float3 worldPos;
        };

        float4 _BaseColor;
        float _Smoothness;

        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
        {
            surface.Albedo = _BaseColor.rgb;
            surface.Smoothness = _Smoothness;
        }
        ENDCG
    }

    FallBack "Diffuse"
}