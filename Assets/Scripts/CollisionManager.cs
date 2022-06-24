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
            GameManager1.uiButtons.SpeedUI();
            GameManager1.uiButtons.AddSpeed();
        }
        else if (other.CompareTag("Health"))
        {
            HealthComponent health = player.GetComponent<HealthComponent>();
            //health.IncreaseHealth(10);
            player.AddSegment();
            player.AddSegment();
            //GameManager1.uiButtons.AddSegment();
            Destroy(other.gameObject);
            sfxManager.CollectPowerup();
        }
        else if (other.CompareTag("Larvae"))
        {
            HealthComponent health = player.GetComponent<HealthComponent>();
            //health.IncreaseHealth(10);
            player.AddSegment();
            Destroy(other.gameObject);
            if (sfxManager)
                sfxManager.CollectLarvae();
        }
        else if (other.CompareTag("CaveTrigger"))
        {
            sfxManager.EnterCave();
        }
        else if (other.CompareTag("BossTrigger"))
        {
            sfxManager.EnterBoss();
            //
        }
        else if (other.CompareTag("Weapon Pickup"))
        {
            sfxManager.CollectSpecial();

            //Card card = other.gameObject.GetComponent<Card>();
            //int cardIndex = card.GetCardIndex();

            //cardManager.CollectCard(cardIndex);
           
            if (other.gameObject.GetComponent<WeaponPickup>().isShield)
            {
                player.IncreaseSpeed(50.0f);
                GameManager1.uiButtons.SpeedUI();
                GameManager1.uiButtons.AddSpeed();
            }
            if (other.gameObject.GetComponent<WeaponPickup>().isLauncher)
            {
                player.AddSegment();
                player.AddSegment();
                player.IncreaseSpeed(50.0f);
                GameManager1.uiButtons.SpeedUI();
                GameManager1.uiButtons.AddSpeed();
            }
            if (other.gameObject.GetComponent<WeaponPickup>().isLaser)
            {
                player.AddSegment();
                player.IncreaseSpeed(100.0f);
                GameManager1.uiButtons.SpeedUI();
                GameManager1.uiButtons.AddSpeed();
                GameManager1.uiButtons.AddSpeed();
            }
            if (other.gameObject.GetComponent<WeaponPickup>().isGun)
            {
                player.AddSegment();
                player.AddSegment();
                player.AddSegment();
                player.IncreaseSpeed(150.0f);
                GameManager1.uiButtons.SpeedUI();
                GameManager1.uiButtons.AddSpeed();
                GameManager1.uiButtons.AddSpeed();
                GameManager1.uiButtons.AddSpeed();
            }
            if (other.gameObject.GetComponent<WeaponPickup>().isFlame)
            {
                player.AddSegment();
                player.AddSegment();
                player.IncreaseSpeed(100.0f);
                GameManager1.uiButtons.SpeedUI();
                GameManager1.uiButtons.AddSpeed();
                GameManager1.uiButtons.AddSpeed();
            }

            Destroy(other.gameObject);


        }
    }
}