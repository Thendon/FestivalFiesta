using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSoundController : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;

    private const float MIN_RPM = 0;
    private const float MAX_RPM = 10000;
    private const float MIN_LOAD = -1;
    private const float MAX_LOAD = 1;

    [SerializeField, Range(MIN_RPM, MAX_RPM)]
    private float motorRPM;
    [SerializeField, Range(MIN_LOAD, MAX_LOAD)]
    private float load;

    void Awake()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Vehicles/Car Engine");
    }

    public void Start()
    {
        if (!instance.isValid())
        {
            Debug.LogError("[VehicleSoundController] EventInstance is invalid");
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
        instance.setParameterByName("parameter:/Vehicles/Load", load);
    }

    /// <summary>
    /// Range [0, 10.000]
    /// </summary>
    public void SetMotorRPM(float rpm)
    {
        rpm = Mathf.Clamp(rpm, MIN_RPM, MAX_RPM);
    }

    /// <summary>
    /// Range [-1, -1]
    /// </summary>
    public void SetMotorLoad(float load)
    {
        load = Mathf.Clamp(load, MIN_LOAD, MAX_LOAD);
    }
}
