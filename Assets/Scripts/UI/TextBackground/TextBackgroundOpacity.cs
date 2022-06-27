using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBackgroundOpacity : TextChildBackground
{
    [SerializeField] Color solidColour = Color.black, transparentColour;

    Image bkgrndImage;

    private void Start()
    {
        bkgrndImage = GetComponent<Image>();
        UpdateBackgroundColours();
    }
    protected override void Update()
    {
        UpdateVisual();
    }
    /// <summary>
    /// If enabled and not already set, assign the colour as transparent, otherwise, leave it as transparent
    /// </summary>
    protected override void UpdateBackgroundColours()
    {
        if (SettingsVariables.boolDictionary["bSolidTxtBackgrounds"])
            bkgrndImage.color = solidColour;
        else
            bkgrndImage.color = transparentColour;
        bHasValueChanged = SettingsVariables.boolDictionary["bSolidTxtBackgrounds"];
    }
}