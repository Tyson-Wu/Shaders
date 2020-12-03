using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
[Serializable]
public class SingleGerstnerWaveParam
{
    [Tooltip("是否是方向波还是原点波")]
    public bool IsDirWave;
    [Tooltip("波传播方向或者原点(分别表示 x 和 z 轴)")]
    public Vector2 DirOrPos;
    [Tooltip("在所有波中的贡献比例")]
    public float Contribution;
    [Tooltip("振幅")]
    public float Amplitude;
    [Tooltip("波的扩展频率")]
    public float Frequency;
    [Tooltip("波的起伏频率")]
    public float Fluctuation;
    [Tooltip("波的步长")]
    public float Steepness;
}
public interface IWaveMaterialSetting
{
    Material GetMaterial();
}


[CreateAssetMenu(fileName = "MultiGerstnerWavesSetting", menuName = "Wave/GerstnerWaves_Multi", order = 1)]
public class MultiGerstnerWavesSetting : ScriptableObject, IWaveMaterialSetting
{
    
    [SerializeField,Range(1,4)] int waveCount = 4;
    public Material material;
    public SingleGerstnerWaveParam param1;
    public SingleGerstnerWaveParam param2;
    public SingleGerstnerWaveParam param3;
    public SingleGerstnerWaveParam param4;

    public Material GetMaterial()
    {
        if(material==null|| material.shader.name != "Gems/MultiGerstnerWaves")
        {
            material = new Material(Shader.Find("Gems/MultiGerstnerWaves"));
        }
        List<SingleGerstnerWaveParam> waveParams = new List<SingleGerstnerWaveParam>();
        waveParams.Add(param1); waveParams.Add(param2); waveParams.Add(param3); waveParams.Add(param4);
        float all_Contribution = 0;
        float[] cons = new float[4];
        float[] ams = new float[4];
        float[] flus = new float[4];
        float[] stes = new float[4];
        for (int i=0;i< 4; ++i)
        {
            SingleGerstnerWaveParam p = waveParams[i];
            cons[i] = p.Contribution;
            flus[i] = p.Fluctuation;
            stes[i] = p.Steepness;
            ams[i] = p.Amplitude;
            if(i< waveCount) all_Contribution += cons[i];
        }
        if(all_Contribution>1)
        {
            for (int i = 0; i < 4; ++i)
            {
                cons[i] = cons[i] / all_Contribution;
            }
        }
        for (int i = 0; i < 4; ++i)
        {
            float cons_3 = Mathf.Pow(cons[i], 1 / 3.0f);
            float c = ams[i] * flus[i] * stes[i];
            if (c < cons_3) continue;
            c = Mathf.Pow(c, 1 / 3.0f);
            cons_3 /= c;
            ams[i] = ams[i] * cons_3;
            flus[i] = flus[i] * cons_3;
            stes[i] = stes[i] * cons_3;
        }
        Vector4[] dir = new Vector4[4];
        Vector4[] am_flu_St_fre = new Vector4[4];
        for (int i = 0; i < 4; ++i)
        {
            SingleGerstnerWaveParam p = waveParams[i];
            dir[i] = new Vector4(p.DirOrPos.x, p.DirOrPos.y, p.IsDirWave ? 0 : 1, 0);
            am_flu_St_fre[i] = new Vector4(ams[i], flus[i], stes[i], p.Frequency);
        }
        material.SetVectorArray("_dir", dir);
        material.SetVectorArray("_am_flu_St_fre", am_flu_St_fre);
        material.SetInt("_param_count", waveCount);
        return material;
    }
}
