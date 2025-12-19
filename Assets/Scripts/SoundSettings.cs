using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SoundSettings : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    public float savedMusicVolume;
    public float savedSFXVolume;

    private void OnEnable()
    {
        savedMusicVolume = AudioManager.Instance.musicSource.volume;
        savedSFXVolume = AudioManager.Instance.sfxSource.volume;

        if (musicSlider != null) musicSlider.value = savedMusicVolume;
        if (sfxSlider != null) sfxSlider.value = savedSFXVolume;
    }

    private void Awake()
    {
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    public void OnMusicChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    public void ApplySettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SaveVolume();
            savedMusicVolume = musicSlider.value;
            savedSFXVolume = sfxSlider.value;
        }
    }

    public void CancelSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(savedMusicVolume);
            AudioManager.Instance.SetSFXVolume(savedSFXVolume);
            SceneManager.LoadScene("DieScene");
        }
    }
}
