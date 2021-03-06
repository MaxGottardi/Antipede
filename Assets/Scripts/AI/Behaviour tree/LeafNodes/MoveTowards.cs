using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : Node
{
    float previousDistToNode = 0;
    Quaternion forwardOffset;
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

        if (doDistCheck)
            forwardOffset = Quaternion.AngleAxis(Random.Range(-45, 45), blackboard.transform.up); //the offset to apply to the direction of movement
        //Random.Range(-.75f, .75f);
        else
            forwardOffset = Quaternion.AngleAxis(0, blackboard.transform.up); ;
    }
    public override NodeState evaluate()
    {
        if (blackboard.nextPosTransform == null)
            return NodeState.Failure;

        if (!blackboard.anim.GetCurrentAnimatorStateInfo(0).IsTag("isWalk")) //if not yet walking, initiate the walking animation
            blackboard.anim.SetTrigger("Walk");

        blackboard.transform.position += (blackboard.transform.forward) * Time.deltaTime * blackboard.Speed;

        Vector3 lookDir = forwardOffset * (blackboard.nextPosVector - blackboard.transform.position).normalized;// + (forwardOffset * blackboard.transform.right);
        Debug.DrawRay(blackboard.transform.position, lookDir, Color.cyan);
        blackboard.transform.rotation = GenericMovement.SetRotation(blackboard, lookDir);

        float currDist = 0; //current distance away from the segment
        if (doDistCheck)
            currDist = NodeDistance();

        //MoveToGround();

        if (!doDistCheck || currDist < 1 || currDist > previousDistToNode) //successful if close to the node or already passed it between the last frame and this one
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

        Vector3 lookDir = forwardOffset*(blackboard.nextPosVector - blackboard.transform.position);// + (forwardOffset*blackboard.transform.right);
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
        RaycastHit raycastHit, raycastHit1;//, raycastForward, raycastMiddle;
        bool didHit = Physics.Raycast(blackboard.transform.position, -blackboard.transform.up, out raycastHit, 15, blackboard.groundLayer);
        bool didHit1 = Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.up, out raycastHit1, 15, blackboard.groundLayer);


//////////below is an attampt to get the ants to walk over each other, as can see it did not work well

        //    bool didHitMiddle = Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset/4, -blackboard.transform.up, out raycastMiddle, 2, blackboard.EnemyLayer);
        //    bool didHitForward = Physics.Raycast(blackboard.transform.position, blackboard.transform.forward, out raycastForward, 0.5f, blackboard.EnemyLayer);
        //if (didHitForward)
        //    raycastHit = raycastForward;
        //else if (didHitMiddle)
        //    raycastHit = raycastMiddle;

        ////float number_of_rays = 10;
        ////float totalAngle = 360;

        ////float delta = totalAngle / number_of_rays;
        ////Vector3 pos = blackboard.transform.position;
        ////const float range = .5f;

        ////Collider[] hitColliders = Physics.OverlapSphere(blackboard.transform.position, range, blackboard.EnemyLayer);

        ////foreach (Collider enemySegment in hitColliders) //get all nearby ants to call for backup
        ////{
        ////    if (enemySegment.gameObject.transform.parent != blackboard.transform)
        ////    {
        ////        Vector3 dir = (enemySegment.gameObject.transform.position - blackboard.transform.position).normalized;
        ////        bool withinRange = Vector3.Angle(-blackboard.transform.up, dir) < 270 / 2;
        ////        if (withinRange)
        ////        {
        ////            RaycastHit htted;
        ////            Physics.Raycast(blackboard.transform.position, dir, out htted, range, blackboard.EnemyLayer);
        ////            raycastHit = htted;
        ////        }
        ////        break;
        ////    }
        ////}

        ////Collider[] hitColliders1 = Physics.OverlapSphere(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, range, blackboard.EnemyLayer);

        ////foreach (Collider enemySegment in hitColliders1) //get all nearby ants to call for backup
        ////{
        ////    if (enemySegment.gameObject.transform.parent != blackboard.transform)
        ////    {
        ////        Vector3 dir = (enemySegment.gameObject.transform.position - (blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset)).normalized;
        ////        bool withinRange = Vector3.Angle(-blackboard.transform.up, dir) < 270 / 4;
        ////        if (withinRange)
        ////        {
        ////            RaycastHit htted;
        ////            Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, dir, out htted, range, blackboard.EnemyLayer);
        ////            raycastHit1 = htted;
        ////        }
        ////        break;
        ////    }
        ////}

        ////Collider[] hitColliders3 = Physics.OverlapSphere(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset / 2, range, blackboard.EnemyLayer);

        ////foreach (Collider enemySegment in hitColliders3) //get all nearby ants to call for backup
        ////{
        ////    if (enemySegment.gameObject.transform.parent != blackboard.transform)
        ////    {
        ////        Vector3 dir = (enemySegment.gameObject.transform.position - (blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset / 2)).normalized;
        ////        bool withinRange = Vector3.Angle(-blackboard.transform.up, dir) < 270 / 2;
        ////        if (withinRange)
        ////        {
        ////            RaycastHit htted;
        ////            Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset / 2, dir, out htted, range, blackboard.EnemyLayer);
        ////            if (didHit && raycastHit.collider.gameObject.name == "Enemy")
        ////                raycastHit1 = htted;
        ////            else
        ////                raycastHit1 = htted;
        ////        }
        ////        break;
        ////    }
        ////}

        if (didHit) //set the position of the ant to the ground
        {
            //Debug.Log(raycastHit.collider.gameObject.name);

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

#if UNITY_EDITOR
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -0.5f , 5 * (blackboard.transform.forward- blackboard.transform.up * 0.1f), Color.green);
        Debug.DrawRay(blackboard.transform.position, 5 * (raycastHit.normal), Color.blue);
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, 5 * (raycastHit1.normal), Color.blue);
#endif
        return upSmooth;
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);

        saveableData.moveTowardsPrevDistToNode.list.Add(previousDistToNode);
        saveableData.moveTowardsForwardOffset.list.Add(forwardOffset);

    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);

        forwardOffset = saveableData.moveTowardsForwardOffset.list[0];
        saveableData.moveTowardsForwardOffset.list.RemoveAt(0);
        
        previousDistToNode = saveableData.moveTowardsPrevDistToNode.list[0];
        saveableData.moveTowardsPrevDistToNode.list.RemoveAt(0);
    }
}