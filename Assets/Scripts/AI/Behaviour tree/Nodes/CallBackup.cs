using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackup : Node
{
    float runTime = 1.0f;
    public CallBackup(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        //within a certain range of the AI cast out and see who responds
        if (runTime <= 0)
        {
            runTime = 1.0f;
            return NodeState.Success;
        }
        else
            return NodeState.Running;
    }
}
