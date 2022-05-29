using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject backupPlayer;
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
            Debug.Log(backupPlayer);
        }
        
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerSegment") && (backupPlayer == null || backupPlayer.name != "backupPlayer"))
        {
            foreach (GameObject CheckpointWithBackup in checkPoints)
            {
                Destroy(CheckpointWithBackup.GetComponent<Checkpoint>().backupPlayer);
            }

            backupPlayer = Instantiate(player, new Vector3(transform.position.x, transform.position.y-5, transform.position.z), player.transform.rotation);
            backupPlayer.SetActive(false);
        }
    }
}
