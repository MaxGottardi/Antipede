using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SliderValue : MonoBehaviour
{
    Slider slider;
    public TextMeshProUGUI text;

    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        if (gameObject.name == "MasterSound")
        {
            slider.value = SettingsVariables.sliderDictionary["totalSound"] * 100;
        }
        else if (gameObject.name == "SoundSFX")
        {
            slider.value = SettingsVariables.sliderDictionary["sfxSound"] * 100;
        }
        else if (gameObject.name == "SoundMusic")
        {
            slider.value = SettingsVariables.sliderDictionary["musicSound"] * 100;
        }

        else if (gameObject.name == "ZoomCamSlider")
            slider.value = SettingsVariables.sliderDictionary["zoomSpeed"];
        else if (gameObject.name == "RotateCamSlider")
            slider.value = SettingsVariables.sliderDictionary["camRotSpeed"];
        text.text = slider.value.ToString("F2");
    }

    // Update is called once per frame
    public void UpdateText()
    {
        text.text = slider.value.ToString("F2");

        if (gameObject.name == "MasterSound")
        {
            SettingsVariables.sliderDictionary["totalSound"] = slider.value / 100.0f;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary["totalSound"], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
            SaveSettings.SaveFloat("totalSound");
        }
        else if (gameObject.name == "SoundSFX")
        {
            SettingsVariables.sliderDictionary["sfxSound"] = slider.value / 100.0f;
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary["sfxSound"], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
            SaveSettings.SaveFloat("sfxSound");
        }
        else if (gameObject.name == "SoundMusic")
        {
            SettingsVariables.sliderDictionary["musicSound"] = slider.value / 100.0f;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary["musicSound"], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
            SaveSettings.SaveFloat("musicSound");
        }

        else if (gameObject.name == "ZoomCamSlider")
        {
            SettingsVariables.sliderDictionary["zoomSpeed"] = slider.value;
            SaveSettings.SaveFloat("zoomSpeed");
        }
        else if (gameObject.name == "RotateCamSlider")
        {
            SettingsVariables.sliderDictionary["camRotSpeed"] = slider.value;
            SaveSettings.SaveFloat("camRotSpeed");
        }
    }
}
