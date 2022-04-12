using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : Node
{
    float previousDistToNode = 0;
    public MoveTowards(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }

    public override void init()
    {
        base.init();

        previousDistToNode = NodeDistance();
    }
    public override NodeState evaluate()
    {
        doInit();

        blackboard.transform.position += blackboard.transform.forward * Time.deltaTime * blackboard.Speed;
        Vector3 lookPos = blackboard.nextPosTransform.transform.position;
        lookPos.y = blackboard.transform.position.y;

        Quaternion targetRotation = Quaternion.LookRotation(lookPos);
        blackboard.transform.rotation = targetRotation;//Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed); //need fast turning towards desired position instead here

        float currDist = NodeDistance();
        if (currDist < 1) //here if say a large lag spike and goes over the node but as never within range will always fail otherwise
            return NodeState.Success;
        else
        {
            previousDistToNode = currDist;
            return NodeState.Running;
        }
    }

    float NodeDistance()
    {
        return Vector3.Distance(blackboard.transform.position, blackboard.nextPosTransform.transform.position);
    }
}