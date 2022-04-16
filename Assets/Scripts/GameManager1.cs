using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    public static GameObject playerObj;
    public static CameraController cameraController;
    private void Awake()
    {
        playerObj = GameObject.Find("Centipede");
        cameraController = Camera.main.gameObject.GetComponent<CameraController>();
    }
}
