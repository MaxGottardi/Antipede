using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveUIData : MonoBehaviour
{
    string directoryName;
    SaveableData saveableData;

    public TextMeshProUGUI saveNameTxt;

    public void InitilizeData(string dirName, SaveableData saveableData)
    {
        this.directoryName = dirName;
        this.saveableData = saveableData;
        saveNameTxt.text = directoryName;
    }

    public void OnClick()
    {
        LoadingScene.nextScene = "Environment Test";
        LoadingScene.prevScene = "MainMenu";
        PersistentDataManager.saveableData = saveableData;
        PersistentDataManager.directoryName = directoryName;
        PersistentDataManager.bIsNewGame = false;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }
    public void DestroySave()
    {
        GameObject.Find("SavingObjs").GetComponent<PersistentDataManager>().DeleteSaveFile(directoryName);
        Destroy(gameObject);
    }
}
