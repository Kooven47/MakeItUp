using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider MasterVolumeSlider;
    [SerializeField] Slider BackgroundMusicVolumeSlider;
    [SerializeField] Slider FootstepVolumeSlider;
    [SerializeField] Slider SoundEffectVolume;

    [SerializeField] AudioMixer masterMixer;
    // Start is called before the first frame update
    void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedMasterVolume", 100), "MasterVolume"); // There is where we need to access saved volume
        SetVolume(PlayerPrefs.GetFloat("SavedBackgroundMusicVolume", 100), "BackgroundMusicVolume");
        SetVolume(PlayerPrefs.GetFloat("SavedFootstepVolume", 100), "FootstepVolume");
        SetVolume(PlayerPrefs.GetFloat("SavedSoundEffectVolume", 100), "SoundEffectVolume");
    }

    public void SetVolume(float _value, string sliderName)
    {
        if (_value < 1)
        {
            _value = 0.001f;
        }
        if (sliderName == "MasterVolume")
            PlayerPrefs.SetFloat("SavedMasterVolume", _value);
        if (sliderName == "BackgroundMusicVolume")
            PlayerPrefs.SetFloat("SavedBackgroundMusicVolume", _value);
        if (sliderName == "FootstepVolume")
            PlayerPrefs.SetFloat("SavedFootstepVolume", _value);
        if (sliderName == "SoundEffectVolume")
            PlayerPrefs.SetFloat("SavedSoundEffectVolume", _value);
        RefreshSlider(_value, sliderName);
        // Save volume pref here
        
        masterMixer.SetFloat(sliderName, Mathf.Log10(_value / 100) * 20f);
    }

    public void SetVolumeFromSlider(string sliderName)
    {
        if (sliderName == "MasterVolume")
            SetVolume(MasterVolumeSlider.value, sliderName);
        if (sliderName == "BackgroundMusicVolume")
            SetVolume(BackgroundMusicVolumeSlider.value,sliderName);
        if (sliderName == "FootstepVolume")
            SetVolume(FootstepVolumeSlider.value, sliderName);
        if (sliderName == "SoundEffectVolume")
            SetVolume(SoundEffectVolume.value, sliderName);
    }

    public void RefreshSlider(float _value, string sliderName)
    {
        if (sliderName == "MasterVolume")
            MasterVolumeSlider.value = _value;
        if (sliderName == "BackgroundMusicVolume")
            BackgroundMusicVolumeSlider.value = _value;
        if (sliderName == "FootstepVolume")
            FootstepVolumeSlider.value = _value;
        if (sliderName == "SoundEffectVolume")
            SoundEffectVolume.value = _value;
    }
}
