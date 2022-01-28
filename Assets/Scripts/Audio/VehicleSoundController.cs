using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSoundController : MonoBehaviour
{
    public FMOD.Studio.EventInstance instance;
    public FMODUnity.EventReference fmodEvent;

    [Range(0f, 10000f)]
    public float motorRPM;
    [Range(0f, 1f)]
    public float load;

    void Awake()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
    }

    void Start()
    {
        StartMusic();
    }

    public void StartMusic()
    {
        if (!instance.isValid())
        {
            Debug.LogError("[MusicManager] EventInstance is invalid");
            return;
        }
        instance.start();
    }

    public void OnDisable()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    public void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }

    void Update()
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        instance.setParameterByName("parameter:/Vehicles/Car Engine/RPM", motorRPM);
        instance.setParameterByName("parameter:/Vehicles/Car Engine/Load", load);
    }
}
