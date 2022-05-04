using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] MCentipedeBody player;
    [SerializeField] SFXManager sfxManager;
    [SerializeField] CardManager cardManager;
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
            cardManager.CollectCard(1);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Weapon Pickup"))
        {
            Debug.Log("Colledted Weapon");
            WeaponPickup PickedUp = other.gameObject.GetComponent<WeaponPickup>();

            if (PickedUp != null)
            {
                WeaponCardUI.Add(PickedUp.Weapon);
            }
            else
            {
                Debug.LogError("Weapon Pickup has no WeaponPickup Component: " + other.name);
            }

            Destroy(other.gameObject);
        }
    }
}