using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tarantula: MonoBehaviour, IDataInterface
{
    public int idValue; //the current id value to specify this element as unique
    public static int numTarantulasLeft = 3;
    private GameObject nest;
    private GameObject rotationPoint;
    private int nestArea = 50;
    private int huntingRadius = 25;
    private float moveSpeed = 5f;
    private float health;
    public float maxHealth = 100;
    public float waitSpawnAntTime = 30;

    public Slider healthSlider;
    private bool dying;
    private float deathTimer;

    private float distToNest;//not saved
    private float distToPlayer;//not saved

    private GameObject targetSeg;
    float newTargetSegDist;//not saved
    float oldTargetSegDist;//not saved

    private Animation animator;
    private string currClipName;
    private bool tarantulaMoving;

    public bool attackingPlayer;
    private float attackDelay = 0.9f;
    private float attackTimer;
    private MCentipedeBody player;

    [SerializeField] private Rigidbody webPrefab;
    private float shootTimer;
    private float shootAnimTimer;
    private float shootDelay = 0.7f;
    private bool shooting;

    public GameObject antPrefab;
    private float spawnAntTimer;

    [SerializeField] SFXManager sfxManager;
    [SerializeField] GameObject shieldEffect;

    [SerializeField] GameObject gateToDestroy;
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

        //webPrefab = gameObject.transform.Find("Web").gameObject.GetComponent<Rigidbody>();
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

            bool nearPlayer = false;
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
                        if (SceneManager.GetActiveScene().name != "BossOnly3")
                        {
                            //Dont display UI in Bossonly scene
                            GameManager1.uiButtons.SpiderInfo();
                        }
                        ChasePlayer();
                        ShootWeb();
                        SpawnAnts();
                        nearPlayer = true;
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
                currClipName = "Walk";
                animator.Play("Walk");
            }

            if (!tarantulaMoving && !attackingPlayer && !shooting)
            {
                currClipName = "Idle";
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
            if (spawnAntTimer >= waitSpawnAntTime && nearPlayer)
            {
                SpawnAnts();
                spawnAntTimer = 0;
            }
        }

        if (health <= 0)
        {
            dying = true;
            deathTimer += Time.deltaTime;
            currClipName = "Death";
            animator.Play("Death");
            if (deathTimer >= 2)
            {
                numTarantulasLeft--;
                if (numTarantulasLeft <= 0)
                    Win();
                Destroy(gameObject);
            }
            if (gateToDestroy != null)
            {
                //Destroy(gateToDestroy);
                gateToDestroy.GetComponent<WallMovement>().SetMovable();
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
        health -= amount;
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
            currClipName = "Attack";
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
                currClipName = "Attack_Left";
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
                //if (genericAnt.stateMachine != null)
                  //  genericAnt.stateMachine.changeState(genericAnt.stateMachine.SpawnIn);

            }
            spawnAntTimer = 0;
        }
    }

    public void Win()
    {
        //Show Canvas with you won
        GameManager1.uiButtons.Win();
    }

    public void LoadData(SaveableData saveableData)
    {
        if(!saveableData.spiderData.dictionary.ContainsKey(idValue))
        {//as the key was not saved, it means it must have been destroyed before the saving occured so delete it
            Destroy(nest);
            return;
        }
        SpiderData spiderData = saveableData.spiderData.dictionary[idValue];
        //also loading in of the players save

        health = spiderData.spiderHealth;
        healthSlider.value = CalculateHealth();

        deathTimer = spiderData.spiderDeathTimer;
        attackTimer = spiderData.spiderAttackTimer;
        shootTimer = spiderData.spiderShootTimer;
        shootAnimTimer = spiderData.spiderShootAnimTimer;
        spawnAntTimer = spiderData.spiderSpawnAntTimer;

        dying = spiderData.spiderDying;
        tarantulaMoving = spiderData.spiderMoving;
        attackingPlayer = spiderData.spiderAttackPlayer;
        shooting = spiderData.spiderShooting;

        rotationPoint.transform.position = spiderData.spiderPosition;
        rotationPoint.transform.rotation = spiderData.spiderRotation;

        currClipName = spiderData.spiderCurAnimClip;
        animator.Play(currClipName);
        animator[currClipName].normalizedTime = spiderData.spiderCurAnimTime;
    }

    public void SaveData(ref SaveableData saveableData)
    {
        SpiderData spiderData = new SpiderData();
        spiderData.spiderHealth = health;
        spiderData.spiderDeathTimer = deathTimer;
        spiderData.spiderAttackTimer = attackTimer;
        spiderData.spiderShootTimer = shootTimer;
        spiderData.spiderShootAnimTimer = shootAnimTimer;
        spiderData.spiderSpawnAntTimer = spawnAntTimer;

        spiderData.spiderDying = dying;
        spiderData.spiderMoving = tarantulaMoving;
        spiderData.spiderAttackPlayer = attackingPlayer;
        spiderData.spiderShooting = shooting;

        spiderData.spiderPosition = rotationPoint.transform.position;
        spiderData.spiderRotation = rotationPoint.transform.rotation;

        spiderData.spiderCurAnimClip = currClipName;
        spiderData.spiderCurAnimTime = animator[currClipName].normalizedTime;

        saveableData.spiderData.dictionary.Add(idValue, spiderData);
    }
}
