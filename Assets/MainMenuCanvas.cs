using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    static MainMenuCanvas instance = null;

    public void Awake()
    {
        if (instance == null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
