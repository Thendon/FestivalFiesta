using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [SerializeField]
    AudioData audioData = null;
    [SerializeField]
    GameObject prefab = null;
    [SerializeField]
    float maxHeight = 10.0f;
    [SerializeField]
    float distance = 100.0f;

    GameObject[] instances = null;

    private void Start()
    {
        instances = new GameObject[audioData.sampleCount];
        float circleFactor = (1.0f / audioData.sampleCount) * 360.0f;
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = Instantiate(prefab, transform);
            instance.name = "instance_" + i;
            float pos = i * circleFactor;
            Vector3 dir = new Vector3(Mathf.Cos(pos), 0, Mathf.Sin(pos));
            instance.transform.position = transform.position + dir * distance;
            instance.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

            instances[i] = instance;
        }
    }

    private void Update()
    {
        for (int i = 0; i < audioData.sampleCount; i++)
        {
            GameObject instance = instances[i];
            Vector3 scale = instance.transform.localScale;
            scale.y = audioData.samples[i] * maxHeight;
            instance.transform.localScale = scale;
        }
    }
}
