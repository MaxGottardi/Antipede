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
        // blackboard.transform.position = new Vector3(blackboard.transform.position.x, 0.193f, blackboard.transform.position.z);

        if (blackboard.nextPosTransform == null)
        {
            Debug.Log("Failed!!!!!!!!!");
            return NodeState.Failure;
        }
        float currDist = 0; //current distance away from the segment
        if (doDistCheck)
            currDist = NodeDistance();

        Vector3 lookDir = blackboard.nextPosVector - blackboard.transform.position;
        //lookPos.y = 0.193f;//blackboard.transform.position.y;

        Quaternion targetRotation = Quaternion.LookRotation((lookDir), Vector3.up); //get a vector pointing from current position to the new one and convert it to a rotation

        Vector3 ground = SetGround();

        Vector3 NormalForward = Vector3.Cross(blackboard.transform.GetChild(0).forward, ground);
        Vector3 NormalRight = Vector3.Cross(blackboard.transform.right, ground);


        //Debug.DrawRay(blackboard.transform.position, 25 * Vector3.Cross(SetGround(), (lookPos - blackboard.transform.position)).normalized, Color.blue);

        Quaternion antRotation = Quaternion.RotateTowards(blackboard.transform.rotation, targetRotation, Time.deltaTime * blackboard.rotSpeed);

////        Debug.Log(targetRotation + "the ants rotation");
        //blackboard.transform.localRotation = finalRote;//Quaternion.Euler(SetGround().x, antRotation.y, SetGround().z);

        blackboard.transform.rotation = antRotation;

        //targetRotation.eulerAngles = ground.eulerAngles;
        //targetRotation.z = ground.z;
        //        targetRotation.eulerAngles = new Vector3(ground.eulerAngles.x, targetRotation.eulerAngles.y, ground.eulerAngles.z);
        blackboard.transform.GetChild(0).rotation = Quaternion.LookRotation(Vector3.Cross(blackboard.transform.GetChild(0).right, ground));
        //        blackboard.transform.GetChild(0).right = NormalRight;
        // blackboard.transform.GetChild(0).forward = blackboard.transform.forward;


        //blackboard.transform.rotation = SetGround();//Quaternion.LookRotation(Vector3.right, SetGround());

        //Vector3 newRote;
        //newRote.x = blackboard.transform.GetChild(0).eulerAngles.x;
        //newRote.y = 0;
        //newRote.z = blackboard.transform.GetChild(0).eulerAngles.z;
        //blackboard.transform.GetChild(0).localEulerAngles = newRote;

        //blackboard.transform.rotation = Quaternion.LookRotation(Vector3.Cross(blackboard.transform.TransformDirection(Vector3.right), SetGround()));//lookPos - blackboard.transform.position));//Quaternion.Euler(SetGround().x, antRotation.y, SetGround().z);

        // blackboard.transform.up += SetGround();
        // blackboard.transform.up = SetGround();

        //I have 2 different directions
        /*
            the normal which is the direction need to be at to be perpendicular to the ground

            the forward direction which is the direction needing to be to ensure heading towards the desired object
         */



        if (!doDistCheck || currDist < 1) //here if say a large lag spike and goes over the node but as never within range will always fail otherwise
        {
            Debug.Log("Successfully reached the next node");
            return NodeState.Success;
        }
        else
            return NodeState.Running;

    }

    float NodeDistance()
    {
        return Vector3.Distance(blackboard.transform.position, blackboard.nextPosVector);
    }

    /// <summary>
    /// Set the ant so that it will always be on the ground
    /// </summary>
    Vector3 SetGround()
    {
        RaycastHit raycastHit, raycastHit1;
        bool didHit = Physics.Raycast(blackboard.transform.position, -Vector3.up, out raycastHit, 15, blackboard.groundLayer);
        Physics.Raycast(blackboard.transform.position + blackboard.transform.forward * -0.5f, -Vector3.up, out raycastHit1, 15, blackboard.groundLayer);

        Vector3 groundPoint = blackboard.transform.position;
        groundPoint.y = raycastHit.point.y + 0.15f;
        blackboard.transform.position = groundPoint;

        Vector3 upos = Vector3.Cross(blackboard.transform.GetChild(0).right, (raycastHit1.point - raycastHit.point).normalized);
        Debug.DrawRay(blackboard.transform.position + blackboard.transform.forward * -0.5f, 5 * (upos), Color.green);

        if (didHit)
            return raycastHit.normal;
        else
            return blackboard.transform.GetChild(0).up;
    }
}