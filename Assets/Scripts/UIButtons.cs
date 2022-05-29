using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    public GameObject tutWindow, winWindow, spiderWindow;

    public GameObject moveUI, CamUI, pauseUI, attackUI, shootUI, addWeaponUI, speedUI;

#if UNITY_EDITOR
        // Skips needing the click begin and continue because I'm lazy.
        // -- This is developer only and won't be included in an actual build. -- \\
        /// <see cref="GameManager1.Update"/>
        public GameObject Dev_Story_Skip;
#endif

        bool seenSpeed = false, seenAttack = false, seenShoot = false, seenSpider = false;

    public int speedIncrease = 0, segmentIncrease = 0, segmentDegrease = 0;
    public Text speedInfo, segAddInfo, segDecreaseInfo;

    public void Win()
    {
        Time.timeScale = 0;
        winWindow.SetActive(true);
    }

    // Start is called before the first frame update
    public void AddSegment()
    {
        if (segAddInfo.color.a <= 0.1)
        {
            segmentIncrease = 0;
            segAddInfo.color = new Color(segAddInfo.color.r, segAddInfo.color.g, segAddInfo.color.b, 1);
        }
        segmentIncrease++;
        segAddInfo.gameObject.SetActive(false);
        segAddInfo.gameObject.SetActive(true);
        segAddInfo.text = "Added Segment(x" + segmentIncrease + ")";
    }
    //xd

    public void RemoveSegment()
    {
        if (segDecreaseInfo.color.a <= 0.1)
        {
            segmentDegrease = 0;
            segDecreaseInfo.color = new Color(segAddInfo.color.r, segAddInfo.color.g, segAddInfo.color.b, 1);
        }
        segmentDegrease++;
        segDecreaseInfo.gameObject.SetActive(false);
        segDecreaseInfo.gameObject.SetActive(true);
        segDecreaseInfo.text = "Removed Segment(x" + segmentDegrease + ")";
    }

    public void AddSpeed()
    {
        if (speedInfo.color.a <= 0.1)
        {
            speedIncrease = 0;
            speedInfo.color = new Color(segAddInfo.color.r, segAddInfo.color.g, segAddInfo.color.b, 1);
        }
        speedIncrease++;
        speedInfo.gameObject.SetActive(false);
        speedInfo.gameObject.SetActive(true);
        speedInfo.text = "Increased Speed(x" + speedIncrease + ")";
    }

    public void ChangeScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void StoryFinished(GameObject View)
    {
        View.SetActive(false);
        if (!SettingsVariables.boolDictionary["bPlayTutorial"])
            Time.timeScale = 1;
    }

    public void Continue()
    {
        Time.timeScale = 1;
        tutWindow.SetActive(false);
        spiderWindow.SetActive(false);
    }

    //below are the functions for enabling and disabling the other UI elements
    public void StartUI()
    {
        if (SettingsVariables.boolDictionary["bPlayTutorial"])
        {
            tutWindow.SetActive(true);
            Time.timeScale = 0;
            moveUI.SetActive(true);
            pauseUI.SetActive(true);
            CamUI.SetActive(true);
        }
        else
        {
            tutWindow.SetActive(false);
            moveUI.SetActive(false);
            pauseUI.SetActive(false);
            CamUI.SetActive(false);
        }
    }

    public void AttackUI()
    {
        if (!seenAttack && SettingsVariables.boolDictionary["bPlayTutorial"])
        {
            seenAttack = true;
            tutWindow.SetActive(true);
            Time.timeScale = 0;
            moveUI.SetActive(false);
            pauseUI.SetActive(false);
            CamUI.SetActive(false);
            attackUI.SetActive(true);
            shootUI.SetActive(false);
            addWeaponUI.SetActive(false);
            speedUI.SetActive(false);
        }
    }

    public void SpiderInfo()
    {
        if (!seenSpider)
        {
            Time.timeScale = 0;
            spiderWindow.SetActive(true);
            seenSpider = true;
        }
    }

    public void ShootUI()
    {
        if (!seenShoot && SettingsVariables.boolDictionary["bPlayTutorial"])
        {
            seenShoot = true;
            tutWindow.SetActive(true);
            Time.timeScale = 0;
            moveUI.SetActive(false);
            pauseUI.SetActive(false);
            CamUI.SetActive(false);
            attackUI.SetActive(false);
            shootUI.SetActive(true);
            addWeaponUI.SetActive(true);
            speedUI.SetActive(false);
        }
    }

    public void SpeedUI()
    {
        if (!seenSpeed && SettingsVariables.boolDictionary["bPlayTutorial"])
        {
            seenSpeed = true;
            tutWindow.SetActive(true);
            Time.timeScale = 0;
            moveUI.SetActive(false);
            pauseUI.SetActive(false);
            CamUI.SetActive(false);
            attackUI.SetActive(false);
            shootUI.SetActive(false);
            addWeaponUI.SetActive(false);
            speedUI.SetActive(true);
        }
    }
}