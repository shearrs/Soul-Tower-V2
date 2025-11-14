Shader "Custom/Waves"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float2 uv = IN.uv;
                uv = 2.0 * uv - 1.0;
                float radius = length(uv);
                float3 position = IN.positionOS.xyz;
                position.y += 0.5 * sin(12 * radius - (4.0 * _Time.y));

                OUT.positionHCS = TransformObjectToHClip(position);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                uv = 2.0 * uv - 1.0;

                float radius = length(uv);
                float colorValue = 0.5 * sin(12 * radius - (4.0 * _Time.y));

                half4 color = half4(colorValue, colorValue, colorValue, 1.0);

                return color;
            }
            ENDHLSL
        }
    }
}
