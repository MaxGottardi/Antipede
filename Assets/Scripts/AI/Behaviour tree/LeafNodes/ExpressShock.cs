using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressShock : Node
{
    float shockTime = 0.45f;
    public ExpressShock(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override void init()
    {
        base.init();

        shockTime = 0.45f;
        blackboard.shockBar.SetActive(true);
    }
    public override NodeState evaluate()
    {
        shockTime -= Time.deltaTime;
        if (shockTime <= 0)
        {
            blackboard.shockBar.SetActive(false);
            return NodeState.Success;
        }
        else
            return NodeState.Running;
    }
}
