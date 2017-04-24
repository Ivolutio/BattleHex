using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public GameObject EscMenu;
    public GameObject WinPlayer1;
    public GameObject WinPlayer2;

    int wasPlacing = 0;
	public void Esc(bool state)
    {
        escOpen = state;
        EscMenu.SetActive(state);
        Player player1 = GameObject.Find("Player1").GetComponent<Player>();
        Player player2 = GameObject.Find("Player2").GetComponent<Player>();
        if (wasPlacing == 1)
            player1.placingPawns = !state;
        else if (wasPlacing == 2)
            player2.placingPawns = !state;
        else
        {
            if (player1.placingPawns)
            {
                player1.placingPawns = !state;
                wasPlacing = 1;
            }
            else if (player2.placingPawns)
            {
                player2.placingPawns = !state;
                wasPlacing = 2;
            }
            else
                wasPlacing = 0;
        }
    }

    public void WinP1()
    {
        Esc(true);
        WinPlayer1.SetActive(true);
    }

    public void WinP2()
    {
        Esc(true);
        WinPlayer2.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    bool escOpen;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc(!escOpen);
        }
    }
}
