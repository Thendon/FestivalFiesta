using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    public Transform playerTransform;

    public float distance;

    void Update()
    {
        transform.position = playerTransform.position + -transform.forward * distance;
    }
}
