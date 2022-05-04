using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOverlay : MonoBehaviour
{

    [SerializeField] Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
