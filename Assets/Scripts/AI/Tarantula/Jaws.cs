using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jaws : MonoBehaviour
{
    private GameObject tarantula;
    // Start is called before the first frame update
    void Start()
    {
        tarantula = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerSegment")
        {
            if (!tarantula.GetComponent<Tarantula>().attackingPlayer)
            tarantula.GetComponent<Tarantula>().AttackPlayer();
        }
    }
}
