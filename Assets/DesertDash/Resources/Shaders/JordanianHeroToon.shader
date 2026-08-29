Shader "DesertDash/Lizard Runner Toon"
{
    Properties
    {
        _MainTex ("Base Color", 2D) = "white" {}
        _BaseColor ("Base Tint", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.16, 0.22, 0.34, 1)
        _RimColor ("Rim Color", Color) = (0.34, 0.78, 0.92, 1)
        _Saturation ("Saturation", Range(0, 2)) = 1.34
        _Contrast ("Contrast", Range(0.5, 1.5)) = 1.08
        _Brightness ("Brightness", Range(0.5, 1.5)) = 1.04
        _LightSteps ("Light Steps", Range(2, 5)) = 3
        _RimPower ("Rim Power", Range(1, 8)) = 3.6
        _RimStrength ("Rim Strength", Range(0, 1)) = 0.24
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPosition : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _BaseColor;
            half4 _ShadowColor;
            half4 _RimColor;
            half _Saturation;
            half _Contrast;
            half _Brightness;
            half _LightSteps;
            half _RimPower;
            half _RimStrength;

            v2f vert(appdata input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.worldNormal = UnityObjectToWorldNormal(input.normal);
                output.worldPosition = mul(unity_ObjectToWorld, input.vertex).xyz;
                UNITY_TRANSFER_FOG(output, output.position);
                return output;
            }

            half3 GradeColor(half3 color)
            {
                half luminance = dot(color, half3(0.2126h, 0.7152h, 0.0722h));
                color = lerp(luminance.xxx, color, _Saturation);
                color = (color - 0.5h) * _Contrast + 0.5h;
                return saturate(color * _Brightness);
            }

            half4 frag(v2f input) : SV_Target
            {
                half4 sampleColor = tex2D(_MainTex, input.uv) * _BaseColor;
                half3 baseColor = GradeColor(sampleColor.rgb);
                half3 normal = normalize(input.worldNormal);
                half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                half diffuse = saturate(dot(normal, lightDirection));
                half steps = max(2.0h, floor(_LightSteps));
                half celLight = floor(diffuse * steps) / max(1.0h, steps - 1.0h);
                celLight = saturate(celLight);

                half3 ambient = max(ShadeSH9(half4(normal, 1.0h)), half3(0.12h, 0.12h, 0.12h));
                half3 shadowed = baseColor * _ShadowColor.rgb;
                half3 lit = baseColor * (_LightColor0.rgb * 0.92h + ambient * 0.52h);
                half3 color = lerp(shadowed, lit, celLight);

                half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.worldPosition);
                half rim = pow(1.0h - saturate(dot(normal, viewDirection)), _RimPower) * _RimStrength;
                color += _RimColor.rgb * rim;

                half4 output = half4(saturate(color), sampleColor.a);
                UNITY_APPLY_FOG(input.fogCoord, output);
                return output;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }

    Fallback "Diffuse"
}
