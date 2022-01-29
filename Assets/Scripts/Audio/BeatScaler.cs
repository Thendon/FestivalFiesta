using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScaler : MonoBehaviour
{
    public Vector3 scaleNormal;
    public Vector3 scaleBeat;

    public AnimationCurve scaleCurve;

    float t = 0;

    void ScaleToBeat()
    {
        t = 1f;
    }

    void Update()
    { 
        t = Mathf.Clamp01(t - Time.deltaTime);
        Vector3 scale = Vector3.LerpUnclamped(scaleNormal, scaleBeat, scaleCurve.Evaluate(t));
        transform.localScale = scale;
    }


    void OnEnable()
    {
        BeatSystem.beatTickEvent += ScaleToBeat;
    }

    void OnDisable()
    {
        BeatSystem.beatTickEvent += ScaleToBeat;
    }
}
