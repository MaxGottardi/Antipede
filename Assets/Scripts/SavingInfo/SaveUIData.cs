using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveUIData : MonoBehaviour
{
    string directoryName;
    SaveableData saveableData;

    public TextMeshProUGUI saveNameTxt, modeTxt, lastPlayedTimeTxt;

    public void InitilizeData(string dirName, SaveFileInfo saveFileInfo)
    {
        this.directoryName = dirName;
        saveNameTxt.text = directoryName;
        if (saveFileInfo.saveableData != null)
        {
            this.saveableData = saveFileInfo.saveableData;
            modeTxt.text = "Game Mode: " + setModetxt(saveableData.gameSceneLoaded);
            lastPlayedTimeTxt.text = "Last Played: " + saveFileInfo.lastPlayedTime;
        }
        else
        {
            modeTxt.text = "!!!Warning Fill Corrupt, Cannot Load";
            lastPlayedTimeTxt.text = "";
        }
    }
    string setModetxt(string sceneName)
    {
        if (sceneName == "BossOnly3")
            return "Boss Only"; 
        
        if (sceneName == "Intermediate")
            return "Short Game";

        return "Full Game";
    }
    public void OnClick()
    {
        if (saveableData != null)
        {
            LoadingScene.gameSceneLoad = saveableData.gameSceneLoaded;
            LoadingScene.nextScene = saveableData.gameSceneLoaded;
            LoadingScene.prevScene = "MainMenu";
            PersistentDataManager.saveableData = saveableData;
            PersistentDataManager.directoryName = directoryName;
            PersistentDataManager.bIsNewGame = false;
            SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
        }
    }
    public void DestroySave()
    {
        GameObject.Find("SavingObjs").GetComponent<PersistentDataManager>().DeleteSaveFile(directoryName);
        Destroy(gameObject);
    }
}
