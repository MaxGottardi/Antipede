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
    private float maxHealth = 10;

    private Slider healthSlider;

    private float distToNest;
    private float distToPlayer;

    private GameObject middleSeg;
    private int middleSegInt;

    private Animation animator;
    private bool tarantulaMoving;

    public bool attackingPlayer;
    private float attackDelay = 0.9f;
    private float attackTimer;
    private GameObject player;
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

        attackingPlayer = false;
        player = GameObject.Find("Centipede");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Debug.Log(tarantulaMoving);
            DecreaseHealth();
        }

        if (middleSeg != null)
        {
            distToNest = Vector3.Distance(nest.transform.position, rotationPoint.transform.position);
            distToPlayer = Vector3.Distance(rotationPoint.transform.position, middleSeg.transform.position);


            if (distToPlayer < huntingRadius && !attackingPlayer)
            {
                if (distToNest < nestArea)
                {
                    ChasePlayer();
                }
                else if (nestArea > Vector3.Distance(middleSeg.transform.position, nest.transform.position))
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
                
                player.GetComponent<MCentipedeBody>().RemoveSegment(100);
                //Debug.Log(player.GetComponent<MCentipedeBody>().Segments.Count)

                attackTimer = 0;
            }
        }
    }

    public void UpdateMiddleSeg()
    {
        
        middleSegInt = (player.GetComponent<MCentipedeBody>().Segments.Count / 2);
        middleSeg = player.GetComponent<MCentipedeBody>().Segments[middleSegInt].gameObject;
        
    }
    public void ChasePlayer()
    {
        tarantulaMoving = true;
        Vector3 dir = middleSeg.transform.position - rotationPoint.transform.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        rotationPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);

        //float moveDist = distToNest - 1f;
        //Debug.Log(moveDist);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveSpeed * Time.deltaTime;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * moveSpeed * Time.deltaTime;

        Vector3 newPos = new Vector3(x, 0f, z) + rotationPoint.transform.position;
        rotationPoint.transform.position = newPos;   
    }
    public void ReturnToNest()
    {
        tarantulaMoving = true;
        Vector3 dir = nest.transform.position - rotationPoint.transform.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        rotationPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);

        //float moveDist = distToNest - 1f;
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveSpeed * Time.deltaTime;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * moveSpeed * Time.deltaTime;

        Vector3 newPos = new Vector3(x, 0f, z) + rotationPoint.transform.position;
        rotationPoint.transform.position = newPos;    
    }

    public void DecreaseHealth()
    {
        health--;
        //updates the health slider
        healthSlider.value = CalculateHealth();
        if (health <= 0)
        {
            Destroy(nest);
        }
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
