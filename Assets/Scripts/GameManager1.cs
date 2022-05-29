using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    public static GameObject playerObj;
    public static MCentipedeBody mCentipedeBody;
    public static CameraController cameraController;
    public static GenerateGrid generateGrid;
    public static UIButtons uiButtons;
    private void Awake()
    {
        playerObj = GameObject.Find("Centipede");
        mCentipedeBody = playerObj.GetComponent<MCentipedeBody>();
        cameraController = Camera.main.gameObject.GetComponent<CameraController>();
        generateGrid = gameObject.GetComponent<GenerateGrid>();
        uiButtons = gameObject.GetComponent<UIButtons>();

    }

    private void Start()
    {
        if (uiButtons != null)
            uiButtons.StartUI();
    }
    
    private void Update()
    {
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Centipede");
            mCentipedeBody = playerObj.GetComponent<MCentipedeBody>();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.BackQuote) && uiButtons.Dev_Story_Skip)
        {
            uiButtons.StoryFinished(uiButtons.Dev_Story_Skip);
            uiButtons.Continue();
        }
#endif
    }
}
