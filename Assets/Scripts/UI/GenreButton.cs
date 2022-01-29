using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenreButton : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    SceneLoader loader;
    [SerializeField]
    TMPro.TMP_Text text;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (loader == null)
            loader = FindObjectOfType<SceneLoader>();
        button.onClick.AddListener(OnButtonClick);
        text.text = name;
    }

    void OnButtonClick()
    {
        loader.StartRound(name);
    }
}
