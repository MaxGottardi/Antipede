using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAway : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Colliding with an enemy");
            Vector3 movementDir = transform.position - other.gameObject.transform.parent.position;
            other.gameObject.transform.parent.position += movementDir * Time.deltaTime * 300 * 5;
        }
    }
}
