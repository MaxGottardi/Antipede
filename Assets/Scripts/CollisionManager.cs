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
        if (other.gameObject.tag == "Speed")
        {
            player.IncreaseSpeed(50);
            Destroy(other.gameObject);
            sfxManager.CollectPowerup();
        }
        if (other.gameObject.tag == "Health")
        {
            HealthComponent health = player.GetComponent<HealthComponent>();
            health.IncreaseHealth(10);
            Destroy(other.gameObject);
            sfxManager.CollectPowerup();
        }
        if (other.gameObject.tag == "Larvae")
        {
            HealthComponent health = player.GetComponent<HealthComponent>();
            health.IncreaseHealth(10);
            Destroy(other.gameObject);
            sfxManager.CollectLarvae();
        }
        if (other.gameObject.tag == "CaveTrigger")
        {
            sfxManager.EnterCave();
        }
        if (other.gameObject.tag == "BossTrigger")
        {
            sfxManager.EnterBoss();
        }
    }

}
