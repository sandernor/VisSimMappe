Shader "Custom/IndGPUInstSH"
{
    Properties
    {
        _FarColor("Far color", Color) = (.2, .2, .2, 1)
    }
        SubShader
    {
        Pass
        {
            Tags
            {
                "RenderType" = "Opaque"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            /*#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"*/

            float4 _FarColor;

            StructuredBuffer<float4> position_buffer_1;
            //StructuredBuffer<float4> position_buffer_2;
            float4 color_buffer[3];

            struct attributes
            {
                float3 normal : NORMAL;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct varyings
            {
                float4 vertex : SV_POSITION;
                float3 color : TEXCOORD3;
            };

            varyings vert(attributes v, const uint instance_id : SV_InstanceID)
            {
                const float3 pos = position_buffer_1[instance_id];
                const float3 color = _FarColor;

                varyings o;
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
                o.color = color;

                return o;
            }

            half4 frag(const varyings i) : SV_Target
            {
                const float3 lighting = 11.7;
                return half4(i.color * lighting, 1);;
            }
            ENDCG
        }
    }
}