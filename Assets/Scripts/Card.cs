using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    
    public int cardIndex;
    public string rarity;


    [SerializeField]Material cardSprite1;
    [SerializeField] Material cardSprite2;
    [SerializeField] Material cardSprite3;
    [SerializeField] Material cardSprite4;

    [SerializeField] GameObject card1Prefab;
    [SerializeField] GameObject card2Prefab;
    [SerializeField] GameObject card3Prefab;
    [SerializeField] GameObject card4Prefab;

    MeshRenderer meshRenderer;
    bool gun = false;
    bool laser = false;
    bool launcher = false;
    bool shield = false;
    bool active = false;
    int speedIncrease = 0;



    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        cardIndex = calculateRarity();
        //SetSprite(cardIndex);
        SetPrefab(cardIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int calculateRarity() //while this works, a shuffle list or something is better as with those you can ensure you will only ever have x amount of a certain type
    {
        int random = Random.Range(0, 100);

        if (random < 40)
        {
            //Shield
            shield = true;
            rarity = "common";
            return 1;
        }
        if (random >= 40 && random < 65)
        {
            //Launcher
            launcher = true;
            rarity = "uncommon";
            return 2;
        }
        if (random >= 65 && random < 90)
        {
            //Laser
            laser = true;
            rarity = "uncommon";
            return 3;
        }
        if (random >= 90)
        {
            //Gun
            gun = true;
            rarity = "rare";
            return 4;
        }

        return 0;
    }

    void SetSprite(int index)
    {
        if (index == 1)
        {
            meshRenderer.material = cardSprite1;
        }
        if (index == 2)
        {
            meshRenderer.material = cardSprite2;
        }
        if (index == 3)
        {
            meshRenderer.material = cardSprite3;
        }
        if (index == 4)
        {
            meshRenderer.material = cardSprite4;
        }
    }

    void SetPrefab(int cardIndex)
    {
        //instantiate CardPrefab
        //Destroy this
        if (cardIndex == 1)
        {
            Instantiate(card1Prefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (cardIndex == 2)
        {
            Instantiate(card2Prefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (cardIndex == 3)
        {
            Instantiate(card3Prefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (cardIndex == 4)
        {
            Instantiate(card4Prefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public int GetCardIndex()
    {
        return this.cardIndex;
    }

    public string GetRarity()
    {
        return this.rarity;
    }
}
