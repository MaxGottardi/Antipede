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
    public GameObject PauseElementsHolder;

    public static bool enableKeyChange = false;
    string changeKey;
    TextMeshProUGUI changeText;

    public TextMeshProUGUI forwardTxt, leftTxt, rightTxt, halfTxt, camTxt, attackTxt, pauseTxt;
    public Toggle checkpointToogle, tutorialToggle, shootMenuToggle;

    [Header("Controls Toggles")]
    public Toggle weaponToggle;
    public Toggle attackToggle;
    public Toggle halveSpeedToggle;

    [Header("Graphics Toggles")]
    public Toggle solidBackToggle;
    void Awake()
    {
        soundPanel = GameObject.Find("SoundPanel");
        otherPanel = GameObject.Find("OtherPanel");
        controlsPanel = GameObject.Find("ControlsPanel");
        graphicsPanel = GameObject.Find("GraphicsPanel");
        RebindKeyPanel = GameObject.Find("KeyPanel");
        RebindKeyPanel.SetActive(false);

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

        checkpointToogle.isOn = SettingsVariables.boolDictionary["bEnableCheckpoints"];
        tutorialToggle.isOn = SettingsVariables.boolDictionary["bPlayTutorial"];
        shootMenuToggle.isOn = SettingsVariables.boolDictionary["bShootToActivate"];

        //Controls Toggles
        halveSpeedToggle.isOn = SettingsVariables.boolDictionary["bHalveSpeedToggle"];
        if(halveSpeedToggle.isOn)
            halveSpeedToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Halve Speed: Toggle";

        attackToggle.isOn = SettingsVariables.boolDictionary["bAttackToggle"];
        if (attackToggle.isOn)
            attackToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Attack: Toggle";

        weaponToggle.isOn = SettingsVariables.boolDictionary["bWeaponToggle"];
        if (weaponToggle.isOn)
            weaponToggle.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Weapon Fire: Toggle";

        //graphics toggles
        solidBackToggle.isOn = SettingsVariables.boolDictionary["bSolidTxtBackgrounds"];
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

    public void ToggleText(string defaultText)
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            bool toggleValue = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
            if (toggleValue) //adjust the toggles name to match its state
            {
                string textName = defaultText.Substring(0, defaultText.IndexOf(":") + 1); //get the first part of the toogles label, detailing its name
                selectedObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textName + " Toggle";
            }
            else
                selectedObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = defaultText;

            Debug.Log("Stuff is changed");
        }
    }
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
}
