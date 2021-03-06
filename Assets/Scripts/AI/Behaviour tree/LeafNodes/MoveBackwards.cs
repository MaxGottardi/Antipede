using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTooClose : Node
{
    public PlayerTooClose(GenericAnt blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState evaluate()
    {
        if (PlayerDist() < 7) //only flee if too close to the players head
            return NodeState.Success;
        return NodeState.Failure;
    }
    float PlayerDist()
    {
        return Vector3.Distance(blackboard.transform.position, GameManager1.mCentipedeBody.Head.position);
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        //no data to load
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        //not required as no new data to save
    }
}

public class MoveBackwards : Node
{ //find next position on the map to move towards

    float backwardOffset;
    float backwardTime = 2; //num seconds it can move backwards for

    int doGoBack = 1; //the direction to move in when going backwards, with -1 meaning it would move forward instead
    HunterAnt realOwner;
    public MoveBackwards(GenericAnt blackboard, int doGoBack = 1)
    {
        this.blackboard = blackboard;
        realOwner = blackboard.gameObject.GetComponent<HunterAnt>();

        this.doGoBack = doGoBack;
    }

    public override void init()
    {
        base.init();

        backwardOffset = Random.Range(-.5f, .5f);
        backwardTime = 2;
        if (realOwner)
            realOwner.isFleeing = true;
        
        if (blackboard.animMultiplier > 0)
            blackboard.animMultiplier *= -1;
    }
    public override NodeState evaluate()
    {
// Debug.Log("Moving Backwards");
        if (Vector3.Distance(blackboard.transform.position, GameManager1.mCentipedeBody.Head.position) >= 11 || backwardTime <= 0)
        {
            backwardTime = 2f;
            if (realOwner)
                realOwner.isFleeing = false;
            return NodeState.Success;
        }
        else
        {
            backwardTime -= Time.deltaTime;

            if (!blackboard.anim.GetCurrentAnimatorStateInfo(0).IsTag("isWalk")) //if not yet walking, initiate the walking animation
            {
                blackboard.anim.SetTrigger("Walk");
            }

            blackboard.transform.position -= doGoBack * (blackboard.transform.forward) * Time.deltaTime * blackboard.Speed * 1.5f; //move backwards
            Vector3 lookDir = (GameManager1.mCentipedeBody.Head.position - blackboard.transform.position) + (blackboard.transform.right * backwardOffset);
            blackboard.transform.rotation = GenericMovement.SetRotation(blackboard, lookDir); //set appropriate orientation

            return NodeState.Running;
        }
    }

    public override void end()
    {
        if (blackboard.animMultiplier < 0) //ensure the animations cannot ever be inadvertidly reversed perminently
          blackboard.animMultiplier *= -1;

        if (realOwner)
            realOwner.isFleeing = false;
        base.end();
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);

        saveableData.moveBackwardsOffset.list.Add(backwardOffset);
        saveableData.moveBackwardsTime.list.Add(backwardTime);
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);

        backwardOffset = saveableData.moveBackwardsOffset.list[0];
        saveableData.moveBackwardsOffset.list.RemoveAt(0);
        backwardTime = saveableData.moveBackwardsTime.list[0];
        saveableData.moveBackwardsTime.list.RemoveAt(0);
    }
}