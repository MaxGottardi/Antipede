using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tarantula: MonoBehaviour
{

    private GameObject nest;
    private GameObject rotationPoint;
    public int nestArea = 40;
    public int huntingRadius = 15;
    public float moveSpeed = 4f;
    //public float damage = 2;
    private float health;
    public float maxHealth = 10;

    private Slider healthSlider;
    private bool dying;
    private float deathTimer;

    private float distToNest;
    private float distToPlayer;

    private GameObject targetSeg;
    float newTargetSegDist;
    float oldTargetSegDist;

    private Animation animator;
    private bool tarantulaMoving;

    public bool attackingPlayer;
    private float attackDelay = 0.9f;
    private float attackTimer;
    private MCentipedeBody player;

    private float newTargetTimer;
    //private bool attack;
    // Start is called before the first frame update
    void Awake()
    {
        rotationPoint = gameObject.transform.parent.gameObject;
        nest = rotationPoint.transform.parent.gameObject;
    }
    void Start()
    {
        health = maxHealth;
        healthSlider = gameObject.transform.Find("Canvas").gameObject.transform.Find("Slider").gameObject.GetComponent<Slider>();
        healthSlider.value = CalculateHealth();

        animator = GetComponent<Animation>();

        dying = false;
        attackingPlayer = false;
        player = GameObject.Find("Centipede").GetComponent<MCentipedeBody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(health);
            //DecreaseHealth();
        }

        if (!dying)
        {
            foreach (MSegment segment in player.Segments)
            {
                if (oldTargetSegDist > newTargetSegDist)
                {
                    targetSeg = segment.gameObject;
                }

                oldTargetSegDist = newTargetSegDist;
                newTargetSegDist = Vector3.Distance(rotationPoint.transform.position, segment.gameObject.transform.position);
            }
            
            
            if (targetSeg != null)
            {
                distToNest = Vector3.Distance(nest.transform.position, rotationPoint.transform.position);
                distToPlayer = Vector3.Distance(rotationPoint.transform.position, targetSeg.transform.position);


                if (distToPlayer < huntingRadius && !attackingPlayer)
                {
                    if (distToNest < nestArea)
                    {
                        ChasePlayer();
                    }
                    else if (nestArea > Vector3.Distance(targetSeg.transform.position, nest.transform.position))
                    {
                        ChasePlayer();
                    }
                    else
                    {
                        tarantulaMoving = false;
                    }
                }

                if (distToPlayer >= huntingRadius)
                {
                    if (distToNest >= 1)
                    {
                        ReturnToNest();
                    }
                    else
                    {
                        tarantulaMoving = false;
                    }
                }
            }
            else
            {
                tarantulaMoving = false;
            }

            if (tarantulaMoving && !attackingPlayer)
            {
                animator.Play("Walk");
            }

            if (!tarantulaMoving && !attackingPlayer)
            {
                animator.Play("Idle");
            }

            if (!animator.IsPlaying("Attack"))
            {
                attackingPlayer = false;
            }

            if (attackingPlayer)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDelay)
                {

                    player.RemoveSegment(100);
                    attackTimer = 0;
                }
            }
        }

        if (health <= 0)
        {
            dying = true;
            deathTimer += Time.deltaTime;
            animator.Play("Death");
            if (deathTimer >= 2)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ChasePlayer()
    {
        tarantulaMoving = true;
        Vector3 dir = targetSeg.transform.position - rotationPoint.transform.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        rotationPoint.transform.rotation = Quaternion.RotateTowards(rotationPoint.transform.rotation, Quaternion.AngleAxis(angle, Vector3.down), 90*Time.deltaTime);

        Vector3 newPos = rotationPoint.transform.right * moveSpeed * Time.deltaTime;
        rotationPoint.transform.position += newPos;   
    }
    public void ReturnToNest()
    {
        tarantulaMoving = true;
        Vector3 dir = nest.transform.position - rotationPoint.transform.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        rotationPoint.transform.rotation = Quaternion.RotateTowards(rotationPoint.transform.rotation, Quaternion.AngleAxis(angle, Vector3.down), 90 * Time.deltaTime);

        Vector3 newPos = rotationPoint.transform.right * moveSpeed * Time.deltaTime;
        rotationPoint.transform.position += newPos;    
    }

    public void DecreaseHealth()
    {
        health--;
        //updates the health slider
        healthSlider.value = CalculateHealth();
    }
    float CalculateHealth()
    {
        return health / maxHealth;
    }

    public void AttackPlayer()
    {
        animator.Play("Attack");
        attackingPlayer = true;
        attackTimer = 0;
    }
}
