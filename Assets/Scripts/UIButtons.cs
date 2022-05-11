using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public GameObject tutWindow;

    public GameObject moveUI, CamUI, pauseUI, attackUI, shootUI, addWeaponUI, speedUI;

    bool seenSpeed = false, seenAttack = false, seenShoot = false;
    // Start is called before the first frame update
    public void ChangeScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        tutWindow.SetActive(false);
    }

    //below are the functions for enabling and disabling the other UI elements
    public void StartUI()
    {
        tutWindow.SetActive(true);
        Time.timeScale = 0;
        moveUI.SetActive(true);
        pauseUI.SetActive(true);
        CamUI.SetActive(true);
    }

    public void AttackUI()
    {
        if (!seenAttack)
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

    public void ShootUI()
    {
        if (!seenShoot)
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
        if (!seenSpeed)
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