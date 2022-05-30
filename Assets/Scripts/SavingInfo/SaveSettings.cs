using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSettings : MonoBehaviour
{
    //bool save
    const string bPlayTutorial = "bPlayTutorial";
    const string bShootToActivate = "bShootToActivate";
    const string bEnableCheckpoints = "bEnableCheckpoints";

    //key saves
    const string Forward = "Forward";
    const string Left = "Left";
    const string Right = "Right";
    const string Fire = "Fire";
    const string Pause = "Pause";
    const string ChangeCam = "ChangeCam";
    const string HalveSpeed = "HalveSpeed";

    //float saves
    const string totalSound = "totalSound";
    const string sfxSound = "sfxSound";
    const string musicSound = "musicSound";
    const string camRotSpeed = "camRotSpeed";

    public AudioMixer audioMixer;

    private void Awake()
    {
        LoadKeys();
        LoadFloats();
        LoadBools();
    }
    private void Start()
    {
        //adjust the audio volumns to match their values
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[totalSound], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[sfxSound], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(SettingsVariables.sliderDictionary[musicSound], 0.01f, 1)) * 40); //set the audio value so it is in the proper decible formate and between -80 and 0
    }
    // Start is called before the first frame update
    void LoadKeys()
    {
        if (PlayerPrefs.HasKey(Forward))
            SettingsVariables.keyDictionary[Forward] = (KeyCode)PlayerPrefs.GetInt(Forward);

        if (PlayerPrefs.HasKey(Left))
            SettingsVariables.keyDictionary[Left] = (KeyCode)PlayerPrefs.GetInt(Left);

        if (PlayerPrefs.HasKey(Right))
            SettingsVariables.keyDictionary[Right] = (KeyCode)PlayerPrefs.GetInt(Right);

        if (PlayerPrefs.HasKey(Fire))
            SettingsVariables.keyDictionary[Fire] = (KeyCode)PlayerPrefs.GetInt(Fire);

        if (PlayerPrefs.HasKey(Pause))
            SettingsVariables.keyDictionary[Pause] = (KeyCode)PlayerPrefs.GetInt(Pause);

        if (PlayerPrefs.HasKey(ChangeCam))
            SettingsVariables.keyDictionary[ChangeCam] = (KeyCode)PlayerPrefs.GetInt(ChangeCam);

        if (PlayerPrefs.HasKey(HalveSpeed))
            SettingsVariables.keyDictionary[HalveSpeed] = (KeyCode)PlayerPrefs.GetInt(HalveSpeed);
    }
    void LoadFloats()
    {
        if (PlayerPrefs.HasKey(totalSound))
        {
            SettingsVariables.sliderDictionary[totalSound] = PlayerPrefs.GetFloat(totalSound);
        }

        if (PlayerPrefs.HasKey(sfxSound))
        {
            SettingsVariables.sliderDictionary[sfxSound] = PlayerPrefs.GetFloat(sfxSound);
        }

        if (PlayerPrefs.HasKey(musicSound))
        {
            SettingsVariables.sliderDictionary[musicSound] = PlayerPrefs.GetFloat(musicSound);
        }

        if (PlayerPrefs.HasKey(camRotSpeed))
            SettingsVariables.sliderDictionary[camRotSpeed] = PlayerPrefs.GetFloat(camRotSpeed);
    }
    void LoadBools()
    {
        if (PlayerPrefs.HasKey(bPlayTutorial))
            SettingsVariables.boolDictionary[bPlayTutorial] = System.Convert.ToBoolean(PlayerPrefs.GetInt(bPlayTutorial));

        if (PlayerPrefs.HasKey(bShootToActivate))
            SettingsVariables.boolDictionary[bShootToActivate] = System.Convert.ToBoolean(PlayerPrefs.GetInt(bShootToActivate));

        if (PlayerPrefs.HasKey(bEnableCheckpoints))
            SettingsVariables.boolDictionary[bEnableCheckpoints] = System.Convert.ToBoolean(PlayerPrefs.GetInt(bEnableCheckpoints));
    }

    public static void SaveFloat(string value)
    {
        PlayerPrefs.SetFloat(value, SettingsVariables.sliderDictionary[value]);
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
