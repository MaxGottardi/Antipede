using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAnt : GenericAnt
{
    //implement the shuffle list stuff, mkaing it static so all can use it
    public Vector3 WeaponPos;
    public Transform WeaponParent;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        PickWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PickWeapon()
    {

    }
}
