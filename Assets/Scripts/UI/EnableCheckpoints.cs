using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableCheckpoints : MonoBehaviour
{

    private Toggle enableCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        enableCheckpoint = gameObject.GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableCheckpoint.isOn)
        { 
            foreach (GameObject checkpoint in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (checkpoint.name == "Checkpoint")
                {
                    checkpoint.SetActive(true);
                }
            }
        }
        else
        {
            foreach (GameObject checkpoint in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (checkpoint.name == "Checkpoint")
                {
                    checkpoint.SetActive(false);
                }
            }
        }
    }
}
