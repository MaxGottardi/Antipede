using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineAttackSeg : Node
{
    public DetermineAttackSeg(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override NodeState evaluate()
    {
        MSegment currSegment = null;
        float distToSegmnet = -1;
        foreach (MSegment segment in GameManager1.playerObj.GetComponent<MCentipedeBody>().Segments) //find the closest player segment
        {
            float dist = Vector3.Distance(blackboard.transform.position, segment.gameObject.transform.position);
            if (currSegment == null || dist < distToSegmnet) //also if segment does not have an ant currently attacking it
            {
                currSegment = segment;
                distToSegmnet = dist;
            }
        }
        if (currSegment != null)
        {
            blackboard.nextPosTransform = currSegment.gameObject.transform;
            blackboard.pathToNextPos = GameManager1.generateGrid.APathfinding(blackboard.transform.position, blackboard.nextPosTransform.position);//generate the new path

            blackboard.nextPosVector = blackboard.pathToNextPos[blackboard.pathToNextPos.Count - 1];
            blackboard.pathToNextPos.RemoveAt(blackboard.pathToNextPos.Count - 1);
            if (blackboard.pathToNextPos.Count <= 0) //as no new tiles to move towards can safely say move towards the final goal
                blackboard.nextPosVector = blackboard.nextPosTransform.position;
            //currSegment.beingAttacked = true;
            return NodeState.Success; //assigned segment
        }

        return NodeState.Failure; //no segments found
    }
}

public class PathToSegment : Node
{
    public PathToSegment(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override NodeState evaluate()
    {
        if (blackboard.nextPosTransform != null)
        {
            blackboard.pathToNextPos = GameManager1.generateGrid.APathfinding(blackboard.transform.position, blackboard.nextPosTransform.position);//generate the new path

            blackboard.nextPosVector = blackboard.pathToNextPos[blackboard.pathToNextPos.Count - 1]; //get the next node to move towards
            blackboard.pathToNextPos.RemoveAt(blackboard.pathToNextPos.Count - 1);
            if (blackboard.pathToNextPos.Count <= 0) //as no new tiles to move towards can safely say move towards the final goal
                blackboard.nextPosVector = blackboard.nextPosTransform.position;
            //currSegment.beingAttacked = true;
            return NodeState.Success; //assigned segment
        }

        return NodeState.Failure; //no segments found
    }
}