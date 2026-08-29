Shader "DesertDash/Fantasy Panoramic Sky"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Equirectangular Sky", 2D) = "grey" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _Exposure ("Exposure", Range(0, 4)) = 1
        _Rotation ("Rotation", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Tint;
            half _Exposure;
            half _Rotation;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            v2f vert(appdata input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.direction = input.vertex.xyz;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float3 direction = normalize(input.direction);
                float rotation = radians(_Rotation);
                float cosine = cos(rotation);
                float sine = sin(rotation);
                float rotatedX = direction.x * cosine - direction.z * sine;
                float rotatedZ = direction.x * sine + direction.z * cosine;
                direction.x = rotatedX;
                direction.z = rotatedZ;

                float2 uv;
                uv.x = atan2(direction.z, direction.x) * 0.159154943 + 0.5;
                uv.y = 0.5 - asin(clamp(direction.y, -1.0, 1.0)) * 0.318309886;
                fixed3 sky = tex2D(_MainTex, uv).rgb * _Tint.rgb * _Exposure;
                return fixed4(sky, 1.0);
            }
            ENDCG
        }
    }

    Fallback Off
}
