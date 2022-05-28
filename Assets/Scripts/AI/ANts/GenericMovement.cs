using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement
{
    public static Quaternion SetRotation(GenericAnt blackboard, Vector3 lookDir)
    {
        lookDir = RVOAvoidance(blackboard, lookDir); //appears to be working quite well, only issue now being in the way it actually senses which object to navigate around as buggs out when have multiple
        //solved ant orientation using code from here https://forum.unity.com/threads/look-at-object-while-aligned-to-surface.515743/
        (Vector3, Vector3) up = SetGround(blackboard);//the the up position of the normal of the ground

        ////Vector3 lookDir = (blackboard.nextPosVector - blackboard.transform.position) + (blackboard.transform.right * forwardOffset);
        lookDir.y = 0; //ignore all vertical height, so appears to be on flat ground
        lookDir.Normalize();



        //remove the up amount from the vector
        float d = Vector3.Dot(lookDir, up.Item1); //get the amount of the direction which was up, relative to the grounds normal
        lookDir -= d * up.Item1; //removes any upwards values, so the vectors now 90 degress to the normal and still heading in the right direction
        lookDir.Normalize();

        //convert the directional vector into a rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDir * 0.5f, up.Item1);
        Quaternion targetRotationNorm = Quaternion.LookRotation(lookDir * 0.5f, up.Item2);

        //smoothly rotate towards the desired direction
        Quaternion smoothRotation = Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed * 0.5f);
        smoothRotation = Quaternion.RotateTowards(smoothRotation, targetRotationNorm, Time.deltaTime * blackboard.rotSpeed * 0.5f);
        return smoothRotation;
    }

    /// <summary>
    /// overall goal is to find the direction required to take to ensure it does not actually collide with another object
    /// note not actual RVO, a heavily simplified and basic attempt at it which overall gives desired effects
    /// </summary>
    /// <param name="blackboard">the object the transformations get applied to</param>
    /// <returns></returns>
    public static Vector3 RVOAvoidance(GenericAnt blackboard, Vector3 lookDirOrign)
    {
        Vector3 lookDir = lookDirOrign;
        float rayDist = 4;
        ; //ajust this collision avoidance to work using RVO https://gamma.cs.unc.edu/RVO/icra2008.pdf, as current method does not work well in crowds and occilations

        float sphereRadius = 1; //this radius around own circle where anything inside will result in a collision
                                //
        float avoidanceRadius = sphereRadius * 2; //this combine raduis of the current object and the one it hit, so that only directions outside of this will never collide

        Collider[] hitColliders = Physics.OverlapSphere(blackboard.transform.position, sphereRadius, blackboard.EnemyLayer);

        float distAway = -1;
        Collider closest = null;
        foreach (Collider enemy in hitColliders) //find the enemy closest to this one
        {
            Vector3 dir = enemy.gameObject.transform.position - blackboard.transform.position;

            if (enemy.gameObject != blackboard.transform.GetChild(0).gameObject)
            {

                float newDist = Vector3.Distance(enemy.gameObject.transform.position, blackboard.transform.position);
                if (distAway < 0 || distAway > newDist)
                {
                    distAway = newDist;
                    closest = enemy;
                }
            }

        }
        if (closest != null)
        {
            Vector3 dir = closest.gameObject.transform.position - blackboard.transform.position;

            Vector3 moveDir = dir;
            Quaternion offsetAngle;
            offsetAngle = Quaternion.AngleAxis(40, blackboard.transform.up);

            moveDir.y = 0;
            Vector3 newAngle = offsetAngle * moveDir.normalized; //offset the direction slighly so it is pushed more to the side

            Debug.DrawRay(blackboard.transform.position, rayDist * (newAngle), Color.black);

            //apply a force to the object to push it away from its closest neighbour
            blackboard.transform.position -= moveDir.normalized * blackboard.Speed/5 * Time.deltaTime;

            ///find the amount to move the direction by so that it will not collide
            float distToShift = Mathf.Clamp(avoidanceRadius - Vector3.Distance(closest.gameObject.transform.position, blackboard.transform.position), -avoidanceRadius, avoidanceRadius);
            float dotProductNorm = Vector3.Dot(blackboard.transform.right, dir.normalized);

            Vector3 newPoint = closest.gameObject.transform.position;
            if (dotProductNorm < 0)
                newPoint -= closest.gameObject.transform.right * distToShift;
            else
                newPoint += closest.gameObject.transform.right * distToShift;

            lookDir = blackboard.transform.position - newPoint;

            lookDir = (lookDirOrign + lookDir) / 2; //this part from rvo, find the new direction to rotate the object towards, so it not only avoids collision, but tries to move towards the goal
            return lookDir;
        }
        return lookDirOrign;
    }



    /// <summary>
    /// Set the ant so that it will always be on the ground
    /// </summary>
    /// <returns>The normal of the ground used for orienting the ant correctly</returns>
    public static (Vector3, Vector3) SetGround(GenericAnt blackboard)
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
       

        if (didHit) //set the position of the ant to the ground
        {
            //Debug.Log(raycastHit.collider.gameObject.name);

            Vector3 groundPoint = blackboard.transform.localPosition;
            groundPoint.y = raycastHit.point.y + blackboard.groundOffset;
            blackboard.transform.position = Vector3.MoveTowards(blackboard.transform.position, groundPoint, Time.deltaTime * 100);

            //also rotate the object so it is orienting towards the grounds normal
            //Quaternion normalRotation = Quaternion.LookRotation(raycastHit.normal, blackboard.transform.right);
            //Quaternion smoothNormal = Quaternion.RotateTowards(blackboard.transform.rotation, normalRotation, Time.deltaTime * blackboard.rotSpeed);
            ////Vector3 normalDirection = Vector3.RotateTowards(blackboard.transform.up, raycastHit.normal, Time.deltaTime * blackboard.rotSpeed, 0);
            ////blackboard.transform.up = normalDirection;//Quaternion.LookRotation(normalDirection, Vector3.right);//raycastHit.normal;
            ////blackboard.transform.up = raycastHit.normal; //ensure always orient to the grounds locations
        }


        Vector3 upSmooth;
        if (didHit1 && didHit)//when on the edge between two different triangles, get a vector which will point up ensuring a smooth rotation between the two
        {
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
        return (upSmooth, raycastHit.normal);
    }
}
