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
    [SerializeField] Material cardSprite6;
    [SerializeField] Material cardSprite7;
    [SerializeField] Material cardSprite8;

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
        SetSprite(cardIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int calculateRarity()
    {


        int random = Random.Range(0, 100);

        if (random < 20)
        {
            //Launcher
            launcher = true;
            rarity = "common";
            return 1;
        }
        if (random >= 20 && random <40)
        {
            //Shield
            shield = true;
            rarity = "common";
            return 2;
        }
        if (random >= 40 && random < 60)
        {
            //Laser
            laser = true;
            rarity = "common";
            return 3;
        }

        if (random >=60 && random < 77)
        {
            //Gun
            gun = true;
            rarity = "uncommon";
            return 4;
        }
        if (random >= 77 && random < 87)
        {
            //Launcher + 1 Speed
            launcher = true;
            speedIncrease = 1;
            rarity = "uncommon";
            return 5;
        }

        if (random >= 87 && random < 94)
        {
            //Laser + Shield
            laser = true;
            shield = true;
            rarity = "rare";
            return 6;
        }
        if (random >= 94 && random < 99)
        {
            //Laser + Launcher + 2 Speed
            laser = true;
            launcher = true;
            speedIncrease = 2;
            rarity = "rare";
            return 7;
        }

        if (random >= 99)
        {
            //Gun + Shield + 3 Speed
            gun = true;
            shield = true;
            speedIncrease = 3;
            rarity = "ultrarare";
            return 8;
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
        if (index == 6)
        {
            meshRenderer.material = cardSprite6;
        }
        if (index == 7)
        {
            meshRenderer.material = cardSprite7;
        }
        if (index == 8)
        {
            meshRenderer.material = cardSprite8;
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
