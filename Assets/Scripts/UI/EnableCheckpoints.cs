using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableCheckpoints : MonoBehaviour
{

    private Toggle enableCheckpoint;
    private GameObject[] checkPoints;

    // Start is called before the first frame update
    void Start()
    {
        enableCheckpoint = gameObject.GetComponent<Toggle>();

        checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
    }

    // Update is called once per frame
    void Update()
    {
        if (enableCheckpoint.isOn)
        {
            foreach (GameObject checkpoint in checkPoints)
            {
                checkpoint.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject checkpoint in checkPoints)
            {
                checkpoint.SetActive(false);
            }
        }
    }
}
