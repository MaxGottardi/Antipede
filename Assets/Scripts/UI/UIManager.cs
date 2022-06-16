using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static GameObject soundPanel, otherPanel, controlsPanel, graphicsPanel;
    public static GameObject RebindKeyPanel;
    [HideInInspector]public GameObject PauseElementsHolder, colourPicker;

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

    enum MaterialIDEnum
    {
        antAntenna,
        spiderHealth
    };
    int materialID;

    public Material antennaMat, spiderMat, spiderSliderMat;
    public Image antennaImg, spiderImg;

    //colour buttons
    public Button blackBtn, whiteBtn, lGrayBtn, dGrayBtn, redBtn, yellowBtn, orangeBtn, lGreenBtn, greenBtn, cyanBtn, blueBtn, purpleBtn;
    public Texture2D blackTxture, whiteTxture, lGrayTxture, dGrayTxture, redTxture, yellowTxture, orangeTxture, lGreenTxture, greenTxture, cyanTxture, blueTxture, purpleTxture;
    void Awake()
    {
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
        if(halveSpeedToggle.isOn)
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
            bool toggleValue = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
            SettingsVariables.boolDictionary[component] = toggleValue;
            SaveSettings.SaveBool(component);

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
        if(enableKeyChange)
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

    public void ShowHideSection(GameObject section)
    {
        section.SetActive(!section.activeSelf);
    }
}
