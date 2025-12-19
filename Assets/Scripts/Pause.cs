using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Pause : SoundSettings
{
    public GameObject canvasMenu;
    public GameObject canvasPlay;
    public GameObject panel1;
    public GameObject panel;
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
    
    public void Settings()
    {
        panel.SetActive(true);
        panel1.SetActive(false);
        if (AudioManager.Instance != null)
        {
            savedMusicVolume = AudioManager.Instance.musicSource.volume;
            savedSFXVolume = AudioManager.Instance.sfxSource.volume;
        }

        if (musicSlider != null) musicSlider.value = savedMusicVolume;
        if (sfxSlider != null) sfxSlider.value = savedSFXVolume;
    }


    public void SettingsBack()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(savedMusicVolume);
            AudioManager.Instance.SetSFXVolume(savedSFXVolume);
        }


        if (musicSlider != null) musicSlider.value = savedMusicVolume;
        if (sfxSlider != null) sfxSlider.value = savedSFXVolume;

        panel.SetActive(false);
        panel1.SetActive(true);
    }

    public void ApplySettingsFromPause()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SaveVolume();
        }

        if (musicSlider != null) savedMusicVolume = musicSlider.value;
        if (sfxSlider != null) savedSFXVolume = sfxSlider.value;
        panel.SetActive(false);
        panel1.SetActive(true);
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
