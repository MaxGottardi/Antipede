using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : Node
{
    float previousDistToNode = 0;
    bool doDistCheck;
    public MoveTowards(GenericAnt blackboard, bool doDistCheck)
    {
        this.blackboard = blackboard;
        this.doDistCheck = doDistCheck;
    }

    public override void init()
    {
        base.init();

        if (blackboard.nextPosTransform != null)
            previousDistToNode = NodeDistance();
    }
    public override NodeState evaluate()
    {
        blackboard.transform.position += blackboard.transform.forward * Time.deltaTime * blackboard.Speed;
        blackboard.transform.position = new Vector3(blackboard.transform.position.x, 0.193f, blackboard.transform.position.z);

        if (blackboard.nextPosTransform == null)
            return NodeState.Failure;

        float currDist = 0; //current distance away from the segment
        if (doDistCheck)
            currDist = NodeDistance();

        Vector3 lookPos = blackboard.nextPosTransform.position;
        lookPos.y = 0.193f;//blackboard.transform.position.y;

        Quaternion targetRotation = Quaternion.LookRotation(lookPos - blackboard.transform.position); //get a vector pointing from current position to the new one and convert it to a rotation
        blackboard.transform.rotation = Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed);




        if (!doDistCheck || currDist < 1) //here if say a large lag spike and goes over the node but as never within range will always fail otherwise
            return NodeState.Success;
        else
            return NodeState.Running;

    }

    float NodeDistance()
    {
        return Vector3.Distance(blackboard.transform.position, blackboard.nextPosTransform.position);
    }
}