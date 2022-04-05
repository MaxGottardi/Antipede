using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressShock : Node
{
    float shockTime = 1.0f;
    public ExpressShock(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        shockTime -= Time.deltaTime;
        if (shockTime <= 0)
        {
            shockTime = 1.0f;
            return NodeState.Success;
        }
        else
            return NodeState.Running;
    }
}
