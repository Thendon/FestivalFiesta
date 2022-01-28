using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wow
public class SimpleProjectile : MonoBehaviour
{
    public float speed = 3.0f;

    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
}
