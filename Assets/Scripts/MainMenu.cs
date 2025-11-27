using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        GameSession.IsNewGame = true; 
        SceneManager.LoadScene("LoadScene"); 
    }

    public void LoadGame()
    {
        GameSession.IsNewGame = false; 
        SceneManager.LoadScene("LoadScene"); 
    }
}
