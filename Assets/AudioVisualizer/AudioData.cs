using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    //[SerializeField]
    //AudioSource audioSource = null;
    [SerializeField]
    public int sampleCountPower = 9;
    [SerializeField]
    public int freqBandCount = 8;

    [HideInInspector]
    public float[] samples;
    [HideInInspector]
    public float[] freqBands;
    [HideInInspector]
    public int sampleCount;
    
    FMOD.DSP fft;

    private void Awake()
    {
        //if (audioSource != null)
        //    audioSource = GetComponent<AudioSource>();

        //waaaas florian beherrscht bitshifting magic? ?? ? 
        sampleCount = 1 << sampleCountPower;
        sampleCount = Mathf.Clamp(64, sampleCount, 8192);
        samples = new float[sampleCount];

        freqBands = new float[freqBandCount];
    }

    private void Start()
    {
        FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.BLACKMANHARRIS);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, sampleCount * 2);

        FMOD.ChannelGroup channelGroup;
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);
    }

    void Update()
    {
        //if (audioSource.clip == null)
        //    return;

        //audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);
        GetSpectrumData();
        GenerateFrequenzyBands();
    }

    void GetSpectrumData()
    {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        var spectrum = fftData.spectrum;

        if (fftData.numchannels > 0)
            for (int i = 0; i < sampleCount; ++i)
                samples[i] = spectrum[0][i];
    }

    void GenerateFrequenzyBands()
    {
        int count = 0;

        int rest = sampleCount % freqBandCount;
        for (int i = 0; i < freqBandCount; i++)
        {
            float sum = 0;
            //only works for 512 lol
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if(i == freqBandCount - 1)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                sum += samples[count] * (count + 1);
                count++;
            }

            freqBands[i] = sum / count;
        }
    }
}
