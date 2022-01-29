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
    [SerializeField, ColorUsage(true, true)]
    Color[] colors = new Color[8];

    [SerializeField]
    GameObject sampleContainer = null;
    [SerializeField]
    GameObject freq8Container = null;
    [SerializeField]
    GameObject freq64Container = null;

    Material sampleMaterial = null;
    Color sampleColor = Color.white;
    GameObject[] sampleInstances = null;
    Material[] freq8BandMaterials = null;
    Color[] freq8BandColors = null;
    GameObject[] freq8BandInstances = null;
    Material[] freq64BandMaterials = null;
    Color[] freq64BandColors = null;
    GameObject[] freq64BandInstances2 = null;

    private void Start()
    {
        float circleFactor = (1.0f / audioData.sampleCount) * Mathf.PI * 2.0f;
        sampleInstances = new GameObject[audioData.sampleCount];

        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = Instantiate(samplePrefab, sampleContainer.transform);

            MeshRenderer meshRenderer = instance.GetComponent<MeshRenderer>();
            if (i == 0)
            {
                sampleMaterial = Instantiate(meshRenderer.material);
                sampleColor = sampleMaterial.GetColor("_EmissionColor");
            }
            meshRenderer.material = sampleMaterial;

            instance.name = "sample_" + i;
            float pos = i * circleFactor;

            Vector3 dir = new Vector3(Mathf.Cos(pos), Mathf.Sin(pos), 0);
            instance.transform.position = sampleContainer.transform.position + dir * sampleDistance;
            instance.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir.normalized);
             
            sampleInstances[i] = instance;
        }

        freq8BandInstances = new GameObject[8];
        freq8BandMaterials = new Material[8];
        freq8BandColors = new Color[8];
        for (int i = 0; i < 8; i++)
        {
            GameObject instance = Instantiate(freqBandPrefab, freq8Container.transform);
            instance.name = "freqBand8_" + i;

            Vector3 scale = instance.transform.localScale;
            scale.y = 200.0f; //audioData.freqBand8Buffer[i] * maxHeightBand;
            instance.transform.localScale = scale;
            Vector3 pos = new Vector3();
            pos.x += i * freqBandDistance - 8 * freqBandDistance * 0.5f;
            pos.y = scale.y * 0.5f;
            instance.transform.localPosition = pos;

            freq8BandMaterials[i] = instance.GetComponent<MeshRenderer>().material;
            freq8BandMaterials[i].SetColor("_EmissionColor", colors[i]);
            freq8BandColors[i] = colors[i];

            freq8BandInstances[i] = instance;
        }

        freq64BandInstances2 = new GameObject[64];
        freq64BandMaterials = new Material[64];
        freq64BandColors = new Color[64];
        for (int i = 0; i < 64; i++)
        {
            GameObject instance = Instantiate(freqBandPrefab, freq64Container.transform);
            instance.name = "freqBand64_" + i;

            Vector3 pos = freq64Container.transform.position;
            pos.x += i * freqBandDistance - 64 * freqBandDistance * 0.5f;
            instance.transform.position = pos;
            freq64BandMaterials[i] = instance.GetComponent<MeshRenderer>().material;
            freq64BandColors[i] = freq64BandMaterials[i].GetColor("_EmissionColor");

            freq64BandInstances2[i] = instance;
        }
    }

    float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 10.0f, -80.0f, 0.0f);
    }

    private void Update()
    {
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = sampleInstances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = 80.0f + lin2dB(audioData.samplesLeft[i]) * maxHeightFreq;
            instance.transform.localScale = scale;

            sampleMaterial.SetColor("_EmissionColor", sampleColor * audioData.amplitudeBuffer);
        }

        for (int i = 0; i < freq8BandInstances.Length; i++)
        {
            GameObject instance = freq8BandInstances[i];

            freq8BandMaterials[i].SetColor("_EmissionColor", freq8BandColors[i] * (audioData.audioBand8Buffer[i] * 0.8f + 0.2f) );
        }

        for (int i = 0; i < freq64BandInstances2.Length; i++)
        {
            GameObject instance = freq64BandInstances2[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = audioData.freqBand64Buffer[i] * maxHeightBand;
            instance.transform.localScale = scale;

            Vector3 pos = instance.transform.localPosition;
            pos.y = scale.y * 0.5f;
            instance.transform.localPosition = pos;

            freq64BandMaterials[i].SetColor("_EmissionColor", freq64BandColors[i] * audioData.audioBand64Buffer[i]);
        }
    }
}
