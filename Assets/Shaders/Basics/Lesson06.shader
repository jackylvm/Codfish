Shader "Codfish/Basics/Lesson06/Surface"
{
    Properties
    {
        _Smoothness("Smoothness",Range(0,1)) = 0.5
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        void ConfigureSurface(Input IN, inout SurfaceOutputStandard surface)
        {
            surface.Albedo = float3(1, 0.8431, 0);
            surface.Smoothness = 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}