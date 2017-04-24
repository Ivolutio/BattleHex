using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public World World;
    public GameObject mainCam;
    public GameObject MenuCam;
    public GameObject GameUI;

    public void StartGame()
    {
        mainCam.SetActive(true);
        MenuCam.gameObject.SetActive(false);
        World.StartGame();

        GameUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void RerollWorld()
    {
        World.RerollBoardButton();
    }

    public void OpenSettings()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
