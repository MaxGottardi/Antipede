using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnt : GenericAnt
{
    public static ShuffleBag<GameObject> parentBag;
    [SerializeField] GameObject[] parentParts;

    GameObject heldPart;

    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new GuardAttack(this);
        stateMachine.Investigate = new GuardInvestigate(this);
        stateMachine.Dead = new GuardDead(this);

        if (!isHelper) //only spawn in a part on the inital guards, non on any more which spawn in later
        {
            if (parentBag == null)
            {
                parentBag = new ShuffleBag<GameObject>();
                parentBag.shuffleList = parentParts;
            }
            SpawnPart();
        }
    }

    //spawn in parent components
    void SpawnPart()
    {
        heldPart = parentBag.getNext();
        if(heldPart != null)
        {
            heldPart = Instantiate(heldPart, headTransform);
            heldPart.transform.localRotation = Quaternion.Euler(0.4f, -90, -170);
            heldPart.transform.localScale /= 1.5f;
            heldPart.transform.localPosition = new Vector3(0, 1.3f, -0);
        }
    }

    public void DropParentSeg()
    {
        if (heldPart != null)
        {
            heldPart.transform.parent = null;
            heldPart.transform.localScale *= 2f;
            heldPart.GetComponent<ParentCollectible>().Collect();
        }
    }
}
