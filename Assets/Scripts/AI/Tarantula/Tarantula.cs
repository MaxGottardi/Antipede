using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tarantula: MonoBehaviour
{
    public static int numTarantulasLeft = 3;
    private GameObject nest;
    private GameObject rotationPoint;
    private int nestArea = 50;
    private int huntingRadius = 25;
    private float moveSpeed = 5f;
    private float health;
    private float maxHealth = 100;

    public Slider healthSlider;
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

    private Rigidbody webPrefab;
    private float shootTimer;
    private float shootAnimTimer;
    private float shootDelay = 0.7f;
    private bool shooting;

    public GameObject antPrefab;
    private float spawnAntTimer;

    [SerializeField] SFXManager sfxManager;
    [SerializeField] GameObject shieldEffect;
    // Start is called before the first frame update
    void Awake()
    {
        rotationPoint = gameObject.transform.parent.gameObject;
        nest = rotationPoint.transform.parent.gameObject;
        sfxManager = FindObjectOfType<SFXManager>();
    }
    void Start()
    {
        health = maxHealth;
        healthSlider = gameObject.transform.Find("Canvas").gameObject.transform.Find("Slider").gameObject.GetComponent<Slider>();
        healthSlider.value = CalculateHealth();

        animator = GetComponent<Animation>();

        dying = false;
        attackingPlayer = false;
        shooting = false;
        player = GameObject.Find("Centipede").GetComponent<MCentipedeBody>();

        webPrefab = gameObject.transform.Find("Web").gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Centipede").GetComponent<MCentipedeBody>();
        }
        if (healthSlider.value < 0.5f && !shieldEffect.activeSelf)
            shieldEffect.SetActive(true);

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

                //Checks if the player is withing hunting range of tarantula or if the player is in the nest area
                if (distToPlayer < huntingRadius ||
                    Vector3.Distance(targetSeg.transform.position, nest.transform.position) < nestArea)
                {   //if the tarantula is close enough to the nest it will follow the player
                    if (distToNest < nestArea)
                    {
                        GameManager1.uiButtons.SpiderInfo();
                        ChasePlayer();
                        ShootWeb();
                        SpawnAnts();
                    }   
                    else
                    {
                        tarantulaMoving = false;
                    }
                }
                //checks if the player is out of hunting range
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

            if (tarantulaMoving && !attackingPlayer && !shooting)
            {
                animator.Play("Walk");
            }

            if (!tarantulaMoving && !attackingPlayer && !shooting)
            {
                animator.Play("Idle");
            }

            if (!animator.IsPlaying("Attack"))
            {
                attackingPlayer = false;
            }

            if (!animator.IsPlaying("Attack_Left"))
            {
                shooting = false;
            }

            if (attackingPlayer)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDelay)
                {
                    player.RemoveSegment(100, targetSeg.transform.position);
                    attackTimer = 0;
                }
            }

            spawnAntTimer += Time.deltaTime;
            if (spawnAntTimer >= 30)
            {
                SpawnAnts();
                spawnAntTimer = 0;
            }
        }

        if (health <= 0)
        {
            dying = true;
            deathTimer += Time.deltaTime;
            animator.Play("Death");
            if (deathTimer >= 2)
            {
                numTarantulasLeft--;
                if (numTarantulasLeft <= 0)
                    Win();
                Destroy(gameObject);
            }
        }
    }

    public void ChasePlayer()
    {
        if (!attackingPlayer && !shooting)
        {
            tarantulaMoving = true;
            Vector3 dir = targetSeg.transform.position - rotationPoint.transform.position;
            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            rotationPoint.transform.rotation = Quaternion.RotateTowards(rotationPoint.transform.rotation, Quaternion.AngleAxis(angle, Vector3.down), 90 * Time.deltaTime);

            Vector3 newPos = rotationPoint.transform.right * moveSpeed * Time.deltaTime;
            rotationPoint.transform.position += newPos;
            
        }
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

    public void DecreaseHealth(int amount)
    {
        health-= amount;
        healthSlider.value = CalculateHealth();
    }
    float CalculateHealth()
    {
        return health / maxHealth;
    }

    public void AttackPlayer()
    {
        if (!shooting)
        {
            animator.Play("Attack");
            attackingPlayer = true;
            attackTimer = 0;
            sfxManager.SpiderAttack();
        }
    }

    private void ShootWeb()
    {
        if (!attackingPlayer)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= 5)
            {
                shooting = true;
                animator.Play("Attack_Left");
                shootAnimTimer += Time.deltaTime;
                if (shootAnimTimer >= shootDelay)
                {
                    SpawnWeb();
                    shootAnimTimer = 0;
                    shootTimer = 0;
                    sfxManager.SpiderWeb();
                }
            }
        }
    }
    private void SpawnWeb()
    {
        if (!attackingPlayer)
        {
            shooting = true;
            Rigidbody webShot;
            webShot = Instantiate(webPrefab, transform.position + transform.forward * 6.5f, Quaternion.identity);
            webShot.transform.position = new Vector3(webShot.transform.position.x, 1, webShot.transform.position.z);
            Vector3 target = player.transform.position - webShot.transform.position;
            webShot.velocity = new Vector3(target.x, target.y - 0.25f, target.z) * 3;
            webShot.GetComponent<Web>().isShot = true;
        }
    }

    private void SpawnAnts()
    {
        spawnAntTimer += Time.deltaTime;
        if (spawnAntTimer >= 30)
        {
            for (float i = -2; i < 2; i++)
            {
                GameObject ant;
                ant = Instantiate(antPrefab, transform.position + new Vector3(Mathf.Sin(i) * 10, 0, Mathf.Cos(i) * 10), Quaternion.identity);
                GenericAnt genericAnt = ant.GetComponent<GenericAnt>();
                genericAnt.maxSightDist = 100;
                genericAnt.largeViewAnlge = 360;
                genericAnt.isHelper = true;
                if (genericAnt.stateMachine != null)
                    genericAnt.stateMachine.changeState(genericAnt.stateMachine.SpawnIn);

            }
            spawnAntTimer = 0;
        }
    }

    public void Win()
    {
        //Show Canvas with you won
        GameManager1.uiButtons.Win();
    }
}
