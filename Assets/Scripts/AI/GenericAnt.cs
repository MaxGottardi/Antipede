using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>The base class for an ant.</summary>
public class GenericAnt : MonoBehaviour
{
    [HideInInspector] public Transform nextPosTransform;
    [HideInInspector] public List<Vector3> pathToNextPos;
    [HideInInspector] public Vector3 nextPosVector;

    [Header("General Settings")]
    public GameObject[] nodesList;
    public GameObject shockBar;

    [HideInInspector] public Animator anim;
    public LayerMask playerLayer, EnemyLayer, groundLayer;




    public float health = 100;
    float maxHealth;
    public GameObject leftAntenna, rightAntenna;
    [HideInInspector] public float callBackupWait = 0; //the time remaining which cannot call a backup
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public bool canInvestigate = false, callingBackup = false;

    [Header("Movement Settings")]
    public float Speed = 1.5f;
    public float rotSpeed = 5;
    public string FollowingNodes;
    //the offset of the y axis the ant recieves to ensure it will always appear on the ground
    public float groundOffset;
    //the offset for the raycast checking the ground at the back of the ant
    public float backGroundCheckOffset;

    [Header("Sight Checks")] //the view angle checks
    public float maxSightDist = 5.0f;
    public float shortSightDist = 2.0f;
    [Range(0, 360)]
    public float largeViewAnlge, shortViewAngle;

    [Header("Backup Calling")]
    public float maxBackupDist;
    public float backupCallDist = 7.5f;

    [Header("Attack Settings")]
    public float attachDist = 0.5f;

    public virtual void Start()
    {
        maxHealth = health;
        pathToNextPos = new List<Vector3>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        //anim.SetTrigger("Walk");
        nodesList = GameObject.FindGameObjectsWithTag(FollowingNodes);
        stateMachine = new StateMachine(this);
        stateMachine.changeState(stateMachine.Movement);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        stateMachine.Update();
    }
    /// <summary>
    /// Check to see if any segment of the player is within sight
    /// </summary>
    /// <returns>Did it detect the player?</returns>
    public bool DetectPlayer() //here use the proper vision cone
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxSightDist, playerLayer);

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
        return (distAway < shortSightDist && Vector3.Angle(transform.forward, dirToPoint) < shortViewAngle / 2 || Vector3.Angle(transform.forward, dirToPoint) < largeViewAnlge / 2);
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
        if (stateMachine.currState != stateMachine.Dead)
        {
            health -= amount;

            //needs to be a value between 30 and 150
            ///curr hea
            float healthRatio = health / maxHealth;
            //new_value = ( (old_value - old_min) / (old_max - old_min) ) * (new_max - new_min) + new_min
            float currRote = health > 0 ? healthRatio * (150 - 30) + 30 : 150;
            float currChildRote = health > 0 ? healthRatio * (70 - 0) + 0 : 70;

            leftAntenna.transform.localRotation = Quaternion.Euler(currRote, leftAntenna.transform.localRotation.y, leftAntenna.transform.localRotation.z);
            rightAntenna.transform.localRotation = Quaternion.Euler(currRote, rightAntenna.transform.localRotation.y, rightAntenna.transform.localRotation.z);
            
            leftAntenna.transform.GetChild(0).localRotation = Quaternion.Euler(currChildRote, leftAntenna.transform.GetChild(0).localRotation.y, leftAntenna.transform.GetChild(0).localRotation.z);
            rightAntenna.transform.GetChild(0).localRotation = Quaternion.Euler(currChildRote, rightAntenna.transform.GetChild(0).localRotation.y, rightAntenna.transform.GetChild(0).localRotation.z);

            if (health <= 0)
                stateMachine.changeState(stateMachine.Dead);
            else
                stateMachine.changeState(stateMachine.Damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, maxSightDist);
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, shortSightDist);

        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -largeViewAnlge / 2);
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, +largeViewAnlge / 2);
        
        Vector3 shortViewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -shortViewAngle / 2);
        Vector3 shortViewAngle02 = DirectionFromAngle(transform.eulerAngles.y, +shortViewAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(transform.position, transform.position + viewAngle01 * maxSightDist);
        Handles.DrawLine(transform.position, transform.position + viewAngle02 * maxSightDist);

        Handles.color = Color.red;
        Handles.DrawLine(transform.position, transform.position + shortViewAngle01 * shortSightDist);
        Handles.DrawLine(transform.position, transform.position + shortViewAngle02 * shortSightDist);
    }

    private Vector3 DirectionFromAngle(float eulerY, float anglesInDegrees)
    {
        anglesInDegrees += eulerY;

        return new Vector3(Mathf.Sin(anglesInDegrees * Mathf.Deg2Rad), 0,
            Mathf.Cos(anglesInDegrees * Mathf.Deg2Rad));
    }

}