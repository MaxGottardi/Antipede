using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : MonoBehaviour
{
    public static GameObject soundPanel, otherPanel, controlsPanel, graphicsPanel;
    public static GameObject RebindKeyPanel;
    [HideInInspector] public GameObject PauseElementsHolder, colourPicker;

    public static bool enableKeyChange = false;
    string changeKey;
    TextMeshProUGUI changeText;

    public TextMeshProUGUI forwardTxt, leftTxt, rightTxt, halfTxt, camTxt, attackTxt, pauseTxt;
    public Toggle checkpointToggle, tutorialToggle, shootMenuToggle;

    [Header("Controls Toggles")]
    public Toggle weaponToggle;
    public Toggle attackToggle;
    public Toggle halveSpeedToggle;
    public Toggle forwardMoveToggle;

    [Header("Graphics Toggles")]
    public Toggle solidBackToggle;
    public Toggle motionBlurToggle;
    public Toggle glowToggle;
    public Toggle vSyncToggle;
    public Toggle camFollowToggle;

    PostProcessVolume postProcessVolume;

    enum MaterialIDEnum
    {
        antAntenna,
        spiderHealth
    };
    int materialID;

    public Material antennaMat, spiderMat, spiderSliderMat;
    public Image antennaImg, spiderImg;

    [Header("Dropdowns")]
    public TMP_Dropdown viewModeDropDown;
    public TMP_Dropdown shadowDropDown;
    public TMP_Dropdown textureDropDown;

    [Header("ColorBtns")]
    //colour buttons
    public Button blackBtn;
    public Button whiteBtn;
    public Button lGrayBtn;
    public Button dGrayBtn;
    public Button redBtn;
    public Button yellowBtn;
    public Button orangeBtn;
    public Button lGreenBtn;
    public Button greenBtn;
    public Button cyanBtn;
    public Button blueBtn;
    public Button purpleBtn;
    public Texture2D blackTxture, whiteTxture, lGrayTxture, dGrayTxture, redTxture, yellowTxture, orangeTxture, lGreenTxture, greenTxture, cyanTxture, blueTxture, purpleTxture;
    void Awake()
    {
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();

        soundPanel = GameObject.Find("SoundPanel");
        otherPanel = GameObject.Find("OtherPanel");
        controlsPanel = GameObject.Find("ControlsPanel");
        graphicsPanel = GameObject.Find("GraphicsPanel");
        colourPicker = GameObject.Find("ColourPanel");
        RebindKeyPanel = GameObject.Find("KeyPanel");
        RebindKeyPanel.SetActive(false);
        colourPicker.SetActive(false);

        otherPanel.SetActive(false);
        soundPanel.SetActive(true);
        controlsPanel.SetActive(false);
        graphicsPanel.SetActive(false);

        AssignValues();
    }
    void AssignValues()
    {
        forwardTxt.text = SettingsVariables.keyDictionary["Forward"].ToString();
        leftTxt.text = SettingsVariables.keyDictionary["Left"].ToString();
        rightTxt.text = SettingsVariables.keyDictionary["Right"].ToString();
        halfTxt.text = SettingsVariables.keyDictionary["HalveSpeed"].ToString();
        camTxt.text = SettingsVariables.keyDictionary["ChangeCam"].ToString();
        attackTxt.text = SettingsVariables.keyDictionary["Fire"].ToString();
        pauseTxt.text = SettingsVariables.keyDictionary["Pause"].ToString();

        checkpointToggle.isOn = SettingsVariables.boolDictionary["bEnableCheckpoints"];
        tutorialToggle.isOn = SettingsVariables.boolDictionary["bPlayTutorial"];
        shootMenuToggle.isOn = SettingsVariables.boolDictionary["bShootToActivate"];

        //Controls Toggles
        halveSpeedToggle.isOn = SettingsVariables.boolDictionary["bHalveSpeedToggle"];
        if (halveSpeedToggle.isOn)
            halveSpeedToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Toggle";

        attackToggle.isOn = SettingsVariables.boolDictionary["bAttackToggle"];
        if (attackToggle.isOn)
            attackToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Toggle";

        weaponToggle.isOn = SettingsVariables.boolDictionary["bWeaponToggle"];
        if (weaponToggle.isOn)
            weaponToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Toggle";

        forwardMoveToggle.isOn = SettingsVariables.boolDictionary["bForwardMoveToggle"];
        if (forwardMoveToggle.isOn)
            forwardMoveToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Toggle";

        //graphics toggles
        solidBackToggle.isOn = SettingsVariables.boolDictionary["bSolidTxtBackgrounds"];
        if (solidBackToggle.isOn)
            solidBackToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Enabled"; 
        
        camFollowToggle.isOn = SettingsVariables.boolDictionary["bCamFollow"];
        if (camFollowToggle.isOn)
            camFollowToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Enabled";

        motionBlurToggle.isOn = SettingsVariables.boolDictionary["bMotionBlur"];
        if (motionBlurToggle.isOn)
            motionBlurToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Enabled";
        EnableMotionBlur();

        glowToggle.isOn = SettingsVariables.boolDictionary["bItemGlow"];
        if (glowToggle.isOn)
            glowToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Enabled";
        EnableGlow();

        vSyncToggle.isOn = SettingsVariables.boolDictionary["bVSync"];
        if (vSyncToggle.isOn)
            vSyncToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Enabled";
        EnableVSync();

        viewModeDropDown.value = SettingsVariables.intDictionary["ViewMode"];
        AdjustViewMode();
        viewModeDropDown.captionText.text = viewModeDropDown.options[SettingsVariables.intDictionary["ViewMode"]].text;
        viewModeDropDown.RefreshShownValue();

        shadowDropDown.value = SettingsVariables.intDictionary["ShadowRes"];
        AdjustShadows();
        shadowDropDown.captionText.text = shadowDropDown.options[SettingsVariables.intDictionary["ShadowRes"]].text;
        shadowDropDown.RefreshShownValue();

        textureDropDown.value = SettingsVariables.intDictionary["TextureRes"];
        AdjustTextures();
        textureDropDown.captionText.text = shadowDropDown.options[SettingsVariables.intDictionary["TextureRes"]].text;
        textureDropDown.RefreshShownValue();
    }

    /// <summary>
    /// for all the colour buttons, setup their appropriate on click events
    /// </summary>
    void SetupColourButtons()
    {
        blackBtn.onClick.AddListener(() => SetMaterialColour(blackBtn.gameObject.GetComponent<Image>(), blackTxture));
        whiteBtn.onClick.AddListener(() => SetMaterialColour(whiteBtn.gameObject.GetComponent<Image>(), whiteTxture));
        lGrayBtn.onClick.AddListener(() => SetMaterialColour(lGrayBtn.gameObject.GetComponent<Image>(), lGrayTxture));
        dGrayBtn.onClick.AddListener(() => SetMaterialColour(dGrayBtn.gameObject.GetComponent<Image>(), dGrayTxture));

        redBtn.onClick.AddListener(() => SetMaterialColour(redBtn.gameObject.GetComponent<Image>(), redTxture));
        yellowBtn.onClick.AddListener(() => SetMaterialColour(yellowBtn.gameObject.GetComponent<Image>(), yellowTxture));
        orangeBtn.onClick.AddListener(() => SetMaterialColour(orangeBtn.gameObject.GetComponent<Image>(), orangeTxture));

        lGreenBtn.onClick.AddListener(() => SetMaterialColour(lGreenBtn.gameObject.GetComponent<Image>(), lGreenTxture));
        greenBtn.onClick.AddListener(() => SetMaterialColour(greenBtn.gameObject.GetComponent<Image>(), greenTxture));
        cyanBtn.onClick.AddListener(() => SetMaterialColour(cyanBtn.gameObject.GetComponent<Image>(), cyanTxture));
        blueBtn.onClick.AddListener(() => SetMaterialColour(blueBtn.gameObject.GetComponent<Image>(), blueTxture));
        purpleBtn.onClick.AddListener(() => SetMaterialColour(purpleBtn.gameObject.GetComponent<Image>(), purpleTxture));
    }

    private void Start()
    {
        InitilizeColours();
        SetupColourButtons();
    }
    public void GameControls()
    {
        otherPanel.SetActive(false);
        soundPanel.SetActive(false);
        controlsPanel.SetActive(true);
        AssignValues();
    }
    public void GameAudio()
    {
        otherPanel.SetActive(false);
        soundPanel.SetActive(true);
        controlsPanel.SetActive(false);
        AssignValues();
    }
    public void GameOther()
    {
        otherPanel.SetActive(true);
        soundPanel.SetActive(false);
        controlsPanel.SetActive(false);
        AssignValues();
    }
    public void GameBack(GameObject obj)
    {
        obj.SetActive(false);
        PauseElementsHolder.SetActive(true);
    }

    public void ChangeKey(string key)
    {
        enableKeyChange = true;
        changeKey = key;
    }
    public void AssignText(TextMeshProUGUI text)
    {
        changeText = text;
        RebindKeyPanel.SetActive(true);
    }

    public void ChangeBool(string component)
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            bool toggleValue = selectedObj.GetComponent<Toggle>().isOn;
            SettingsVariables.boolDictionary[component] = toggleValue;
            SaveSettings.SaveBool(component);

        }
    }

    /// <summary>
    /// saves the new dropdown value
    /// </summary>
    /// <param name="component">the saved value to update</param>
    public void ChangeInt(string component)
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            int dropdownValue = selectedObj.transform.parent.parent.parent.parent.gameObject.GetComponent<TMP_Dropdown>().value; //as when a dropdown changes the selected item is not the parent, move up the higherachy to it
            SettingsVariables.intDictionary[component] = dropdownValue;
            SaveSettings.SaveInt(component);

        }
    }

    /// <summary>
    /// when a toggle changes, update its text if required
    /// </summary>
    /// <param name="defaultText">The default setting for the toggle</param>
    public void ToggleText(string defaultText)
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            bool toggleValue = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
            if (toggleValue) //adjust the toggles name to match its state
            {
                //string textName = defaultText.Substring(0, defaultText.IndexOf(":") + 1); //get the first part of the toogles label, detailing its name
                selectedObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Toggle";
            }
            else
                selectedObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = defaultText;
        }
    }

    /// <summary>
    /// same as above, though switches the text between enabled and disabled
    /// </summary>
    /// <param name="defaultText"></param>
    public void ToggleTextDefault()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            bool toggleValue = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
            if (toggleValue) //adjust the toggles name to match its state
            {
                //string textName = defaultText.Substring(0, defaultText.IndexOf(":") + 1); //get the first part of the toogles label, detailing its name
                selectedObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Enabled";
            }
            else
                selectedObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Disabled";
        }
    }

    /// <summary>
    /// whenever a key is pressed down and it has been allowed, update the key to the new value
    /// </summary>
    private void OnGUI()
    {
        if (enableKeyChange)
        {
            Event e = Event.current;
            if (e != null && e.isKey)
            {
                RebindKeyPanel.SetActive(false);
                SettingsVariables.keyDictionary[changeKey] = e.keyCode;
                changeText.text = e.keyCode.ToString();
                enableKeyChange = false;
                SaveSettings.SaveKey(changeKey);
                if (GameManager1.uiButtons != null)
                {
                    GameManager1.uiButtons.UpdateControlText();
                }
            }
        }
    }

    public void EnableColourPick(int materialID)
    {
        this.materialID = materialID;
        colourPicker.SetActive(true);
    }

    public void SetMaterialColour(Image image, Texture2D spiderTexture)
    {
        Color imgColour = image.color;

        switch (materialID)
        {
            case (int)MaterialIDEnum.antAntenna:
                antennaMat.SetColor("_Color", imgColour);
                antennaMat.SetColor("_EmissionColor", imgColour);
                antennaImg.color = imgColour;
                SettingsVariables.sliderDictionary["antennaColourR"] = imgColour.r;
                SaveSettings.SaveFloat("antennaColourR");
                SettingsVariables.sliderDictionary["antennaColourG"] = imgColour.g;
                SaveSettings.SaveFloat("antennaColourG");
                SettingsVariables.sliderDictionary["antennaColourB"] = imgColour.b;
                SaveSettings.SaveFloat("antennaColourB");
                break;
            case (int)MaterialIDEnum.spiderHealth:
                spiderSliderMat.SetColor("_Color", image.color);
                spiderSliderMat.SetColor("_EmissionColor", imgColour);
                spiderMat.SetTexture("_MainTex", spiderTexture);

                spiderImg.color = imgColour;
                SettingsVariables.sliderDictionary["spiderColourR"] = imgColour.r;
                SaveSettings.SaveFloat("spiderColourR");
                SettingsVariables.sliderDictionary["spiderColourG"] = imgColour.g;
                SaveSettings.SaveFloat("spiderColourG");
                SettingsVariables.sliderDictionary["spiderColourB"] = imgColour.b;
                SaveSettings.SaveFloat("spiderColourB");
                break;
            default:
                break;
        }
        colourPicker.SetActive(false);
    }

    void InitilizeColours()
    {
        Color antennaColour = new Color(SettingsVariables.sliderDictionary["antennaColourR"], SettingsVariables.sliderDictionary["antennaColourG"], SettingsVariables.sliderDictionary["antennaColourB"]);
        antennaMat.SetColor("_Color", antennaColour);
        antennaMat.SetColor("_EmissionColor", antennaColour);
        antennaImg.color = antennaColour;

        Color spiderColour = new Color(SettingsVariables.sliderDictionary["spiderColourR"], SettingsVariables.sliderDictionary["spiderColourG"], SettingsVariables.sliderDictionary["spiderColourB"]);
        spiderSliderMat.SetColor("_Color", spiderColour);
        spiderImg.color = spiderColour;
    }

    /// <summary>
    /// When clicked, either expand or collapse the current section so users do not need to worry about those settings any more
    /// </summary>
    /// <param name="section">The current section of objects to either show or hide</param>
    public void ShowHideSection(GameObject section)
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        section.SetActive(!section.activeSelf);
        if (selectedObj != null)
        {
            TextMeshProUGUI text = selectedObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            text.text = section.activeSelf ? "[Collapse]" : "[Expand]";
        }
    }

    public void AdjustViewMode()
    {
        switch (viewModeDropDown.value)
        {
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            default://case 0
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
    }

    public void AdjustShadows()
    {
        switch (shadowDropDown.value)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.High;
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                break;
            case 3:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.Low;
                break;
            default://case 4
                QualitySettings.shadows = ShadowQuality.Disable;
                break;
        }
    }
    public void AdjustTextures()
    {
        switch (textureDropDown.value)
        {
            case 0:
                QualitySettings.masterTextureLimit = 0;
                break;
            case 1:
                QualitySettings.masterTextureLimit = 1;
                break;
            case 2:
                QualitySettings.masterTextureLimit = 2;
                break;
            default://case 4
                QualitySettings.masterTextureLimit = 3;
                break;
        }
    }

    public void EnableMotionBlur()
    {
        postProcessVolume.profile.GetSetting<MotionBlur>().enabled.value = SettingsVariables.boolDictionary["bMotionBlur"];
    }

    public void EnableGlow()
    {
        postProcessVolume.profile.GetSetting<Bloom>().enabled.value = SettingsVariables.boolDictionary["bItemGlow"];
    }

    public void EnableVSync()
    {
        if (SettingsVariables.boolDictionary["bVSync"])
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }
}
