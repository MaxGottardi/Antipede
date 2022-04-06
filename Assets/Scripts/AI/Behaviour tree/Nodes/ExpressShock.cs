using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressShock : Node
{
    float shockTime = 0.25f;
    public ExpressShock(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        shockTime -= Time.deltaTime;
        if (shockTime <= 0)
        {
            shockTime = 0.25f;
            return NodeState.Success;
        }
        else
            return NodeState.Running;
    }
}
