using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioBeam : MonoBehaviour
{
    [SerializeField]
    VisualEffect startEffect;
    [SerializeField]
    VisualEffect endEffect;
    [SerializeField]
    LineRenderer lineRenderer;

    Vector3 startPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;
    SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.material.SetColor("_EmissionColor", sceneLoader.playerPalette.color);
        endEffect.SetTexture("HitTexture", sceneLoader.playerPalette.particleTex);
    }

    private void UpdateEffectPositions()
    {
        startEffect.transform.position = startPos;
        startEffect.transform.LookAt(targetPos, Vector3.up);
        endEffect.transform.position = targetPos;
    }

    public void SetStart(Vector3 point)
    {
        startPos = point;
        lineRenderer.SetPosition(0, startPos);
        UpdateEffectPositions();
    }

    public void SetTarget(Vector3 point)
    {
        targetPos = point;
        lineRenderer.SetPosition(1, targetPos);
        UpdateEffectPositions();
    }
}
