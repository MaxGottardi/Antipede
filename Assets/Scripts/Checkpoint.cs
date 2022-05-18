using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameObject backupPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Centipede" && backupPlayer != null)
        {
            backupPlayer = Instantiate(collision.gameObject, new Vector3(transform.position.x, transform.position.y-5, transform.position.z), collision.gameObject.transform.rotation);
        }
    }
}
