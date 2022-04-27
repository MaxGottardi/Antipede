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
        if (blackboard.nextPosTransform == null)
            return NodeState.Failure;

        if (!blackboard.anim.GetCurrentAnimatorStateInfo(0).IsTag("isWalk")) //if not yet walking, initiate the walking animation
            blackboard.anim.SetTrigger("Walk");

        blackboard.transform.position += blackboard.transform.forward * Time.deltaTime * blackboard.Speed;
        blackboard.transform.rotation = SetRotation();

        float currDist = 0; //current distance away from the segment
        if (doDistCheck)
            currDist = NodeDistance();


        if (!doDistCheck || currDist < 1 || currDist > previousDistToNode) //successful if close to the node or begun moving beyond it already
        {
            //            Debug.Log("Successfully reached the next node");
            return NodeState.Success;
        }
        else
        {
            previousDistToNode = currDist;
            return NodeState.Running;
        }

    }

    float NodeDistance()
    {
        return Vector3.Distance(blackboard.transform.position, blackboard.nextPosVector);
    }

    /// <summary>
    /// Orient the ant so it will be facing towards its goal point while also being flat on the ground
    /// </summary>
    /// <returns>A smooth rotation towards this new orientation</returns>
    Quaternion SetRotation()
    {
        //solved ant orientation using code from here https://forum.unity.com/threads/look-at-object-while-aligned-to-surface.515743/
        Vector3 up = SetGround();//the the up position of the normal of the ground

        Vector3 lookDir = (blackboard.nextPosVector - blackboard.transform.position);
        lookDir.y = 0; //ignore all vertical height, so appears to be on flat ground
        lookDir.Normalize();

        //remove the up amount from the vector
        float d = Vector3.Dot(lookDir, up); //get the amount of the direction which was up, relative to the grounds normal
        lookDir -= d * up; //removes any upwards values, so the vectors now 90 degress to the normal and still heading in the right direction
        lookDir.Normalize();

        //convert the directional vector into a rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDir, up);

        //smoothly rotate towards this point
        Quaternion smoothRotation = Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed);

        return smoothRotation;
    }



    /// <summary>
    /// Set the ant so that it will always be on the ground
    /// </summary>
    /// <returns>The normal of the ground used for orienting the ant correctly</returns>
    Vector3 SetGround()
    {
        RaycastHit raycastHit, raycastHit1;
        bool didHit = Physics.Raycast(blackboard.transform.position, -Vector3.up, out raycastHit, 15, blackboard.groundLayer);
        bool didHit1 = Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -Vector3.up, out raycastHit1, 15, blackboard.groundLayer);

        if (didHit) //set the position of the ant to the ground
        {
            Vector3 groundPoint = blackboard.transform.localPosition;
            groundPoint.y = raycastHit.point.y + blackboard.groundOffset;
            blackboard.transform.localPosition = groundPoint;
        }


        Vector3 upSmooth;
        if (didHit1 && didHit)//when on the edge between two different triangles, get a vector which will point up ensuring a smooth rotation between the two
            upSmooth = Vector3.Cross(blackboard.transform.right, -(raycastHit.point - raycastHit1.point).normalized);
        //above used this link https://answers.unity.com/questions/1420677/best-way-to-rotate-player-object-to-match-the-grou.html
        else
            upSmooth = Vector3.up;

        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -0.5f, 5 * (blackboard.transform.forward), Color.green);
        Debug.DrawRay(blackboard.transform.position, 5 * (raycastHit.normal), Color.blue);
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, 5 * (raycastHit1.normal), Color.blue);

        return upSmooth;
    }
}