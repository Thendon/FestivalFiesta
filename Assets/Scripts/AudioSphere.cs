using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSphere : MonoBehaviour
{
    [SerializeField]
    GameObject sphere = null;
    [SerializeField]
    float minScale = 0.1f;
    [SerializeField]
    float maxScale = 1.2f;
    [SerializeField]
    GameObject freqBandPrefab = null;
    [SerializeField, ColorUsage(true, true)]
    Color[] colors = new Color[8];
    [SerializeField]
    float maxHeightBand = 1.2f;
    [SerializeField]
    float minHeightBand = 0.8f;
    [SerializeField]
    float colorBoost = 1.0f;
    [SerializeField]
    float rotationSpeed = 1.0f;
    [SerializeField]
    float colorLerpSpeed = 1.0f;

    AudioData audioData;
    float sphereDefaultScale = 1.0f;
    Material[] freq8BandMaterials = null;
    Color[] freq8BandColors = null;
    GameObject[] freq8BandInstances = null;

    private void Start()
    {
        audioData = FindObjectOfType<AudioData>();
        sphereDefaultScale = sphere.transform.localScale.x;
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        //sphereMat = sphere.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", sceneLoader.selectedGenre);

        freq8BandInstances = new GameObject[8];
        freq8BandMaterials = new Material[8];
        freq8BandColors = new Color[8];

        float circleFactor = (1.0f / 8) * Mathf.PI * 2.0f;
        for (int i = 0; i < 8; i++)
        {
            GameObject container = new GameObject("freqBand8_" + i + "_container");
            container.transform.parent = sphere.transform;

            float pos = i * circleFactor;
            GameObject instance = Instantiate(freqBandPrefab, container.transform);
            Vector3 dir = new Vector3(Mathf.Cos(pos), 0, Mathf.Sin(pos));
            container.transform.localPosition = dir * (instance.transform.localScale.x * 0.5f + sphereDefaultScale * 0.5f);
            container.transform.rotation = Quaternion.LookRotation(Vector3.up, dir.normalized);
            container.transform.localScale = Vector3.one * instance.transform.localScale.x;

            instance.transform.localScale = Vector3.one;
            instance.transform.localPosition = Vector3.zero;
            instance.name = "freqBand8_" + i;

            freq8BandMaterials[i] = instance.GetComponent<MeshRenderer>().material;
            freq8BandMaterials[i].SetColor("_EmissionColor", colors[i]);
            freq8BandColors[i] = colors[i];

            freq8BandInstances[i] = instance;
        }
    }

    private void Update()
    {
        float ampScale = (maxScale - minScale) * Mathf.Max(audioData.amplitudeBuffer, 0.0001f) + minScale;
        sphere.transform.localScale = Vector3.one * ampScale * sphereDefaultScale;
        sphere.transform.localRotation *= Quaternion.Euler(0.0f, Time.deltaTime * rotationSpeed, 0.0f);

        for (int i = 0; i < freq8BandInstances.Length; i++)
        {
            GameObject instance = freq8BandInstances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = audioData.freqBand8Buffer[i] * (maxHeightBand - minHeightBand) + minHeightBand;
            instance.transform.localScale = scale;

            Vector3 pos = instance.transform.localPosition;
            pos.y = scale.y * 0.5f;
            instance.transform.localPosition = pos;

            freq8BandMaterials[i].SetColor("_EmissionColor", freq8BandColors[i] * audioData.audioBand8Buffer[i] + freq8BandColors[i] * colorBoost);
        }
    }
}
