using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAnt : GenericAnt
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new BombAttack(this);
    }

    // Update is called once per frame

}
