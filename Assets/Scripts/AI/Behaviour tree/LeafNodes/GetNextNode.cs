using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNextNode : Node
{ //find next position on the map to move towards
    public GetNextNode(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        doInit();

        if (blackboard.nextPosTransform.gameObject.GetComponent<NodeReferences>()) //here would, when using proper pathfinding, would be getting the nodes position as well as the path to it
        {
            blackboard.nextPosTransform = blackboard.nextPosTransform.gameObject.GetComponent<NodeReferences>().nextNode.transform;
            Debug.Log("Getting New Node");
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
