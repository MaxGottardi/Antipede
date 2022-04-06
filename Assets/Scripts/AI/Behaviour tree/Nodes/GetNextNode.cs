using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNextNode : Node
{
    public GetNextNode(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        if (blackboard.newNode.GetComponent<NodeReferences>())
        {
            blackboard.newNode = blackboard.newNode.GetComponent<NodeReferences>().nextNode;
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
