Shader "Gems/MultiGerstnerWaves"
{
    Properties
    {

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
            int _param_count;
            float4 _dir[4]; //x 表示x坐标，y 表示z坐标， z 表示是否是点
            float4 _am_flu_St_fre[4]; //x 表示振幅， w 表示水平频率， z 表示步长, w表示垂直震动频率

            void single_wave(float3 v_pos,float4 dir,float4 am,out float3 pos,out float3 normal)
            {
                pos=float3(0,0,0);
                normal = float3(0,1,0);

                float Amplitude = am.x;
                float Frequency = am.w;
                float Steepness = am.z;
                float Fluctuation = am.y;

                float2 dir_x_z = float2(dir.x - dir.z * v_pos.x,dir.y - dir.z * v_pos.z);
                dir_x_z += 2 * dir.z * dir_x_z;
                dir_x_z /= length(dir_x_z)+0.001;
                float2 center_x_z = float2(dir.x,dir.y) * dir.z;

                float value = dot(dir_x_z,float2(v_pos.x-center_x_z.x, v_pos.z-center_x_z.y)) * Fluctuation + _Time.y * Frequency;
                float sin_f = sin(value);
                float cos_f = cos(value);

                pos.x = Steepness * Amplitude * dir_x_z.x * cos_f;
                pos.z = Steepness * Amplitude * dir_x_z.y * cos_f; 
                //pos.x = 0;
                //pos.z = 0; 
                pos.y = Amplitude * sin_f;
			}

            v2f vert (appdata v)
            {
                v2f o;
                float3 pos_wave,normal;
                float3 pos = float3(0,0,0);
                for(int i=0;i<_param_count;++i)
                {
                    single_wave(v.vertex.xyz, _dir[i],_am_flu_St_fre[i], pos_wave, normal); 
                    pos += pos_wave;
				}
                pos += v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(pos);
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
