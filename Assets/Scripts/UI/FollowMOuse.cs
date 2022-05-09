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
}
