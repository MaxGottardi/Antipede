using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    public static GameObject playerObj;
    public static MCentipedeBody mCentipedeBody;
    public static CameraController cameraController;
    public static GenerateGrid generateGrid;
    private void Awake()
    {
        playerObj = GameObject.Find("Centipede");
        mCentipedeBody = playerObj.GetComponent<MCentipedeBody>();
        cameraController = Camera.main.gameObject.GetComponent<CameraController>();
        generateGrid = Camera.main.gameObject.GetComponent<GenerateGrid>();
    }
}
