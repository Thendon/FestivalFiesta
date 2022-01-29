using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(AudioData))]
public class AudioDataEditor : Editor
{
    SerializedProperty lookAtPoint;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioData audioData = (AudioData)target;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("sample count = 2^" + audioData.sampleCountPower, 1 << audioData.sampleCountPower);
        EditorGUI.EndDisabledGroup();
    }
}
#endif

public class AudioData : MonoBehaviour
{
    enum Channel
    {
        Left,
        Right,
        Stereo
    }

    const int freqBandSampleBase = 512;
    //[SerializeField]
    //AudioSource audioSource = null;
    [SerializeField]
    public MusicManager musicManager = null;
    [SerializeField]
    public int sampleCountPower = 9;
    [SerializeField]
    int audioProfile = 0;
    [SerializeField]
    Channel channel;
    [SerializeField]
    float bandDecrease1 = 0.005f;
    [SerializeField]
    float bandDecrease2 = 1.2f;

    [HideInInspector]
    public float[] samplesLeft;
    [HideInInspector]
    public float[] samplesRight;
    [HideInInspector]
    public float[] freqBand8;
    [HideInInspector]
    public float[] freqBand64;
    [HideInInspector]
    public int sampleCount;

    public float amplitude { get; private set; } = 0.0f;
    public float amplitudeBuffer { get; private set; } = 0.0f;
    float amplitudeHighest = 0.0f;

    [HideInInspector]
    public float[] freqBand8Buffer;
    float[] freqBand8Decrease;
    float[] freqBand8Highest;
    float[] audioBand8;
    [HideInInspector]
    public float[] audioBand8Buffer;

    [HideInInspector]
    public float[] freqBand64Buffer;
    float[] freqBand64Decrease;
    float[] freqBand64Highest;
    float[] audioBand64;
    [HideInInspector]
    public float[] audioBand64Buffer;
    FMOD.DSP fft;

    private void Awake()
    {
        //if (audioSource == null)
        //    audioSource = GetComponent<AudioSource>();
        if (musicManager == null)
            musicManager = GetComponent<MusicManager>();

        //waaaas florian beherrscht bitshifting magic? ?? ? 
        sampleCount = 1 << sampleCountPower;
        sampleCount = Mathf.Clamp(512, sampleCount, 8192);
        samplesLeft = new float[sampleCount];

        freqBand8 = new float[8];
        freqBand8Buffer = new float[8];
        freqBand8Decrease = new float[8];
        freqBand8Highest = new float[64];
        audioBand8 = new float[8];
        audioBand8Buffer = new float[8];

        freqBand64 = new float[64];
        freqBand64Buffer = new float[64];
        freqBand64Decrease = new float[64];
        freqBand64Highest = new float[64];
        audioBand64 = new float[64];
        audioBand64Buffer = new float[64];
    }

    private void Start()
    {
        FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.BLACKMANHARRIS);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, sampleCount * 2);

        FMOD.ChannelGroup channelGroup;
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);

        AudioProfile(audioProfile);
    }

    void Update()
    {
        //if (audioSource.clip == null)
        //    return;

        //audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);

        GetSpectrumData();

        GenerateFrequencyBands8();
        GenerateFrequencyBands64();

        BandBuffer(freqBand8, freqBand8Buffer, freqBand8Decrease);
        BandBuffer(freqBand64, freqBand64Buffer, freqBand64Decrease);

        CreateAudioBand(freqBand8, freqBand8Buffer, freqBand8Highest, audioBand8, audioBand8Buffer);
        CreateAudioBand(freqBand64, freqBand64Buffer, freqBand64Highest, audioBand64, audioBand64Buffer);

        GetAmplitude(audioBand8, audioBand8Buffer);
    }

    void AudioProfile(float profile)
    {
        for (int i = 0; i < 8; i++)
            freqBand8Highest[i] = profile;
        for (int i = 0; i < 64; i++)
            freqBand64Highest[i] = profile;
    }

    void GetSpectrumData()
    {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        var spectrum = fftData.spectrum;

        for (int i = 0; i < fftData.numchannels; i++)
        {
            if (i == 0 && (channel == Channel.Left || channel == Channel.Stereo))
                for (int j = 0; j < sampleCount; ++j)
                    samplesLeft[j] = spectrum[i][j];
            else if (i == 1 && (channel == Channel.Right || channel == Channel.Stereo))
                for (int j = 0; j < sampleCount; ++j)
                    samplesRight[j] = spectrum[i][j];
        }
    }

    void GetAmplitude(float[] audio, float[] audioBuffer)
    {
        float currentAmplitude = 0.0f;
        float currentAmplitudeBuffer = 0.0f;

        for (int i = 0; i < 8; i++)
        {
            currentAmplitude += audio[i];
            currentAmplitudeBuffer += audioBuffer[i];
        }
        if (currentAmplitude > amplitudeHighest)
            amplitudeHighest = currentAmplitude;
        amplitude = currentAmplitude / amplitudeHighest;
        amplitudeBuffer = currentAmplitudeBuffer / amplitudeHighest;
    }

    void BandBuffer(float[] band, float[] bandBuffer, float[] bandDecrease)
    {
        for (int i = 0; i < band.Length; i++)
        {
            if (band[i] > bandBuffer[i])
            {
                bandBuffer[i] = band[i];
                bandDecrease[i] = bandDecrease1;
            }
            else if (band[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bandDecrease[i];
                bandDecrease[i] *= bandDecrease2;
            }
        }
    }

    void CreateAudioBand(float[] band, float[] bandBuffer, float[] bandHighest, float[] audio, float[] audioBuffer)
    {
        for (int i = 0; i < band.Length; i++)
        {
            if (band[i] > bandHighest[i])
                bandHighest[i] = band[i];

            audio[i] = band[i] / bandHighest[i];
            audioBuffer[i] = bandBuffer[i] / bandHighest[i];
        }
    }

    float GetSample(int index)
    {
        switch (channel)
        {
            case Channel.Left:
                return samplesLeft[index];
            case Channel.Right:
                return samplesRight[index];
            case Channel.Stereo:
                return samplesLeft[index] + samplesRight[index];
            default:
                return 0.0f;
        }
    }

    void GenerateFrequencyBands8()
    {
        int rest = sampleCount % (freqBandSampleBase - 2);
        int sampleFactor = sampleCount / freqBandSampleBase;

        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            float sum = 0;

            int sampleCount = (int)Mathf.Pow(2, i) * 2 * sampleFactor;
            //last band only goes to 510 otherwise
            if(i == 7)
                sampleCount += rest;

            for (int j = 0; j < sampleCount; j++)
            {
                sum += GetSample(index) * (index + 1); //maybe dont * (index + 1)
                index++;
            }

            freqBand8[i] = sum / index * 10.0f;
        }
    }

    void GenerateFrequencyBands64()
    {
        int sampleFactor = this.sampleCount / freqBandSampleBase;

        int sampleCount = 1;
        int power = 0;

        int index = 0;
        for (int i = 0; i < 64; i++)
        {
            float sum = 0;

            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power) * sampleFactor;

                if (power == 3)
                    sampleCount -= 2 * sampleFactor;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                sum += GetSample(index) * (index + 1); //maybe dont * (index + 1)
                index++;
            }

            freqBand64[i] = sum / index * 80.0f;
        }
    }
}
