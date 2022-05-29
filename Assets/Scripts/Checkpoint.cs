using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool backupPlayerExists = false;
    private GameObject player;
    private GameObject[] checkPoints;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Centipede");
        checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            //Debug.Log(backupPlayer);
        }

        if (player == null)
        {
            player = GameObject.Find("Centipede");
        }
        
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerSegment") && backupPlayerExists == false)
        {
            GameObject backupPlayer = Instantiate(player, new Vector3(transform.position.x, transform.position.y-5, transform.position.z), player.transform.rotation);
            backupPlayer.tag = "backup";
            backupPlayer.name = "backupPlayer";
            backupPlayer.SetActive(false);
            player.GetComponent<MCentipedeBody>().newPlayer = backupPlayer;
            backupPlayerExists = true;
        }
    }
}
