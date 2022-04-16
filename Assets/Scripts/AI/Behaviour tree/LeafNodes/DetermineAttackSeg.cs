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
            //currSegment.beingAttacked = true;
            return NodeState.Success; //assigned segment
        }

        return NodeState.Failure; //no segments found
    }
}
