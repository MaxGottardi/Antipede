using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to update the tutorials text to match the players specified setting
/// </summary>
public class AdjustTutTxt : MonoBehaviour
{
    public string defaultTxt, enabledTxt, settingName;
    Text text;
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SettingsVariables.boolDictionary[settingName] && text.text != enabledTxt)
            text.text = enabledTxt; 
        else if (!SettingsVariables.boolDictionary[settingName] && text.text != defaultTxt)
            text.text = defaultTxt;
    }
}
