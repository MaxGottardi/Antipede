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
        if (blackboard.pathToNextPos.Count <= 0)
        {
            if (blackboard.nextPosTransform.gameObject.GetComponent<NodeReferences>()) //here would, when using proper pathfinding, would be getting the nodes position as well as the path to it
                blackboard.nextPosTransform = blackboard.nextPosTransform.gameObject.GetComponent<NodeReferences>().nextNode.transform;

            blackboard.pathToNextPos = GameManager1.generateGrid.APathfinding(blackboard.transform.position, blackboard.nextPosTransform.position);//generate the new path
        }
        //blackboard.nextPosVector = blackboard.pathToNextPos[blackboard.pathToNextPos.Count - 1]
        //blackboard.pathToNextPos.RemoveAt(blackboard.pathToNextPos.Count - 1)
        if (blackboard.pathToNextPos.Count > 0) //here would, when using proper pathfinding, would be getting the nodes position as well as the path to it
        {
            //Debug.Log("Getting Next Node");
            blackboard.nextPosVector = blackboard.pathToNextPos[blackboard.pathToNextPos.Count - 1];
            blackboard.pathToNextPos.RemoveAt(blackboard.pathToNextPos.Count - 1);
            if (blackboard.pathToNextPos.Count <= 0) //as no new tiles to move towards can safely say move towards the final goal
                blackboard.nextPosVector = blackboard.nextPosTransform.position;
            return NodeState.Success;
        }
//        Debug.Log("Failed Getting a new path");
        return NodeState.Failure; //no node or path could be found
    }
}