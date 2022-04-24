using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;






    // Start is called before the first frame update
    private void Awake()
    {
        maxHealth = 100;
        currentHealth = 50;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.LogWarning("Health: " + currentHealth);

        if (currentHealth == 0)
        {
            Death();
        }

        //Test code
        if (Input.GetKeyDown(KeyCode.X))
        {
            DecreaseHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            IncreaseHealth(10);
        }

    }

    public void IncreaseHealth(float amount)
    {
        if ((currentHealth + amount) > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }
        currentHealth += amount;
    }

    public void DecreaseHealth(float amount)
    {
        if ((currentHealth - amount) < 0)
        {
            currentHealth = 0;
            return;
        }
        currentHealth -= amount;

        //RemoveSegement();
    }

    public void Death()
    {
        Debug.Log("Game Over");

        //Freezes the scene
        Time.timeScale = 0;
    }
}
