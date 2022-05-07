using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{

    [SerializeField] Canvas canvas;
    [SerializeField] Image cardCenter;
    [SerializeField] GameObject card;

    GameObject card1;
    GameObject card2;
    GameObject card3;
    GameObject card4;
    GameObject card5;
    GameObject card6;

    Vector3 card1Pos;
    Vector3 card2Pos;
    Vector3 card3Pos;
    Vector3 card4Pos;
    Vector3 card5Pos;
    Vector3 card6Pos;

    

    bool slot1_active = false;
    bool slot2_active = false;
    bool slot3_active = false;
    bool slot4_active = false;
    bool slot5_active = false;
    bool slot6_active = false;

    // Start is called before the first frame update
    void Start()
    {
        card1Pos = new Vector3(-331.17f, 103f, 0);
        card2Pos = new Vector3(-207.8f, 103, 0);
        card3Pos = new Vector3(-82.7f, 103, 0);
        card4Pos = new Vector3(41, 103, 0);
        card5Pos = new Vector3(167.2f, 103, 0);
        card6Pos = new Vector3(305.9f, 103, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            DisplayOverlay();
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideOverlay();
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F;
        }

    }

    public void DisplayOverlay()
    {
        canvas.gameObject.SetActive(true);
    }

    public void HideOverlay()
    {
        canvas.gameObject.SetActive(false);
    }

    public void CollectCard(int cardIndex)
    {
        Debug.Log(cardIndex);

        SetSlot(cardIndex);
        //Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
        //cardCenter.gameObject.SetActive(true);

        //Choose a card
        
        //Set card gameobject

        //Set image in canvas
    }

    public void SetSlot(int cardIndex)
    {
        if (!slot1_active)
        {
            //set active
            //change slot1 image

        }
    }
}
