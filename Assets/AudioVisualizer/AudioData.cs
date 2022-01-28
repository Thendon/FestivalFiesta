using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource = null;
    [SerializeField]
    public int sampleCountPower = 9;

    [HideInInspector]
    public float[] samples;
    [HideInInspector]
    public int sampleCount;

    private void Awake()
    {
        if (audioSource != null)
            audioSource = GetComponent<AudioSource>();

        //waaaas florian beherrscht bitshifting magic? ?? ? 
        sampleCount = 1 << sampleCountPower;
        samples = new float[sampleCount];
    }

    void Update()
    {
        if (audioSource.clip == null)
            return;

        audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);

    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 1; i < samples.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, samples[i] + 10, 0), new Vector3(i, samples[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(samples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(samples[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), samples[i - 1] - 10, 1), new Vector3(Mathf.Log(i), samples[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(samples[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(samples[i]), 3), Color.blue);
        }
    }
}
