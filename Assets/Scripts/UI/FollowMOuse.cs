using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowMOuse : MonoBehaviour
{
    public GameObject outline;

    // Update is called once per frame
    private void OnMouseEnter()
    {
        outline.SetActive(true);
    }

    private void OnMouseExit()
    {
        outline.SetActive(false);
    }

    private void OnMouseOver()
    {
		if (Input.GetMouseButtonDown(0) && !SettingsVariables.boolDictionary["bShootToActivate"])
		{
			if (gameObject.CompareTag("Play"))
			{
				SceneManager.LoadScene("Environment Test");
			}
			else if (gameObject.CompareTag("Back"))
			{
				SceneManager.LoadScene("MainMenu");
			}
			else if (gameObject.CompareTag("Sound"))
			{
				UIManager.soundPanel.SetActive(true);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(false);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Other"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(true);
				UIManager.controlsPanel.SetActive(false);
				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Controls"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(true);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Settings"))
			{
				SceneManager.LoadScene("SettingsScene");
			}
		}
	}
}
