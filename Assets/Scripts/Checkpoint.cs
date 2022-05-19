using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject backupPlayer;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Centipede");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log(backupPlayer);
        }
        
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(backupPlayer);
        if (collision.gameObject.CompareTag("PlayerSegment") && backupPlayer == null)
        {
            //Debug.Log("bruh");
            backupPlayer = Instantiate(player, new Vector3(transform.position.x, transform.position.y-5, transform.position.z), player.transform.rotation);
            backupPlayer.name = "BackupPlayer";
            backupPlayer.SetActive(false);
        }
    }
}
