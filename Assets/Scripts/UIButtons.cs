using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChangeScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }
}
