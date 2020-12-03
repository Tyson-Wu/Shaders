Shader "Gems/SinWave"
{
    Properties
    {
        _Dir("Dir",Vector)=(1,0,0)
        _W ("W",Range(0,10))=0.5
        _A ("A",Range(0,5))=1
        _E ("E",Range(0,10))=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Dir;
            float _W,_A,_E;
            

            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.y = _A * sin(dot(_Dir.xz,v.vertex.xz)*_W+_Time.y*_E);
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = fixed4(1,1,1,1);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
