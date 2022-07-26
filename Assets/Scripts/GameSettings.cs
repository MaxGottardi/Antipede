using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
	public static Action OnPause;
	public static Action OnResume;

	[SerializeField] GameObject MainPauseCanvas;

	[SerializeField] GameObject PauseElementsHolder;
	[SerializeField] GameObject SettingsElementsHolder;

	public Action<Settings> OnSettingsChanged;
	public Action<Settings> OnReceiveInspectorDefaults;

	public Settings Settings;

	bool bIsPaused;

	private GameObject[] checkPoints;
	private bool initialCheckpointDisable = false;


	void Start()
	{
		Settings = new Settings();

		OnReceiveInspectorDefaults += ReceiveDefaults;
		checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");

		MainPauseCanvas.SetActive(false);
		SettingsElementsHolder.SetActive(false);

		PauseElementsHolder.SetActive(true);

		OnResume?.Invoke();
	}

	void Update()
	{

		if (!initialCheckpointDisable)
        {
			foreach (GameObject checkpoint in checkPoints)
			{
				checkpoint.SetActive(false);
			}
			initialCheckpointDisable = true;
		}

		if (Input.GetKeyDown(SettingsVariables.keyDictionary["Pause"]))
		{
			// bIsPaused:
			// True if escape was pressed DURING PLAY - to pause the game.
			// False if escape was pressed DURING PAUSE - to resume the game.

			bIsPaused = !MainPauseCanvas.activeSelf;
			MainPauseCanvas.SetActive(bIsPaused);

			if (!bIsPaused)
			{
				DefaultState();
			}
			else
			{
				Time.timeScale = 0;
				OnPause?.Invoke();
			}
		}
	}

	public void DefaultState()
	{
		Time.timeScale = 1;
		MainPauseCanvas.SetActive(false);
		SettingsElementsHolder.SetActive(false);

		PauseElementsHolder.SetActive(true);

		OnResume?.Invoke();
	}

	public void ShowSettings(bool bShow)
	{
		SettingsElementsHolder.SetActive(bShow);
		PauseElementsHolder.SetActive(!bShow);
	}

	public void ToMain()
    {
		if (SceneManager.GetActiveScene().name == "Environment Test" || SceneManager.GetActiveScene().name == "BossOnly3"
					|| SceneManager.GetActiveScene().name == "Intermediate")
			Camera.main.GetComponent<PersistentDataManager>().SaveGame();

		Time.timeScale = 1;
        LoadingScene.nextScene = "MainMenu";
        LoadingScene.prevScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

	public void SetInheritRotation(bool bInheritRotation)
	{
		Settings.bInheritRotation = bInheritRotation;

		BroadcastSettingsChanged();
	}

	public void SetCameraMouseSensitivity(float InSensitivity)
	{
		Settings.CameraMouseSensitivity = InSensitivity;

		BroadcastSettingsChanged();
	}

	public void SetVolume(float InVolume)
	{
		Settings.Volume = InVolume;

		BroadcastSettingsChanged();
	}

	void BroadcastSettingsChanged()
	{
		OnSettingsChanged?.Invoke(Settings);
	}

	void ReceiveDefaults(Settings InspectorSettings)
	{
		Settings.bInheritRotation = InspectorSettings.bInheritRotation;
	}

	void OnDestroy()
	{
		OnPause = null;
		OnResume = null;

		OnSettingsChanged = null;
		OnReceiveInspectorDefaults = null;
	}
}

/// <summary>Global Game Settings.</summary>
public struct Settings
{
	// I know this holds one thing - there were more during testing, but they're not needed.
	// and I can't be bothered to change everything to fit one bool.
	//
	// Also, I figured this could hold every setting, so everything gets updated and all in
	// the one place.

	public bool bInheritRotation;
	public float CameraMouseSensitivity;
	public float Volume;

	public Settings(bool bInheritRotation, float CameraMouseSensitivity, float Volume)
	{
		this.bInheritRotation = bInheritRotation;
		this.CameraMouseSensitivity = CameraMouseSensitivity;
		this.Volume = Volume;
	}


	public Settings(bool bInheritRotation, float CameraMouseSensitivity) : this()
	{
		this.bInheritRotation = bInheritRotation;
		this.CameraMouseSensitivity = CameraMouseSensitivity;
	}
}
