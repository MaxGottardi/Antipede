using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    public bool isShot = true;
    private float despawnTimer;
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
            if (transform.position.y <= 0)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                despawnTimer += Time.deltaTime;
                if (despawnTimer >= 10)
                {
                    //Destroy(gameObject);
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
}
