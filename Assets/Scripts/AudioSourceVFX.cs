using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioSourceVFX : MonoBehaviour
{
    [SerializeField]
    VisualEffect[] effects = new VisualEffect[0];
    [SerializeField, Range(0, 7)]
    int[] listeningBands = new int[1];
    [SerializeField]
    float threshhold = 1.0f;
    
    AudioData audioData;
    bool triggered = false;

    private void Awake()
    {
        audioData = FindObjectOfType<AudioData>();
    }

    float GetBandValue()
    {
        float sum = 0.0f;
        foreach (int listeningBand in listeningBands)
        {
            sum += audioData.freqBand8[listeningBand];
        }
        return sum;
    }

    private void Update()
    {
        if (audioData == null)
            return;

        float val = GetBandValue();

        if(!triggered && val > threshhold)
        {
            triggered = true;
            foreach (VisualEffect effect in effects)
            {
                effect.SendEvent("OnPlay");
            }
        }
        else if (triggered && val < threshhold)
        {
            triggered = false;
        }
    }
}
