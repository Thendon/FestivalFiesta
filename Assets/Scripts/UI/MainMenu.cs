using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject menu = null;
    [SerializeField]
    GameObject playButton = null;
    [SerializeField]
    GameObject homeButton = null;
    [SerializeField]
    GameObject mainPage = null;
    [SerializeField]
    GameObject selectionPage = null;
    [SerializeField]
    SceneLoader loader = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        loader.onGameStart += () => SceneChange(false);
        loader.onMainMenu += () => SceneChange(true);

        OpenMainPage();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            Toggle();
    }

    void SceneChange(bool mainMenu)
    {
        playButton.SetActive(mainMenu);
        homeButton.SetActive(!mainMenu);
        Toggle(false);
        OpenMainPage();
    }

    public void OpenMainPage()
    {
        mainPage.SetActive(true);
        selectionPage.SetActive(false);
    }

    public void OpenSelectionPage()
    {
        mainPage.SetActive(false);
        selectionPage.SetActive(true);
    }

    public void Toggle()
    {
        Toggle(!menu.activeSelf);
    }

    public void Toggle(bool state)
    {
        menu.SetActive(state);
    }
}
