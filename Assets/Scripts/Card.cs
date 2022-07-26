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
    [SerializeField] Material cardSprite5;

    [SerializeField] GameObject card1Prefab;
    [SerializeField] GameObject card2Prefab;
    [SerializeField] GameObject card3Prefab;
    [SerializeField] GameObject card4Prefab;
    [SerializeField] GameObject card5Prefab;

    MeshRenderer meshRenderer;
    bool gun = false;
    bool laser = false;
    bool launcher = false;
    bool shield = false;
    bool flame = false;
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

        if (random < 35)
        {
            //Shield
            shield = true;
            rarity = "common";
            return 1;
        }
        if (random >= 35 && random < 60)
        {
            //Launcher
            launcher = true;
            rarity = "uncommon";
            return 2;
        }
        if (random >= 60 && random < 85)
        {
            //Laser
            laser = true;
            rarity = "uncommon";
            return 3;
        }
        if (random >= 85 && random < 95)
        {
            //Gun
            gun = true;
            rarity = "rare";
            return 4;
        }
        if (random >= 95)
        {
            flame = true;
            rarity = "ultra-rare";
            return 5;
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
        if (index == 5)
        {
            meshRenderer.material = cardSprite5;
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
        if (cardIndex == 5)
        {
            Instantiate(card5Prefab, this.transform.position, Quaternion.identity);
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
