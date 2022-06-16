using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SliderValue : MonoBehaviour
{
    Slider slider;
    public TextMeshProUGUI text;

    public AudioMixer audioMixer;

    PostProcessVolume postProcessVolume;

    public string endUnit = "";
    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();

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
        //controls
        else if (gameObject.name == "ZoomCamSlider")
            slider.value = SettingsVariables.sliderDictionary["zoomSpeed"];
        else if (gameObject.name == "RotateCamSlider")
            slider.value = SettingsVariables.sliderDictionary["camRotSpeed"];

        //lighting
        else if (gameObject.name == "FogPercentSlider")
            slider.value = SettingsVariables.sliderDictionary["fogPercentage"];
        else if (gameObject.name == "BrightnessMultiplierSlider")
            slider.value = SettingsVariables.sliderDictionary["brightnessMultiplier"];


        text.text = slider.value.ToString() + endUnit;//"F2"

        AdjustBrightness();
        AdjustFog();
    }

    // Update is called once per frame
    public void UpdateText(string value)
    {
        text.text = slider.value.ToString() + endUnit;

        if (value == "totalSound")
        {
            SettingsVariables.sliderDictionary[value] = slider.value / 100.0f;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[value], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
            SaveSettings.SaveFloat(value);
        }
        else if (value == "sfxSound")
        {
            SettingsVariables.sliderDictionary[value] = slider.value / 100.0f;
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[value], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
            SaveSettings.SaveFloat(value);
        }
        else if (value == "musicSound")
        {
            SettingsVariables.sliderDictionary[value] = slider.value / 100.0f;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[value], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
            SaveSettings.SaveFloat(value);
        }

        else
        {
            SettingsVariables.sliderDictionary[value] = slider.value;
            SaveSettings.SaveFloat(value);
        }
    }

    public void AdjustBrightness()
    {
        float newValue = SettingsVariables.sliderDictionary["brightnessMultiplier"];
        postProcessVolume.profile.GetSetting<ColorGrading>().brightness.value = newValue;
    }

    public void AdjustFog()
    {
        float newValue = 0.05f * SettingsVariables.sliderDictionary["fogPercentage"] / 100;
        RenderSettings.fogDensity = Mathf.Clamp(newValue, 0.0f, 0.05f);
    }
}
