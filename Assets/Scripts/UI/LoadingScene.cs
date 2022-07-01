using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static string prevScene, nextScene, gameSceneLoad;
    Tween tween;
    public float animDuration = 2;
    public GameObject loadTxtObj, tweenLeftObj, tweenRightObj, cam;

    bool reverse = false;
    void Start()
    {
        ////loadTxtObj.SetActive(false);
        ////tweenLeftObj.transform.localScale = new Vector3(0, 1, 1);
        ////tweenRightObj.transform.localScale = new Vector3(0, 1, 1);

        ChangeScene();
    }

    public void ChangeScene()
    {
        reverse = false;
        tween = new Tween(new Vector3(0, 1, 1), new Vector3(1, 1, 1), Quaternion.identity, Quaternion.identity, Time.time, animDuration);
        loadTxtObj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(tween != null)
        {
            tweenLeftObj.transform.localScale = tween.UpdatePosition();
            tweenRightObj.transform.localScale = tween.UpdatePosition();
            if (!reverse&& (Time.time - tween.StartTime) / tween.Duration > 0.99f)
            {
                tweenLeftObj.transform.localScale = new Vector3(1.1f, 1, 1);
                tweenRightObj.transform.localScale = new Vector3(1.1f, 1, 1);
                if (cam != null)
                {
                    cam.SetActive(true);
                }
                reverse = true;
                StartCoroutine(LoadSceneAsync());
            }

            ////////////if(reverse && tweenLeftObj.transform.localScale.x < 0.01f)
            ////////////{
            ////////////    reverse = false;
            ////////////    tweenLeftObj.transform.localScale = new Vector3(0,1,1);
            ////////////    tweenRightObj.transform.localScale = new Vector3(0,1,1);
            ////////////    SceneManager.UnloadSceneAsync("LoadingScene");
            ////////////    loadTxtObj.SetActive(false);
            ////////////    tween = null;
            ////////////}
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(nextScene);
        while(!loadScene.isDone)
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync(prevScene);

        //tween = new Tween(new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.identity, Quaternion.identity, Time.time, animDuration);
        //cam.SetActive(false);
    }
}
