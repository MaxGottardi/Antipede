using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformAttack : Node
{
    public PerformAttack(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        return NodeState.Success;
    }
}

public class AttackWait : Node
{
    float waitTime = 0.5f;
    public AttackWait(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        if (waitTime <= 0)
        {
            //apply the damages to the health here and junk
            waitTime = 0.5f;
            //foreach (GameObject warning in blackboard.shockBars)
            //{
            //    Vector3 pos = warning.transform.localPosition;
            //    pos.y = 1.566f;
            //    warning.transform.localPosition = pos;
            //}
            //////if (Vector3.Distance(blackboard.transform.position, blackboard.nextPosTransform.transform.position) < blackboard.attachDist)
              //////  GameObject.Find("Centipede").GetComponent<MCentipedeBody>().RemoveSegment();
            blackboard.stateMachine.changeState(blackboard.stateMachine.Movement);
            return NodeState.Success;
        }
        else
        {
            waitTime -= Time.deltaTime;
            //foreach (GameObject warning in blackboard.shockBars)
            //{
            //    Vector3 pos = warning.transform.localPosition;
            //    pos.y = 1.566f + (0.5f - waitTime) * 2;
            //    warning.transform.localPosition = pos;
            //}

            return NodeState.Running;
        }
    }
}
