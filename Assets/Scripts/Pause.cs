using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject canvasMenu;
    public GameObject canvasPlay;
    public Animator menuAnimator;
    public string showTrigger = "show";
    public string hideTrigger = "hide";
    bool trigger = false;
    public void TogglePause() 
    {
        if (trigger) {
            ResumGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void ResumGame()
    {
        if (menuAnimator != null)
            menuAnimator.SetTrigger(hideTrigger);
            canvasMenu.SetActive(false);
            trigger = false;
            Time.timeScale = 1.0f;
            canvasPlay.SetActive(true);
    }

    private void PauseGame()
    {
        canvasPlay.SetActive(false);
        Time.timeScale = 0.0f;
        trigger = true;
        canvasMenu.SetActive(true);
        if (menuAnimator != null)
            menuAnimator.SetTrigger(showTrigger);
    }

}
