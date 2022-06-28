using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour, IDataInterface
{
    public bool isShot = false;
    public float despawnTimer;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Centipede");
    }

    // Update is called once per frame
    void Update()
    {
        if (isShot == true)
        {
            if (transform.position.y <= 0.01)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                gameObject.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
                despawnTimer += Time.deltaTime;
                if (despawnTimer >= 10)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerSegment")
        {
            player.GetComponent<MCentipedeBody>().tempSlowSpeed();
        }
    }

    public void LoadData(SaveableData saveableData)
    {
        //as this web exists in the scene when loading in from a save, destroy it
        Destroy(gameObject);
    }

    public void SaveData(SaveableData saveableData)
    {
        WebData webData = new WebData();
        webData.bWebIsShot = isShot;
        webData.webDespawnTimer = despawnTimer;
        webData.webPosition = transform.position;
        webData.webRotation = transform.rotation;
        webData.webVelocity = gameObject.GetComponent<Rigidbody>().velocity;

        saveableData.cobwebData.list.Add(webData);
    }
}
