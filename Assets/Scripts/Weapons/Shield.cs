using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{

    bool shieldActive;
    [SerializeField] SFXManager sfxManager;
    float shieldStartTime = 0;
    float shieldDuration;
    [SerializeField] GameObject centipede;
    MCentipedeBody mcb;




    // Start is called before the first frame update
    void Start()
    {


 
    }

    private void Awake()
    {
        sfxManager = FindObjectOfType<SFXManager>();
        mcb = FindObjectOfType<MCentipedeBody>();
        //mcb = centipede.GetComponent<MCentipedeBody>();
        shieldActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (shieldActive)
        {
            mcb.shieldActive = true;
        }
        else
        {
            mcb.shieldActive = false;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ActivateShield(15.0f);
        }

        if (shieldStartTime > 0)
        {
            if (Time.time <= shieldStartTime + shieldDuration)
            {
                shieldActive = true;
            }
            else
            {
                DeactivateShield();
            }
        }
    }
    public override Projectile Fire(Vector3 Position)
    {
        return null;
    }


    public void ActivateShield(float duration)
    {
        shieldDuration = duration;
        sfxManager.ActivateShield();
        shieldStartTime = Time.time;
    }

    public void DeactivateShield()
    {
        Debug.Log("dsfsdfs");
        shieldStartTime = 0;
        shieldActive = false;
        sfxManager.DeactivateShield();
    }

    public override void OnAttatch()
    {
        Debug.Log("Xdfdfdfdfdfdfdfdfdf");
        ActivateShield(5.0f);
    }
}
