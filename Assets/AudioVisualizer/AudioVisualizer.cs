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
    float maxHeight = 10.0f;
    [SerializeField]
    float sampleDistance = 100.0f;
    [SerializeField]
    float freqBandDistance = 5.0f;

    GameObject[] sampleInstances = null;
    GameObject[] freqBandInstances = null;

    private void Start()
    {
        sampleInstances = new GameObject[audioData.sampleCount];
        float circleFactor = (1.0f / audioData.sampleCount) * Mathf.PI * 2.0f;
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = Instantiate(samplePrefab, transform);
            instance.name = "sample_" + i;
            float pos = i * circleFactor;

            Vector3 dir = new Vector3(Mathf.Cos(pos), 0, Mathf.Sin(pos));
            instance.transform.position = transform.position + dir * sampleDistance;
            instance.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

            sampleInstances[i] = instance;
        }

        freqBandInstances = new GameObject[audioData.freqBandCount];
        for (int i = 0; i < audioData.freqBandCount; i++)
        {
            GameObject instance = Instantiate(freqBandPrefab, transform);
            instance.name = "freqBand_" + i;

            Vector3 pos = transform.position;
            pos.x += i * freqBandDistance - audioData.freqBandCount * freqBandDistance * 0.5f;
            instance.transform.position = pos;

            freqBandInstances[i] = instance;
        }
    }

    float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }

    private void Update()
    {
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = sampleInstances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = lin2dB(audioData.samples[i]) * maxHeight;
            instance.transform.localScale = scale;
        }

        for (int i = 0; i < audioData.freqBandCount; i++)
        {
            GameObject instance = freqBandInstances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = audioData.freqBands[i] * maxHeight;
            instance.transform.localScale = scale;

            Vector3 pos = instance.transform.localPosition;
            pos.y = scale.y * 0.5f;
            instance.transform.localPosition = pos;
        }
    }
}
