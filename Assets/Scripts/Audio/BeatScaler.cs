using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScaler : MonoBehaviour
{
    public Vector3 scaleNormal = Vector3.one;
    public Vector3 scaleBeat = Vector3.one;

    public AnimationCurve scaleCurve;

    public GameObject[] objectsToScale;

    float t = 0;

    void ScaleToBeat()
    {
        t = 1f;
    }

    void Update()
    { 
        t = Mathf.Clamp01(t - Time.deltaTime);
        Vector3 scale = Vector3.LerpUnclamped(scaleNormal, scaleBeat, scaleCurve.Evaluate(t));

        if (objectsToScale != null && objectsToScale.Length > 0)
        {
            foreach (var obj in objectsToScale)
                obj.transform.localScale = scale;
        } 
        else
        {
            transform.localScale = scale;
        }   
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
