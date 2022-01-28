using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBomb : MonoBehaviour
{
    public float growSpeed = 3.0f;

    // Update is called once per frame
    void Update()
    {
        float change = growSpeed * Time.deltaTime;

        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x + change, scale.y, scale.z + change);
    }
}
