using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField]
    float speed = 1.0f;

    void Update()
    {
        Quaternion rot = transform.localRotation;
        rot *= Quaternion.Euler(0f, 0f, speed * Time.deltaTime);
        transform.localRotation = rot;
    }
}
