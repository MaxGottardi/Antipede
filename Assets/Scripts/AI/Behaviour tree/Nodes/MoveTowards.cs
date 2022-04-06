using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : Node
{
    public MoveTowards(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        blackboard.transform.position += blackboard.transform.forward * Time.deltaTime * blackboard.Speed;
        Vector3 lookPos = blackboard.newNode.transform.position;
        lookPos.y = blackboard.transform.position.y;

        blackboard.transform.LookAt(lookPos);

        if (Vector3.Distance(blackboard.transform.position, blackboard.newNode.transform.position) < 1)
            return NodeState.Success;
        else
            return NodeState.Running;
    }
}