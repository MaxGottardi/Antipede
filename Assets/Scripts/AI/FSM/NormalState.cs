using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
/// <summary>
/// The normal movement around the map
/// </summary>
public class MovementState : State
{
    Node topNode;

    GenericAnt owner;
    public MovementState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
        GetNextNode getNextNode = new GetNextNode(owner);
        MoveTowards moveTowards = new MoveTowards(owner, true);

        topNode = new Sequence(new List<Node> { getNextNode, moveTowards });
    }
    public void enter() //omly important when other states also added in
    {
        GameObject closestNode = owner.nodesList[0]; //find the next location to move towards
        foreach (GameObject node in owner.nodesList)
        {
            if (Vector3.Distance(node.transform.position, owner.transform.position) < Vector3.Distance(closestNode.transform.position, owner.transform.position))
                closestNode = node;
        }
        owner.nextPosTransform = closestNode.transform;

        //owner.shockBar.SetActive(false);
        //find the nearest node to this object as it would have been lost
        //stuff do when enter the state
    }

    public void execute()
    {
        if (owner.DetectPlayer() || owner.canInvestigate)
            owner.stateMachine.changeState(owner.stateMachine.Shock);
        else
            topNode.execute();
    }

    public void exit()
    {
        topNode.interupt();

        if (owner.canInvestigate)
            owner.canInvestigate = false;
    }
}
/// <summary>
/// Called when AI first sees player and it showcases warning above head
/// </summary>
public class ShockState : State
{
    float shockTime = 0.45f;
    GenericAnt owner;
    public ShockState(GenericAnt owner)
    {
        this.owner = owner;
    }
    public void enter()
    {
        shockTime = 0.45f;
        owner.callBackupWait = 0; //reset the ability to be able to call for backup
        owner.shockBar.SetActive(true);
    }

    public void execute()
    {
        shockTime -= Time.deltaTime;
        if (shockTime <= 0)
            owner.stateMachine.changeState(owner.stateMachine.Investigate);

    }

    public void exit()
    {
        shockTime = 0.45f;
        owner.shockBar.SetActive(false);
    }
}
/// <summary>
/// When seen player and moving towards it
/// </summary>
public class InvestigateState : State
{
    GenericAnt owner;
    float lostPlayerTime = 3.0f;

    Node topNode;
    public InvestigateState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;

        DetermineAttackSeg determineAttackSeg = new DetermineAttackSeg(owner);

        PathToSegment pathToSegment = new PathToSegment(owner);
        MoveTowards moveTowards = new MoveTowards(owner, false);

        CanCallBackup canCallBackup = new CanCallBackup(owner);
        CallBackup callBackup = new CallBackup(owner);
        Sequence callBackupSeq = new Sequence(new List<Node> { canCallBackup, callBackup });
        Succeeder backupSucceeder = new Succeeder(callBackupSeq);

        Sequence moveSequence = new Sequence(new List<Node> { pathToSegment, moveTowards, backupSucceeder });
        RepeatUntilFail repeatUntilFail = new RepeatUntilFail(moveSequence);

        topNode = new Sequence(new List<Node> { determineAttackSeg, repeatUntilFail });
    }

    public void enter()
    {   
        lostPlayerTime = 3.0f;

        owner.callingBackup = false;
    }
    public void execute()
    {
        if (owner.callBackupWait > 0) //update wating time between each call for beckup
            owner.callBackupWait -= Time.deltaTime;

        if (!owner.DetectPlayer())
            lostPlayerTime -= Time.deltaTime; //update amount of time not seen player for
        else if(lostPlayerTime < 3.0f)
            lostPlayerTime = 3.0f;

        if (!owner.callingBackup && lostPlayerTime <= 0) //when haven't seen player for 3 seconds or segment already destroyed
        {
            //if (owner.nextPosTransform && owner.nextPosTransform.gameObject.GetComponent<MSegment>())
            //    owner.nextPosTransform.gameObject.GetComponent<MSegment>().beingAttacked = false;
            owner.stateMachine.changeState(owner.stateMachine.Movement);
        }
        else if (owner.nextPosTransform && Vector3.Distance(owner.transform.position, owner.nextPosTransform.position) < owner.attachDist && !owner.callingBackup) //within attack range of chosen player segment
            owner.stateMachine.changeState(owner.stateMachine.Attack);
        else //as no change of state occured, can run this one
            topNode.execute();
    }

    public void exit()
    {
        topNode.interupt();

        owner.shockBar.SetActive(false);
        lostPlayerTime = 0;
    }
}
/// <summary>
/// when within certain distance of player and have begun attacking
/// </summary>
public class AttackState : State
{
    float attackTime = 1.0f;
    GenericAnt owner;
    public AttackState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;


        //PerformAttack performAttack = new PerformAttack(owner);
        //AttackWait attackWait = new AttackWait(owner);
        ////MoveTowards moveTowards = new MoveTowards(owner);
        ////CallBackup callBackup = new CallBackup(owner);


        //topNode = new Sequence(new List<Node> { performAttack, attackWait });
    }
    public void enter()
    {
        attackTime = 1.0f;
        
        //initiate the attack animation and junk
    }

    public void execute()
    {
        attackTime -= Time.deltaTime;
        if (attackTime <= 0)//when finished attacking add any damage to the appropriate segment
        {
            if (owner.nextPosTransform && Vector3.Distance(owner.transform.position, owner.nextPosTransform.transform.position) < owner.attachDist)
                GameManager1.playerObj.GetComponent<MCentipedeBody>().RemoveSegment();
            owner.stateMachine.changeState(owner.stateMachine.Investigate);
        }
    }

    public void exit()
    {
        attackTime = 1.0f;
        //deactivate all showing warning symbols
        //throw new System.NotImplementedException();
    }
}

public class DamageState : State
{
    float damageTime = 1.0f;
    GenericAnt owner;
    public DamageState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;


        //PerformAttack performAttack = new PerformAttack(owner);
        //AttackWait attackWait = new AttackWait(owner);
        ////MoveTowards moveTowards = new MoveTowards(owner);
        ////CallBackup callBackup = new CallBackup(owner);


        //topNode = new Sequence(new List<Node> { performAttack, attackWait });
    }
    public void enter()
    {
        damageTime = 1.0f;
        owner.transform.rotation = Quaternion.Euler(90, 67, 180);
        //initiate the attack animation and junk
    }

    public void execute()
    {
        damageTime -= Time.deltaTime;
        if (damageTime <= 0)//when finished attacking add any damage to the appropriate segment
        {
            owner.stateMachine.changeState(owner.stateMachine.Investigate);
        }
    }

    public void exit()
    {
        damageTime = 1.0f;
        owner.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}

public class DeadState : State
{
    float deadTime = 1.0f;
    GenericAnt owner;
    public DeadState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;


        //PerformAttack performAttack = new PerformAttack(owner);
        //AttackWait attackWait = new AttackWait(owner);
        ////MoveTowards moveTowards = new MoveTowards(owner);
        ////CallBackup callBackup = new CallBackup(owner);


        //topNode = new Sequence(new List<Node> { performAttack, attackWait });
    }
    public void enter()
    {
        deadTime = 1.0f;

        owner.transform.rotation = Quaternion.Euler(0, 0, 180);
        //initiate the attack animation and junk
    }

    public void execute()
    {
        deadTime -= Time.deltaTime;
        if (deadTime <= 0)//when finished attacking add any damage to the appropriate segment
        {
            MonoBehaviour.Destroy(owner.gameObject);
        }
    }

    public void exit()
    {
        deadTime = 1.0f;
    }
}