using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnt : MonoBehaviour
{
    [Header("General Settings")]
    public Transform nextPosTransform;
    public List<Vector3> pathToNextPos;
    public Vector3 nextPosVector;
    public GameObject[] nodesList;
    public GameObject shockBar;

    public Animator anim;
    public LayerMask playerLayer, EnemyLayer, groundLayer;

    


    public float health = 100;
    [HideInInspector] public float callBackupWait = 0; //the time remaining which cannot call a backup
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public bool canInvestigate = false, callingBackup = false;

    [Header("Movement Settings")]
    public float Speed = 1.5f, rotSpeed = 5;

    [Header("Sight Checks")] //the view angle checks
    public float sightDist = 5.0f;
    [Range(0, 360)]
    public float largeViewAnlge, shortViewAngle;

    [Header("Backup Calling")]
    public float maxBackupDist, backupCallDist = 7.5f;

    [Header("Attack Settings")]
    public float attachDist = 0.5f;

    void Start()
    {
        pathToNextPos = new List<Vector3>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        anim.SetTrigger("Walk");
        nodesList = GameObject.FindGameObjectsWithTag("FarmerNode");
        stateMachine = new StateMachine(this);
        stateMachine.changeState(stateMachine.Movement);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
    /// <summary>
    /// Check to see if any segment of the player is within sight
    /// </summary>
    /// <returns>Did it detect the player?</returns>
    public bool DetectPlayer() //here use the proper vision cone
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDist, playerLayer);

        foreach (Collider playerSegment in hitColliders) //get all nearby ants to call for backup
        {
            float distAway = Vector3.Distance(transform.position, playerSegment.gameObject.transform.position);
            Vector3 dirToPoint = (playerSegment.gameObject.transform.position - transform.position).normalized;
            if (ValidAngle(distAway, dirToPoint))
            {
                //if (!Physics.Raycast(transform.position, dirToPoint, distAway, ~playerLayer)) //if it hit anything which was not the player than, it means the view is actually obstructed
                {
                    return true; //as nothing hit no walls etc. were in the way so safe to say it saw the player
                }
                //else
                  //  Debug.Log("Failed to ensure no obsticals in the way");
            }
        }
        return false; //no player segment found
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="distAway">How far away the chosen segment is</param>
    /// <param name="dirToPoint">the direction which points towards this segment</param>
    /// <returns>player within an angle of player or not</returns>
    bool ValidAngle(float distAway, Vector3 dirToPoint)
    {
        //if x distance away and within x angle then seen player, or if a much closer distance and a much larger angle
        return (distAway < 1 && Vector3.Angle(transform.forward, dirToPoint) < shortViewAngle/ 2 || Vector3.Angle(transform.forward, dirToPoint) < largeViewAnlge / 2);
    }

    ////void OnDrawGizmosSelected()
    ////{
    ////    // Draw a yellow sphere at the transform's position
    ////    Gizmos.color = Color.yellow;
    ////    Gizmos.DrawSphere(transform.position, 7.5f);
    ////}


    /// <summary>
    /// called whenever this ant takes damage
    /// </summary>
    /// <param name="amount">the amount of health which gets lost</param>
    public void ReduceHealth(int amount)
    {
        health -= amount;
        if (health <= 0)
            stateMachine.changeState(stateMachine.Dead);
        else
            stateMachine.changeState(stateMachine.Damage);

    }
}
