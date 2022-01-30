using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressSlider : MonoBehaviour
{
    public Slider slider; 

    LevelState state;

    void Awake()
    {
        state = FindObjectOfType<LevelState>();
    }

    void Update()
    {
        slider.value = state.Progress;
    }
}
