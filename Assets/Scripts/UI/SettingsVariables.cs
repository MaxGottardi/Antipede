using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsVariables
{
    public static float totalSound = 1, sfxSound = 1, musicSound = 1;

    public static Dictionary<string, bool> boolDictionary = new Dictionary<string, bool>()
    {
        {"bPlayTutorial", true },
        {"bShootToActivate", true },
        {"bEnableCheckpoints", false }
    };



    public static Dictionary<string, float> sliderDictionary = new Dictionary<string, float>()
    {
        {"totalSound", 1.0f },
        {"sfxSound", 1.0f },
        {"musicSound", 1.0f },

        {"camRotSpeed", 1.0f },
        {"zoomSpeed", 1.0f }

    };

    public static Dictionary<string, KeyCode> keyDictionary = new Dictionary<string, KeyCode>()
    {
        { "Forward", KeyCode.W},
        { "Left", KeyCode.A},
        { "Right", KeyCode.D},

        { "Fire", KeyCode.Space},
        { "Pause", KeyCode.Escape},
        { "ChangeCam", KeyCode.T},
        { "HalveSpeed", KeyCode.LeftShift}
    };
}
