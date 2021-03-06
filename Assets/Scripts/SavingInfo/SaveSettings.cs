using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSettings : MonoBehaviour
{
    //float saves
    const string totalSound = "totalSound";
    const string sfxSound = "sfxSound";
    const string musicSound = "musicSound";

    public AudioMixer audioMixer;
    public Material antennaMat, spiderMat, spiderSliderMat;

    private void Start()
    {
        //adjust the audio volumns to match their values
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[totalSound], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[sfxSound], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[musicSound], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0

        SetMaterialColours();
    }

    void SetMaterialColours()
    {
        Color antennaColour = new Color(SettingsVariables.sliderDictionary["antennaColourR"], SettingsVariables.sliderDictionary["antennaColourG"], SettingsVariables.sliderDictionary["antennaColourB"]);
        antennaMat.SetColor("_Color", antennaColour);
        antennaMat.SetColor("_EmissionColor", antennaColour);

        Color spiderColour = new Color(SettingsVariables.sliderDictionary["spiderColourR"], SettingsVariables.sliderDictionary["spiderColourG"], SettingsVariables.sliderDictionary["spiderColourB"]);
        spiderSliderMat.SetColor("_Color", spiderColour);
    }
    // Start is called before the first frame update
    //KeyCode LoadKeys(string item, KeyCode defaultKey)
    //{
    //    if (PlayerPrefs.HasKey(item))
    //        return (KeyCode)PlayerPrefs.GetInt(item);
    //    else
    //        return defaultKey;
    //    ////if (PlayerPrefs.HasKey(Forward))
    //    ////    SettingsVariables.keyDictionary[Forward] = (KeyCode)PlayerPrefs.GetInt(Forward);

    //    ////if (PlayerPrefs.HasKey(Left))
    //    ////    SettingsVariables.keyDictionary[Left] = (KeyCode)PlayerPrefs.GetInt(Left);

    //    ////if (PlayerPrefs.HasKey(Right))
    //    ////    SettingsVariables.keyDictionary[Right] = (KeyCode)PlayerPrefs.GetInt(Right);

    //    ////if (PlayerPrefs.HasKey(Fire))
    //    ////    SettingsVariables.keyDictionary[Fire] = (KeyCode)PlayerPrefs.GetInt(Fire);

    //    ////if (PlayerPrefs.HasKey(Pause))
    //    ////    SettingsVariables.keyDictionary[Pause] = (KeyCode)PlayerPrefs.GetInt(Pause);

    //    ////if (PlayerPrefs.HasKey(ChangeCam))
    //    ////    SettingsVariables.keyDictionary[ChangeCam] = (KeyCode)PlayerPrefs.GetInt(ChangeCam);

    //    ////if (PlayerPrefs.HasKey(HalveSpeed))
    //    ////    SettingsVariables.keyDictionary[HalveSpeed] = (KeyCode)PlayerPrefs.GetInt(HalveSpeed);
    //}
    //float LoadFloats(string item, float defaultValue = 1.0f)
    //{
    //    if (PlayerPrefs.HasKey(item))
    //        return PlayerPrefs.GetFloat(item);
    //    else
    //        return defaultValue;

    //    ////if (PlayerPrefs.HasKey(totalSound))
    //    ////{
    //    ////    SettingsVariables.sliderDictionary[totalSound] = PlayerPrefs.GetFloat(totalSound);
    //    ////}

    //    ////if (PlayerPrefs.HasKey(sfxSound))
    //    ////{
    //    ////    SettingsVariables.sliderDictionary[sfxSound] = PlayerPrefs.GetFloat(sfxSound);
    //    ////}

    //    ////if (PlayerPrefs.HasKey(musicSound))
    //    ////{
    //    ////    SettingsVariables.sliderDictionary[musicSound] = PlayerPrefs.GetFloat(musicSound);
    //    ////}

    //    ////if (PlayerPrefs.HasKey(camRotSpeed))
    //    ////    SettingsVariables.sliderDictionary[camRotSpeed] = PlayerPrefs.GetFloat(camRotSpeed);
    //}
    //bool LoadBools(string item, bool defaultValue)
    //{
    //    if (PlayerPrefs.HasKey(item))
    //        return System.Convert.ToBoolean(PlayerPrefs.GetInt(item));
    //    else
    //        return defaultValue;
    //    ////if (PlayerPrefs.HasKey(bPlayTutorial))
    //    ////    SettingsVariables.boolDictionary[bPlayTutorial] = System.Convert.ToBoolean(PlayerPrefs.GetInt(bPlayTutorial));

    //    ////if (PlayerPrefs.HasKey(bShootToActivate))
    //    ////    SettingsVariables.boolDictionary[bShootToActivate] = System.Convert.ToBoolean(PlayerPrefs.GetInt(bShootToActivate));

    //    ////if (PlayerPrefs.HasKey(bEnableCheckpoints))
    //    ////    SettingsVariables.boolDictionary[bEnableCheckpoints] = System.Convert.ToBoolean(PlayerPrefs.GetInt(bEnableCheckpoints));
    //}

    public static void SaveFloat(string value)
    {
        PlayerPrefs.SetFloat(value, SettingsVariables.sliderDictionary[value]);
    }

    public static void SaveInt(string value)
    {
        PlayerPrefs.SetInt(value, SettingsVariables.intDictionary[value]);
    }

    public static void SaveKey(string value)
    {
        PlayerPrefs.SetInt(value, (int)SettingsVariables.keyDictionary[value]);
    }

    public static void SaveBool(string value)
    {
        PlayerPrefs.SetInt(value, System.Convert.ToInt32(SettingsVariables.boolDictionary[value]));
    }


}
