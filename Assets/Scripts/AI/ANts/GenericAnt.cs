using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

   public enum EAntType
    { 
    farmer,
        hunter,
        guard,
        bomb,
        dasher
    }
/// <summary>The base class for an ant.</summary>
public class GenericAnt : MonoBehaviour, IDataInterface
{
 
    public EAntType antType;
    //none saved, probs not required as should be fine updating on first frame spawns in
    [HideInInspector] public Transform nextPosTransform;
    [HideInInspector] public List<Vector3> pathToNextPos;
    [HideInInspector] public Vector3 nextPosVector;

    [Header("General Settings")]
    public GameObject[] nodesList;
    public GameObject shockBar;

    [HideInInspector] public Animator anim;
    public LayerMask playerLayer, EnemyLayer, groundLayer;

    [SerializeField] public SFXManager sfxManager;


    [HideInInspector] public float callBackupWait = 0; //the time remaining which cannot call a backup
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public bool canInvestigate = false, callingBackup = false;

    [Header("Movement Settings")]
    public float Speed = 1.5f;
    public float animMultiplier;
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
    public float chanceSpawnHelpers; //the percentage chance determining number of ants to spawn as backup
    public float maxBackupDist; //the distance away from the player it can call backup from
    public float backupCallDist = 7.5f; //once calling backup, the distance from the current location other ants will be notified
    public float backupRingScale = 32;
    public GameObject[] spawnedHelp;
    [HideInInspector]public ShuffleBag<GameObject> spawnedHelpBag;
    public bool isHelper = false;
    public GameObject backupRing;

    [Header("Attack Settings")]
    public float attachDist = 0.5f;
    public float attackAnimLength;
    public Transform headTransform;

    [Header("Damage Settings")]
    [SerializeField] public bool[] damageStageChance; //out of 10 bites it recives how many of them will go into the damage stage
    public float health = 100;
    [HideInInspector]public float maxHealth;
    public GameObject leftAntenna, rightAntenna;
    public ShuffleBag<bool> healthBag; //the shuffle bag for the ant damage state
    [Range(0,1)]
    public float minFleeChance;

    [Header("Audio Settings")]
    public AudioSource audioSource;

    public virtual void Start()
    {
        //if (healthBag == null) //if this is not yet set, then it means it hasn't run start, otherwise do run the start method. this required to be called before setting the ants data on its spawning in
        {
            sfxManager = FindObjectOfType<SFXManager>();

            backupRing.SetActive(false);
            healthBag = new ShuffleBag<bool>();
            healthBag.shuffleList = damageStageChance;

            spawnedHelpBag = new ShuffleBag<GameObject>();
            spawnedHelpBag.shuffleList = spawnedHelp;

            maxHealth = health;
            pathToNextPos = new List<Vector3>();
            anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
            audioSource = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            anim.SetFloat("SpeedMultiplier", animMultiplier);
            //anim.SetTrigger("Walk");
            nodesList = GameObject.FindGameObjectsWithTag(FollowingNodes);
            stateMachine = new StateMachine(this);
            if (isHelper)
                stateMachine.changeState(stateMachine.SpawnIn);
            else
                stateMachine.changeState(stateMachine.Movement);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        ////Debug.Log(headTransform.localPosition);
        if (GameManager1.playerObj != null && Vector3.Distance(transform.position, GameManager1.playerObj.transform.position) <= 200)
            stateMachine.Update();
        else
        {
            anim.SetTrigger("Idle");
        }
        ////anim.Play("Base Layer.AntAttack", 0, 0.5f);


        //headTransform.localPosition = new Vector3(100, 100, 100);// = (blackboard.nextPosVector - blackboard.transform.position).normalized;
        //Debug.Log(headTransform.localPosition);
        //issue is when not moving, still likly playing the movement animation
    }
    /// <summary>
    /// Check to see if any segment of the player is within sight
    /// </summary>
    /// <returns>Did it detect the player?</returns>
    public bool DetectPlayer() //here use the proper vision cone
    {
        if (Time.frameCount % 5 == 0 || Time.deltaTime > 0.25f)
        {
            //Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxSightDist, playerLayer);

            foreach (MSegment playerSegment in GameManager1.mCentipedeBody.Segments) //get all nearby ants to call for backup
            {
                float distAway = Vector3.Distance(transform.position, playerSegment.gameObject.transform.position);
                Vector3 dirToPoint = (playerSegment.gameObject.transform.position - transform.position).normalized;
                if (distAway < maxSightDist)
                {
                    if (GameManager1.uiButtons != null)
                        GameManager1.uiButtons.AttackUI();
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


    /// <summary>
    /// called whenever this ant takes damage
    /// </summary>
    /// <param name="amount">the amount of health which gets lost</param>
    public virtual void ReduceHealth(int amount)
    {
        if (stateMachine.currState != stateMachine.Dead && health > 0)
        {
            health -= amount;

            AssignAntennaPositions();
            if (health <= 0)
            {
                Destroy(transform.GetChild(0).GetComponent<Collider>());
                loseAttackInterest();
                stateMachine.changeState(stateMachine.Dead);
            }
            else if (healthBag.getNext()) //if it randomly chooses yes, only then give damage
            {
                loseAttackInterest();
                stateMachine.changeState(stateMachine.Damage);
            }
            else if (stateMachine.currState == stateMachine.Movement)
            {
                stateMachine.changeState(stateMachine.Investigate);
            }
        }
    }

    public void AssignAntennaPositions()
    {
        //needs to be a value between 30 and 150
        float healthRatio = health / maxHealth;
        //new_value = ( (old_value - old_min) / (old_max - old_min) ) * (new_max - new_min) + new_min
        float currRote = health > 0 ? healthRatio * (30 - 150) + 150 : 150;
        float currChildRote = health > 0 ? healthRatio * (0 - 70) + 70 : 70;

        leftAntenna.transform.localRotation = Quaternion.Euler(currRote, 0, 21.5f);
        rightAntenna.transform.localRotation = Quaternion.Euler(currRote, 0, 21.5f);

        leftAntenna.transform.GetChild(0).localRotation = Quaternion.Euler(currChildRote, 0, 0);
        rightAntenna.transform.GetChild(0).localRotation = Quaternion.Euler(currChildRote, 0, 0);

    }
#if UNITY_EDITOR
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, backupCallDist);
    }
#endif
    private Vector3 DirectionFromAngle(float eulerY, float anglesInDegrees)
    {
        anglesInDegrees += eulerY;

        return new Vector3(Mathf.Sin(anglesInDegrees * Mathf.Deg2Rad), 0,
            Mathf.Cos(anglesInDegrees * Mathf.Deg2Rad));
    }

    public bool NearSegment(Vector3 currPos, bool justCheck = false)
    {        
        /////////this code would ensure that once a segment was choosen for an attack it would always only go for that segment, but results in diffuclty attacking when moving fast
       // if(stateMachine.currState == stateMachine.Investigate)
        //{
         //   return Vector3.Distance(currPos, nextPosTransform.position) < attachDist;

       // }

        if (Vector3.Distance(currPos, GameManager1.mCentipedeBody.Head.position) < attachDist || Vector3.Distance(currPos, GameManager1.mCentipedeBody.Tail.position) < attachDist)
            return true; //first, check if near the head or tail
               
        foreach (MSegment segment in GameManager1.mCentipedeBody.Segments) //find if close enough to any segment for the attack to work
        {
            float dist = Vector3.Distance(currPos, segment.gameObject.transform.position);
            if (dist < attachDist) //if close enough can apply the attack damage
            {
                if (!justCheck) //means also looking for player to attack
                    nextPosTransform = segment.gameObject.transform;
                return true;
            }
        }

        return false;
    }

    ////private void OnDrawGizmos()
    ////{
    ////    Gizmos.DrawSphere(transform.position, 5);
    ////}
    /// <summary>
    /// update the segment as the ant is no longer attacking it
    /// </summary>
    public void loseAttackInterest()
    {
        if (nextPosTransform)
        {
            MSegment segment = nextPosTransform.gameObject.GetComponent<MSegment>();
            if (segment != null && segment.numAttacking > 0)
                segment.numAttacking--;
        }
    }

    public void LoadData(SaveableData saveableData)
    {
        //not required, but must be kept anyway
    }

    public virtual void SaveData(SaveableData saveableData)
    {
        //use as a base for inheritance
    }

    public string SaveAntStateName()
    {        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Walk"))
            return "Base Layer.Walk"; 
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.AntBackupCall"))
            return "Base Layer.AntBackupCall"; 
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.AntAttack"))
            return "Base Layer.AntAttack";

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.AntDead"))
            return "Base Layer.AntDead"; 

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.AntDaze"))
            return "Base Layer.AntDaze"; 
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.CoreExpandAnim"))
            return "Base Layer.CoreExpandAnim";

        return "Base Layer.Idle"; //the default state
    }
}