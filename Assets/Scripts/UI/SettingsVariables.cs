using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsVariables
{
    public static Dictionary<string, bool> boolDictionary = new Dictionary<string, bool>() //load in the default values for each setting
        {
            {"bPlayTutorial", LoadBools("bPlayTutorial", true) },
            {"bShootToActivate", LoadBools("bShootToActivate", true) },
            {"bEnableCheckpoints", LoadBools("bEnableCheckpoints", false) },

            {"bAttackToggle",  LoadBools("bAttackToggle", false) },
            {"bHalveSpeedToggle",  LoadBools("bHalveSpeedToggle", false) },
            {"bWeaponToggle",  LoadBools("bWeaponToggle", false) }
        };
    public static Dictionary<string, float> sliderDictionary = new Dictionary<string, float>()
        {
            {"totalSound", LoadFloats("totalSound") },
            {"sfxSound", LoadFloats("sfxSound") },
            {"musicSound", LoadFloats("musicSound") },

            {"camRotSpeed", LoadFloats("camRotSpeed") },
            {"zoomSpeed", LoadFloats("zoomSpeed") }

        };

    public static Dictionary<string, KeyCode> keyDictionary = new Dictionary<string, KeyCode>()
        {
            {"Forward", LoadKeys("Forward", KeyCode.W)},
            {"Left", LoadKeys("Left", KeyCode.A)},
            {"Right", LoadKeys("Right", KeyCode.D)},

            {"Fire", LoadKeys("Fire", KeyCode.Space)},
            {"Pause", LoadKeys("Pause", KeyCode.Escape)},
            {"ChangeCam", LoadKeys("ChangeCam", KeyCode.T)},
            {"HalveSpeed", LoadKeys("HalveSpeed", KeyCode.LeftShift)}
        };

    static bool LoadBools(string item, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(item))
            return System.Convert.ToBoolean(PlayerPrefs.GetInt(item));
        else
            return defaultValue;
    }

    static float LoadFloats(string item, float defaultValue = 1.0f)
    {
        if (PlayerPrefs.HasKey(item))
            return PlayerPrefs.GetFloat(item);
        else
            return defaultValue;

    }

    static KeyCode LoadKeys(string item, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(item))
            return (KeyCode)PlayerPrefs.GetInt(item);
        else
            return defaultKey;
    }
}