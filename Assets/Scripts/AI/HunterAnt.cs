using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>The class for the hunter ant.</summary>
public class HunterAnt : GenericAnt
{
    //implement the shuffle list stuff, mkaing it static so all can use it
    public Vector3 weaponPos;
    public Transform weaponParent;

    public static ShuffleBag weaponsBag;
    public GameObject[] weapons;

    public Weapon weaponClass;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new HunterAttack(this);
        if(weaponsBag == null)
        {
            weaponsBag = new ShuffleBag();
            weaponsBag.shuffleList = weapons;
        }
        PickWeapon();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
       //// weaponClass.Fire(transform.forward);
    }

    void PickWeapon()
    {
        GameObject weapon = Instantiate(weaponsBag.getNext(), weaponParent, false);
        weapon.transform.localPosition = weaponPos;
        weapon.transform.localScale *= 2;
        weapon.transform.up = new Vector3(-90, 0, 0);

        weaponClass = weapon.GetComponent<Weapon>();
        weaponClass.LookAt(transform.forward);
    }
}
