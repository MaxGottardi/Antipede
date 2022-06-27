using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextChildBackground : MonoBehaviour
{
    GameObject solidBackgroundObj;
    protected bool bHasValueChanged = false;
    void Start()
    {
        solidBackgroundObj = transform.GetChild(0).gameObject;
        UpdateBackgroundColours();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateVisual();
    }

    /// <summary>
    /// if the settings value has changed, update the visibility of the solid backgrounds
    /// </summary>
    protected void UpdateVisual()
    {
        if (bHasValueChanged != SettingsVariables.boolDictionary["bSolidTxtBackgrounds"])
        {
            UpdateBackgroundColours();
        }
    }

    protected virtual void UpdateBackgroundColours()
    {
        solidBackgroundObj.SetActive(SettingsVariables.boolDictionary["bSolidTxtBackgrounds"]);
        bHasValueChanged = SettingsVariables.boolDictionary["bSolidTxtBackgrounds"];
    }
}