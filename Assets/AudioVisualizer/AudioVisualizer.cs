using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [SerializeField]
    AudioData audioData = null;
    [SerializeField]
    GameObject samplePrefab = null;
    [SerializeField]
    GameObject freqBandPrefab = null;
    [SerializeField]
    float maxHeightFreq = 10.0f;
    [SerializeField]
    float maxHeightBand = 10.0f;
    [SerializeField]
    float sampleDistance = 100.0f;
    [SerializeField]
    float freqBandDistance = 5.0f;

    GameObject[] sampleInstances = null;
    GameObject[] freqBandInstances = null;
    GameObject[] freqBandInstances2 = null;

    private void Start()
    {
        float circleFactor = (1.0f / audioData.sampleCount) * Mathf.PI * 2.0f;
        GameObject container = new GameObject("sample");
        sampleInstances = new GameObject[audioData.sampleCount];
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = Instantiate(samplePrefab, container.transform);
            instance.name = "sample_" + i;
            float pos = i * circleFactor;

            Vector3 dir = new Vector3(Mathf.Cos(pos), 0, Mathf.Sin(pos));
            instance.transform.position = transform.position + dir * sampleDistance;
            instance.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

            sampleInstances[i] = instance;
        }

        container = new GameObject("freqBand8");
        freqBandInstances = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            GameObject instance = Instantiate(freqBandPrefab, container.transform);
            instance.name = "freqBand8_" + i;

            Vector3 pos = transform.position;
            pos.x += i * freqBandDistance - 8 * freqBandDistance * 0.5f;
            instance.transform.position = pos;

            freqBandInstances[i] = instance;
        }

        container = new GameObject("freqBand64");
        freqBandInstances2 = new GameObject[64];
        for (int i = 0; i < 64; i++)
        {
            GameObject instance = Instantiate(freqBandPrefab, container.transform);
            instance.name = "freqBand64_" + i;

            Vector3 pos = transform.position;
            pos.z += 10.0f;
            pos.x += i * freqBandDistance - 64 * freqBandDistance * 0.5f;
            instance.transform.position = pos;

            freqBandInstances2[i] = instance;
        }
    }

    float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 10.0f, -80.0f, 0.0f);
    }

    float normalize(float db)
    {
        return db / -80.0f;
    }

    private void Update()
    {
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = sampleInstances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = lin2dB(audioData.samplesLeft[i]) * maxHeightFreq;
            instance.transform.localScale = scale;
        }

        for (int i = 0; i < freqBandInstances.Length; i++)
        {
            GameObject instance = freqBandInstances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = audioData.freqBand8[i] * maxHeightBand;
            instance.transform.localScale = scale;

            Vector3 pos = instance.transform.localPosition;
            pos.y = scale.y * 0.5f;
            instance.transform.localPosition = pos;
        }

        for (int i = 0; i < freqBandInstances2.Length; i++)
        {
            GameObject instance = freqBandInstances2[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = audioData.freqBand64[i] * maxHeightBand;
            instance.transform.localScale = scale;

            Vector3 pos = instance.transform.localPosition;
            pos.y = scale.y * 0.5f;
            instance.transform.localPosition = pos;
        }
    }
}
