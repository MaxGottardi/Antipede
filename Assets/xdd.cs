using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class xdd : MonoBehaviour
{
    public float startTime;
    string prevScene = "LoadingScene";
    bool bSceneLoadStarted;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        bSceneLoadStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + 18f && !bSceneLoadStarted)
        {
            bSceneLoadStarted = true;
            StartCoroutine(LoadSceneAsync());
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("Environment Test");
        while (!loadScene.isDone)
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync(prevScene);

        //tween = new Tween(new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.identity, Quaternion.identity, Time.time, animDuration);
        //cam.SetActive(false);
    }
}
