Shader "DesertDash/Amman Skybox"
{
    Properties
    {
        _ZenithColor ("Zenith Color", Color) = (0.12, 0.40, 0.72, 1)
        _HorizonColor ("Horizon Color", Color) = (0.72, 0.80, 0.86, 1)
        _GroundColor ("Ground Color", Color) = (0.52, 0.37, 0.23, 1)
        _SunColor ("Sun Color", Color) = (1.00, 0.78, 0.46, 1)
        _SunDirection ("Sun Direction", Vector) = (-0.3, 0.6, 0.7, 0)
        _SunSize ("Sun Size", Range(0.004, 0.05)) = 0.014
        _CloudAmount ("Cloud Amount", Range(0, 1)) = 0.38
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
            #pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            half4 _ZenithColor;
            half4 _HorizonColor;
            half4 _GroundColor;
            half4 _SunColor;
            float4 _SunDirection;
            half _SunSize;
            half _CloudAmount;

            v2f vert(appdata input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.direction = input.vertex.xyz;
                return output;
            }

            half4 frag(v2f input) : SV_Target
            {
                float3 direction = normalize(input.direction);
                half skyHeight = saturate(direction.y);
                half skyBlend = pow(skyHeight, 0.55h);
                half3 color = lerp(_HorizonColor.rgb, _ZenithColor.rgb, skyBlend);

                half groundBlend = saturate(-direction.y * 5.0h);
                color = lerp(color, _GroundColor.rgb, groundBlend);

                float3 sunDirection = normalize(_SunDirection.xyz);
                half sunFacing = saturate(dot(direction, sunDirection));
                half sunDisc = smoothstep(1.0h - _SunSize, 1.0h, sunFacing);
                half sunGlow = pow(sunFacing, 96.0h) * 0.42h;
                color += _SunColor.rgb * (sunDisc * 1.35h + sunGlow);

                half cloudBand = saturate(1.0h - abs(direction.y - 0.20h) * 5.0h);
                half cloudWaveA = sin(direction.x * 19.0h + direction.z * 7.0h + _Time.y * 0.025h);
                half cloudWaveB = sin(direction.z * 23.0h - direction.x * 5.0h - _Time.y * 0.018h);
                half cloudNoise = cloudWaveA * cloudWaveB * 0.5h + 0.5h;
                half cloudMask = smoothstep(0.67h - _CloudAmount * 0.25h, 0.82h, cloudNoise) * cloudBand;
                color = lerp(color, half3(0.94h, 0.91h, 0.84h), cloudMask * 0.28h);

                half haze = pow(saturate(1.0h - abs(direction.y) * 3.2h), 3.0h);
                color = lerp(color, _HorizonColor.rgb, haze * 0.30h);
                return half4(color, 1.0h);
            }
            ENDCG
        }
    }

    Fallback "Skybox/Procedural"
}
