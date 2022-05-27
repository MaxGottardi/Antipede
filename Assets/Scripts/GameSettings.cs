using System;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
	[SerializeField] GameObject MainPauseCanvas;

	[SerializeField] GameObject PauseElementsHolder;
	[SerializeField] GameObject SettingsElementsHolder;

	public Action<Settings> OnSettingsChanged;
	public Action<Settings> OnReceiveInspectorDefaults;

	public Settings Settings;

	bool bIsPaused;

	private GameObject[] checkPoints;


	void Awake()
	{
		Settings = new Settings();

		OnReceiveInspectorDefaults += ReceiveDefaults;
		checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
		foreach (GameObject checkpoint in checkPoints)
		{
			//checkpoint.SetActive(false);
		}
		DefaultState();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// True if escape was pressed DURING PLAY - to pause the game.
			// False if escape was pressed DURING PAUSE - to resume the game.
			bIsPaused = true;

			bIsPaused = !MainPauseCanvas.activeSelf;
			MainPauseCanvas.SetActive(bIsPaused);

			if (!bIsPaused)
			{
				Time.timeScale = 1;
				DefaultState();
			}
			else
				Time.timeScale = 0;
		}
	}

	void DefaultState()
	{
		MainPauseCanvas.SetActive(false);
		SettingsElementsHolder.SetActive(false);

		PauseElementsHolder.SetActive(true);
	}

	public void ShowSettings(bool bShow)
	{
		SettingsElementsHolder.SetActive(bShow);
		PauseElementsHolder.SetActive(!bShow);
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
