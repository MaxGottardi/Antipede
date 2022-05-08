using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement
{
    public static Quaternion SetRotation(GenericAnt blackboard, Vector3 lookDir)
    {
        //solved ant orientation using code from here https://forum.unity.com/threads/look-at-object-while-aligned-to-surface.515743/
        Vector3 up = SetGround(blackboard);//the the up position of the normal of the ground

        ////Vector3 lookDir = (blackboard.nextPosVector - blackboard.transform.position) + (blackboard.transform.right * forwardOffset);
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
    public static Vector3 SetGround(GenericAnt blackboard)
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

        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -0.5f, 5 * (blackboard.transform.forward - blackboard.transform.up * 0.1f), Color.green);
        Debug.DrawRay(blackboard.transform.position, 5 * (raycastHit.normal), Color.blue);
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, 5 * (raycastHit1.normal), Color.blue);

        return upSmooth;
    }

    //void MoveToGround(GenericAnt blackboard)
    //{
    //    Vector3 groundPoint = blackboard.transform.position;
    //    groundPoint.y = groundY;

    //    float fallStep = Time.deltaTime * 5;
    //    blackboard.transform.position = Vector3.MoveTowards(blackboard.transform.position, groundPoint, fallStep);
    //}
}
