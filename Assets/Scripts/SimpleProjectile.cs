using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wow
public class SimpleProjectile : MonoBehaviour
{
    public float speed = 3.0f;
    public float liveTime = 10.0f;

    float death;

    private void Awake()
    {
        death = liveTime;
    }

    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

        death -= Time.deltaTime;
        if (death < 0)
            Destroy(gameObject);
    }
}
