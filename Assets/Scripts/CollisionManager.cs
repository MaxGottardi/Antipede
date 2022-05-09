using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] MCentipedeBody player;
    [SerializeField] SFXManager sfxManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Speed"))
        {
            player.IncreaseSpeed(50);
            Destroy(other.gameObject);
            sfxManager.CollectPowerup();
        }
        else if (other.CompareTag("Health"))
        {
            HealthComponent health = player.GetComponent<HealthComponent>();
            //health.IncreaseHealth(10);
            player.AddSegment();
            Destroy(other.gameObject);
            sfxManager.CollectPowerup();
        }
        else if (other.CompareTag("Larvae"))
        {
            HealthComponent health = player.GetComponent<HealthComponent>();
            //health.IncreaseHealth(10);
            player.AddSegment();
            Destroy(other.gameObject);
            sfxManager.CollectLarvae();
        }
        else if (other.CompareTag("CaveTrigger"))
        {
            sfxManager.EnterCave();
        }
        else if (other.CompareTag("BossTrigger"))
        {
            sfxManager.EnterBoss();
        }
        else if (other.CompareTag("Card"))
        {
            sfxManager.CollectSpecial();
            //DO SOMETHING WITH CARDS
            Destroy(other.gameObject);
        }
    }
}