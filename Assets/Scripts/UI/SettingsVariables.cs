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
            {"bWeaponToggle",  LoadBools("bWeaponToggle", false) },
            {"bForwardMoveToggle",  LoadBools("bForwardMoveToggle", false) },

            {"bSolidTxtBackgrounds",  LoadBools("bSolidTxtBackgrounds", false) },


            {"bMotionBlur",  LoadBools("bMotionBlur", true) },
            {"bItemGlow",  LoadBools("bItemGlow", true) },
            {"bVSync",  LoadBools("bVSync", true) },
            {"bCamFollow",  LoadBools("bCamFollow", true) }
        };
    public static Dictionary<string, int> intDictionary = new Dictionary<string, int>()
    {
        { "ViewMode", LoadInts("ViewMode")},
        { "ShadowRes", LoadInts("ShadowRes", 1)},
        { "TextureRes", LoadInts("TextureRes", 0)}
    };
    public static Dictionary<string, float> sliderDictionary = new Dictionary<string, float>()
        {
            //sound
            {"totalSound", LoadFloats("totalSound", 100) },
            {"sfxSound", LoadFloats("sfxSound", 100) },
            {"musicSound", LoadFloats("musicSound", 100) },

            //controls
            {"camRotSpeed", LoadFloats("camRotSpeed", 100) },
            {"zoomSpeed", LoadFloats("zoomSpeed", 100) },

            //lighting
            {"fogPercentage", LoadFloats("fogPercentage", 100) },
            {"brightnessMultiplier", LoadFloats("brightnessMultiplier", 0) },

            //graphics
            {"camFOV", LoadFloats("camFOV", 60) },


            //colours
            {"antennaColourR", LoadFloats("antennaColourR", 255/255) },            
            {"antennaColourG", LoadFloats("antennaColourG", 233/255) },            
            {"antennaColourB", LoadFloats("antennaColourB", 0) },

            {"spiderColourR", LoadFloats("spiderColourR", 0) },
            {"spiderColourG", LoadFloats("spiderColourG", 250/255) },
            {"spiderColourB", LoadFloats("spiderColourB", 0) }


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
    static int LoadInts(string item, int defaultValue = 0)
    {
        if (PlayerPrefs.HasKey(item))
            return PlayerPrefs.GetInt(item);
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