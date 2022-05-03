using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnt : GenericAnt
{
    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new GuardAttack(this);
        stateMachine.Investigate = new GuardInvestigate(this);
    }
}
