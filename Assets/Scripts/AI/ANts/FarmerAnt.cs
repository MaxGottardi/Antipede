using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAnt : GenericAnt
{
    [Header("Larvae")]
    [SerializeField] GameObject Larvae;
    Collider larvaeCollider;

    public bool[] useLarvae;
    public static ShuffleBag<bool> larvaeBag;

    // Start is called before the first frame update
    public override void Start()
    {
        if(larvaeBag == null)
        {
            larvaeBag = new ShuffleBag<bool>();
            larvaeBag.shuffleList = useLarvae;
        }
        SpawnLarvae();
        
        base.Start();
    }

    void SpawnLarvae()
    {
        if (larvaeBag.getNext())
        {
            GameObject spawnedLarvae = Instantiate(Larvae, headTransform);
            spawnedLarvae.transform.localRotation = Quaternion.Euler(0.4f, -90, -170);
            spawnedLarvae.transform.localPosition = new Vector3(0, 1.3f, -0);
            Larvae = spawnedLarvae;
            larvaeCollider = Larvae.GetComponent<Collider>();
            larvaeCollider.enabled = false;
        }
        else
            Larvae = null;
    }

    public override void ReduceHealth(int amount)
    {
        if (Larvae)
        {
            larvaeCollider.enabled = true;
            Larvae.transform.parent = null;
            Larvae = null;
        }
        base.ReduceHealth(amount);
    }
}
