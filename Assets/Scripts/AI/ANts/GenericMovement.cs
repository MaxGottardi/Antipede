using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement
{
    public static Quaternion SetRotation(GenericAnt blackboard, Vector3 lookDir)
    {
        lookDir = RVOAvoidance(blackboard, lookDir); //appears to be working quite well, only issue now being in the way it actually senses which object to navigate around as buggs out when have multiple
        //solved ant orientation using code from here https://forum.unity.com/threads/look-at-object-while-aligned-to-surface.515743/
        Vector3 up = SetGround(blackboard);//the the up position of the normal of the ground

        ////Vector3 lookDir = (blackboard.nextPosVector - blackboard.transform.position) + (blackboard.transform.right * forwardOffset);
        lookDir.y = 0; //ignore all vertical height, so appears to be on flat ground
        lookDir.Normalize();



        //remove the up amount from the vector
        float d = Vector3.Dot(lookDir, up); //get the amount of the direction which was up, relative to the grounds normal
        lookDir -= d * up; //removes any upwards values, so the vectors now 90 degress to the normal and still heading in the right direction
        lookDir.Normalize();

        //lookDir = 

        //convert the directional vector into a rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDir, up);

        ////////////////////////////////////Vector3 dir = (blackboard.transform.position - lookDir);
        ////////////////////////////////////dir.y = 0;
        ////////////////////////////////////blackboard.headTransform.localEulerAngles = dir.normalized;
        //smoothly rotate towards this point
        Quaternion smoothRotation = Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed);
        return smoothRotation;
    }

    /// <summary>
    /// overall goal is to find the direction required to take to ensure it does not actually collide with another object
    /// </summary>
    /// <param name="blackboard">the object the transformations get applied to</param>
    /// <returns></returns>
    public static Vector3 RVOAvoidance(GenericAnt blackboard, Vector3 lookDirOrign)
    {
        Vector3 lookDir = lookDirOrign;//blackboard.transform.forward;
        float rayDist = 4;
        RaycastHit raycastHit; //ajust this collision avoidance to work using RVO https://gamma.cs.unc.edu/RVO/icra2008.pdf, as current method does not work well in crowds and occilations

        float sphereRadius = 1; //this radius around own circle where anything inside will result in a collision
                                //
        float avoidanceRadius = sphereRadius * 2; //this combine raduis of the current object and the one it hit, so that only directions outside of this will never collide

        Collider[] hitColliders = Physics.OverlapSphere(blackboard.transform.position, sphereRadius, blackboard.EnemyLayer);

        float distAway = -1;// = Vector3.Distance(transform.position, playerSegment.gameObject.transform.position);
        Collider closest = null;
        foreach (Collider enemy in hitColliders)
//        if(Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, lookDir, out raycastHit, rayDist, blackboard.EnemyLayer)||
//            Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, blackboard.transform.forward + blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
//        || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, blackboard.transform.forward - blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer) ||
//           Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, blackboard.transform.forward + blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
//            || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, blackboard.transform.forward - blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)



//            || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
//            || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, -blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer) ||
//            Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, -blackboard.transform.forward + blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
//            || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, -blackboard.transform.forward - blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
//            || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, -blackboard.transform.forward + blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
//            || Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, -blackboard.transform.forward - blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
//)
        {
            Vector3 dir = enemy.gameObject.transform.position - blackboard.transform.position;

            if (enemy.gameObject != blackboard.transform.GetChild(0).gameObject)// && Vector3.Angle(blackboard.transform.forward, dir.normalized) < 180/2)// && dotProduct < 0.25f)// && dotProduct < 0.5f)
            {
                // Vector3 pushDir = (blackboard.transform.position - enemy.gameObject.transform.position).normalized;
                // blackboard.transform.position += pushDir * Time.deltaTime * blackboard.Speed;

                //pushDir = (enemy.gameObject.transform.position - blackboard.transform.position).normalized;
                //enemy.gameObject.transform.position += pushDir * Time.deltaTime * blackboard.Speed;
               //// float dotProduct = Vector3.Dot(enemy.gameObject.transform.right, blackboard.transform.right);

               //// Vector3 moveDir = dir;
               //// Quaternion offsetAngle;
               //////if (dotProduct > 0)
               ////     offsetAngle = Quaternion.AngleAxis(Random.Range(30, 45), blackboard.transform.up);
               //////else
               ////  //   offsetAngle = Quaternion.AngleAxis(Random.Range(-45, -30), blackboard.transform.up);
               //// //
               //// moveDir.y = 0;
               //// Vector3 newAngle = offsetAngle * moveDir;
               //// blackboard.transform.position -= newAngle.normalized/hitColliders.Length * blackboard.Speed * Time.deltaTime;// / (hitColliders.Length) * Time.deltaTime;
               ////                                                                                                              //// enemy.gameObject.transform.position += dir.normalized * blackboard.Speed / 2 * Time.deltaTime;

                float newDist = Vector3.Distance(enemy.gameObject.transform.position, blackboard.transform.position);
                if (distAway < 0 || distAway > newDist)
                {
                    distAway = newDist;
                    closest = enemy;
                }

                //float distToShift = Mathf.Clamp(avoidanceRadius - Vector3.Distance(raycastHit.collider.gameObject.transform.position, blackboard.transform.position), -avoidanceRadius, avoidanceRadius);
                //////distToShift = 2;
                /////////it sees the obstical and generates a vector which falls outside of it
                //Vector3 newPoint = raycastHit.point;
                //Debug.Log(distToShift);
                //if (raycastHit.collider.transform.localPosition.x > blackboard.transform.localPosition.x)
                //    newPoint.x -= distToShift;
                ////lookDir = Quaternion.AngleAxis(distToShift, blackboard.transform.up) * lookDir;
                //else
                //    newPoint.x += distToShift;
                ////lookDir = Quaternion.AngleAxis(-distToShift, blackboard.transform.up) * lookDir;
                ////lookDir -= new Vector3(distToShift, 0, distToShift).normalized;

                ////the new direction to move in
                //Debug.Log(newPoint +"   " + raycastHit.point);
                //lookDir = newPoint - blackboard.transform.position;

                //Debug.DrawRay(blackboard.transform.position, rayDist * (lookDirOrign), Color.yellow);
                //Debug.DrawRay(blackboard.transform.position, rayDist * (lookDir), Color.black);

                //lookDir = (lookDirOrign + lookDir*2) / 2; //average of the two velocities

                ////              Vector3 dir = blackboard.transform.position - raycastHit.point;
            }

        }
        if (closest != null)
        {
            float dotProduct = Vector3.Dot(closest.gameObject.transform.right, blackboard.transform.right);
            Vector3 dir = closest.gameObject.transform.position - blackboard.transform.position;

            Vector3 moveDir = dir;
            Quaternion offsetAngle;
            //if (dotProduct > 0)
            offsetAngle = Quaternion.AngleAxis(Random.Range(30, 45), blackboard.transform.up);
            //else
            //   offsetAngle = Quaternion.AngleAxis(Random.Range(-45, -30), blackboard.transform.up);
            //
            moveDir.y = 0;
            Vector3 newAngle = offsetAngle * moveDir;
            //(newAngle + lookDirOrign) / 2;
            blackboard.transform.position -= newAngle.normalized * blackboard.Speed * Time.deltaTime;// / (hitColliders.Length) * Time.deltaTime;


            Quaternion targetRotation = Quaternion.LookRotation(newAngle.normalized, blackboard.transform.up);

            ////////////////////////////////////Vector3 dir = (blackboard.transform.position - lookDir);
            ////////////////////////////////////dir.y = 0;
            ////////////////////////////////////blackboard.headTransform.localEulerAngles = dir.normalized;
            //smoothly rotate towards this point
            Quaternion smoothRotation = Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed*2);
           //// blackboard.transform.rotation = smoothRotation;
        }
        //{
        //    //Vector3 dir = closest.gameObject.transform.position - blackboard.transform.position;
        //    //blackboard.transform.position -= dir.normalized * blackboard.Speed / (5) * Time.deltaTime;

        //    float distToShift = Mathf.Clamp(avoidanceRadius - Vector3.Distance(closest.gameObject.transform.position, blackboard.transform.position), -avoidanceRadius, avoidanceRadius);

        //    ///////it sees the obstical and generates a vector which falls outside of it
        //    Vector3 newPoint = closest.gameObject.transform.position;

        //    Vector3 closestDir = closest.gameObject.transform.position - blackboard.transform.position;
        //    float dotProduct = Vector3.Dot(blackboard.transform.right, closestDir.normalized);
        //    if (dotProduct < 0)
        //        newPoint -= closest.gameObject.transform.right * distToShift;
        //    //lookDir = Quaternion.AngleAxis(distToShift, blackboard.transform.up) * lookDir;
        //    else
        //        newPoint += closest.gameObject.transform.right * distToShift; //more the raycast to the right by some amount so it will avoid collision
        //    /////issue is that every frame new point gets updated, but if newpoint still does not result in a collision then do not update it
        //    //lookDir = Quaternion.AngleAxis(-distToShift, blackboard.transform.up) * lookDir;
        //    //lookDir -= new Vector3(distToShift, 0, distToShift).normalized;

        //    //the new direction to move in
        //    ////Debug.Log(newPoint + "   " + raycastHit.point);
        //    lookDir = newPoint - blackboard.transform.position;

        //    Debug.DrawRay(blackboard.transform.position, rayDist * (lookDirOrign), Color.yellow);
        //    Debug.DrawRay(blackboard.transform.position, rayDist * (blackboard.avoidanceDir), Color.black);

        //    if (blackboard.avoidanceUpdateFreqCurr <= 0) //once per second update with the avoidance
        //    {
        //        if(Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 0.1f, blackboard.transform.forward, out RaycastHit ray, sphereRadius, blackboard.EnemyLayer))
        //        {
        //           // if (ray.collider == closest)
        //            {
        //               // Debug.Log("Nearesed");
        //                blackboard.avoidanceDir = lookDir; //only actually adjust it if the current velocity would result in a collision
        //            }
        //           // else
        //            {
        //             //   Debug.Log("Failed to find it in direction so do not update");
        //            }
        //        }
        //        blackboard.avoidanceUpdateFreqCurr = blackboard.waitTimeAvoidance;
        //    }
        //    else
        //        blackboard.avoidanceUpdateFreqCurr -= Time.deltaTime;

        //        lookDir = (lookDirOrign + blackboard.avoidanceDir) / 2; //average of the two velocities

        //    //              Vector3 dir = blackboard.transform.position - raycastHit.point;


        //    ////////////gets the amount the vector needs to shift in order to avoid the object

        //    //                lookDir += dir * blackboard.rotSpeed * Time.deltaTime;
        //    //Debug.DrawRay(blackboard.transform.position, rayDist * (lookDir), Color.black);
        //    ////return lookDir;
        //}
        ////////if (//Physics.Raycast(blackboard.transform.position + blackboard.transform.up *0.1f, blackboard.transform.forward, out raycastHit, rayDist, blackboard.EnemyLayer) ||
        ////////    Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, blackboard.transform.forward + blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, blackboard.transform.forward - blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer) ||
        ////////    Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, blackboard.transform.forward + blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, blackboard.transform.forward - blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)



        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, -blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer) ||
        ////////    Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, -blackboard.transform.forward + blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, -blackboard.transform.forward - blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, -blackboard.transform.forward + blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
        ////////    || Physics.SphereCast(blackboard.transform.position + blackboard.transform.up * 0.1f, sphereRadius, -blackboard.transform.forward - blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)

        ////////    )

        ////////// || Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.forward, out raycastHit, rayDist, blackboard.EnemyLayer) ||
        //////////Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.forward + blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
        //////////  || Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.forward - blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer) ||
        //////////  Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer)
        //////////  || Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, blackboard.transform.right, out raycastHit, rayDist, blackboard.EnemyLayer) ||
        //////////  Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.forward + blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer)
        //////////  || Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, -blackboard.transform.forward - blackboard.transform.right / 2, out raycastHit, rayDist, blackboard.EnemyLayer))
        ////////{

        ////////}

        /////        Debug.DrawRay(blackboard.transform.position, rayDist * (lookDir), Color.blue);

        return lookDirOrign;
    }



    /// <summary>
    /// Set the ant so that it will always be on the ground
    /// </summary>
    /// <returns>The normal of the ground used for orienting the ant correctly</returns>
    public static Vector3 SetGround(GenericAnt blackboard)
    {
        RaycastHit raycastHit, raycastHit1;//, raycastForward, raycastMiddle;
        //when raycasting, cast from high up downwards, so that can always ensure hitting terrain and not being under it
        bool didHit = Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 15, -blackboard.transform.up, out raycastHit, 25, blackboard.groundLayer);
        //cast a ray for the back of the body as well, enabling smooth rotations while changing surface orientations
        bool didHit1 = Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset
            + blackboard.transform.up * 15, -blackboard.transform.up, out raycastHit1, 25, blackboard.groundLayer);

        if(!didHit)
            didHit = Physics.Raycast(blackboard.transform.position + blackboard.transform.up * 15, -blackboard.transform.up, out raycastHit, Mathf.Infinity, blackboard.groundLayer);
        
        if(!didHit1)
            didHit1 = Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset
            + blackboard.transform.up * 15, -blackboard.transform.up, out raycastHit1, Mathf.Infinity, blackboard.groundLayer);
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

            ////blackboard.transform.up = raycastHit.normal; //ensure always orient to the grounds locations
        }


        Vector3 upSmooth;
        if (didHit1 && didHit)//when on the edge between two different triangles, get a vector which will point up ensuring a smooth rotation between the two
        {
            //upSmooth = Vector3.Cross(blackboard.transform.right, -(raycastHit.point - raycastHit1.point).normalized);
            //Debug.Log(Vector3.Dot(blackboard.transform.up, raycastHit.normal));
            //if (Vector3.Dot(blackboard.transform.up, raycastHit.normal) < 0.5f)
              //  upSmooth = Vector3.Cross(blackboard.transform.right, raycastHit.normal);
            //else
                upSmooth = Vector3.Cross(blackboard.transform.right, -(raycastHit.point - raycastHit1.point).normalized);
        }
        //above used this link https://answers.unity.com/questions/1420677/best-way-to-rotate-player-object-to-match-the-grou.html
        else
            upSmooth = Vector3.up;

#if UNITY_EDITOR
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -0.5f, 5 * (blackboard.transform.forward), Color.green);
        Debug.DrawRay(blackboard.transform.position, 5 * (raycastHit.normal), Color.blue);
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -blackboard.backGroundCheckOffset, 5 * (raycastHit1.normal), Color.blue);
#endif
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
