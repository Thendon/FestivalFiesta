using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLight : MonoBehaviour
{
    Light lightSource = null;
    float baseIntensity = 0.0f;
    AudioData data = null;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        data = FindObjectOfType<AudioData>();
    }

    void Start()
    {
        baseIntensity = lightSource.intensity;
    }

    private void Update()
    {
        lightSource.intensity = baseIntensity * data.amplitudeBuffer;
    }
}
