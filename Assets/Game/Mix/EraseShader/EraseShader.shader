Shader "Custom/EraseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Основная текстура
        _MaskTex ("Mask Texture", 2D) = "white" {} // Текстура маски
        _Color ("Color", Color) = (1, 1, 1, 1) // Цвет для смешивания с текстурой
        _Alpha ("Alpha", Range(0, 1)) = 1 // Прозрачность
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; // Основная текстура
            sampler2D _MaskTex; // Текстура маски
            float4 _MainTex_ST; // Параметры текстуры (масштаб и смещение)
            fixed4 _Color; // Цвет для смешивания
            float _Alpha; // Прозрачность

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем цвет из основной текстуры
                fixed4 col = tex2D(_MainTex, i.uv);

                // Получаем маску из текстуры маски
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // Применяем цвет и прозрачность
                col.rgb *= _Color.rgb; // Смешиваем цвет с текстурой
                col.a *= mask.r * _Alpha; // Применяем маску и настройку прозрачности

                return col;
            }
            ENDCG
        }
    }
}