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
				Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToSave();
				//SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
			}
			else if (gameObject.CompareTag("NewGame"))
			{
				//hasSeenChanged = true;
				GameObject.Find("SavingObjs").GetComponent<PersistentDataManager>().AbleMakeNewSaveGame();
				//LoadingScene.nextScene = "IntroCutScene";
				//LoadingScene.prevScene = "MainMenu";
				//PersistentDataManager.bIsNewGame = true;
				//SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
			}
			////////////////else if (gameObject.CompareTag("NewGame"))
			////////////////{
			////////////////	LoadingScene.nextScene = "IntroCutScene";
			////////////////	LoadingScene.prevScene = "MainMenu";
			////////////////	PersistentDataManager.bIsNewGame = true;
			////////////////	SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
			////////////////}
			else if (gameObject.CompareTag("Credits"))
			{
				Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToCredits();
				//SceneManager.LoadScene("Credits");
			}
			else if (gameObject.CompareTag("Quit"))
			{
				Application.Quit();
			}
			else if (gameObject.CompareTag("Back"))
			{
				Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToMainMenu();
				if (UIManager.enableKeyChange)
				{
					UIManager.RebindKeyPanel.SetActive(false);
					UIManager.colourPicker.SetActive(false);
					UIManager.enableKeyChange = false;
				}
				//SceneManager.LoadScene("MainMenu");
			}
			else if (gameObject.CompareTag("Sound"))
			{
				UIManager.soundPanel.SetActive(true);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(false);
				UIManager.graphicsPanel.SetActive(false);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.colourPicker.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Other"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(true);
				UIManager.controlsPanel.SetActive(false);
				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.colourPicker.SetActive(false);
				UIManager.graphicsPanel.SetActive(false);

				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Controls"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(true);
				UIManager.graphicsPanel.SetActive(false);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.colourPicker.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Graphics"))
			{
				UIManager.soundPanel.SetActive(false);
				UIManager.otherPanel.SetActive(false);
				UIManager.controlsPanel.SetActive(false);
				UIManager.graphicsPanel.SetActive(true);

				UIManager.RebindKeyPanel.SetActive(false);
				UIManager.colourPicker.SetActive(false);
				UIManager.enableKeyChange = false;
			}
			else if (gameObject.CompareTag("Settings"))
			{
				Camera.main.gameObject.GetComponent<CameraAnimate>().MoveToSettings();
				//SceneManager.LoadScene("SettingsScene");
			}
		}
	}
}
