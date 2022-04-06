using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnt : MonoBehaviour
{
    public GameObject newNode;
    public GameObject[] nodesList, shockBars;
    public float Speed = 1.5f, attachDist = 0.85f, sightDist = 5.0f;

    public LayerMask playerLayer;
    public bool isRienforcement = false;

    [HideInInspector] public StateMachine stateMachine;
    void Start()
    {
        nodesList = GameObject.FindGameObjectsWithTag("FarmerNode");
        stateMachine = new StateMachine(this);
        stateMachine.changeState(stateMachine.Movement);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public bool DetectPlayer()
    {

        RaycastHit hit;
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        Debug.DrawRay(rayPos, transform.forward * sightDist, Color.yellow);
        Debug.DrawRay(rayPos, (transform.forward + transform.right/2) * sightDist, Color.yellow);
        Debug.DrawRay(rayPos, (transform.forward - transform.right/2)* sightDist, Color.yellow);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rayPos, transform.forward, out hit, sightDist, playerLayer)
            || Physics.Raycast(rayPos, (transform.forward + transform.right / 2), out hit, sightDist, playerLayer)
            || Physics.Raycast(rayPos, (transform.forward - transform.right / 2), out hit, sightDist, playerLayer))
        {
            newNode = hit.collider.gameObject;
            return true;
        }
        //player pos is the global position of a player
        //if found set the found pos to the player position
        return false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 7.5f);
    }
}
