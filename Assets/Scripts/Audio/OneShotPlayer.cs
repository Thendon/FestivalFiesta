using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotPlayer : MonoBehaviour
{

    public void PlayOneShot(string eventString)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventString);
    }

    public void PlayOneShot(string eventString, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventString, position);
    }
}
