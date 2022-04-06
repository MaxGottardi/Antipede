using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MovementState : State
{
    Node topNode;

    GenericAnt owner;
    public MovementState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
        GetNextNode getNextNode = new GetNextNode(owner);
        MoveTowards moveTowards = new MoveTowards(owner);

        topNode = new Sequence(new List<Node> { getNextNode, moveTowards });
    }
    public void enter()
    {
        GameObject closestNode = owner.nodesList[0];
        foreach (GameObject node in owner.nodesList)
        {
            if(Vector3.Distance(node.transform.position, owner.transform.position) < Vector3.Distance(closestNode.transform.position, owner.transform.position))
                closestNode = node;
        }
        owner.newNode = closestNode;
        //find the nearest node to this object as it would have been lost
        //stuff do when enter the state
    }

    public void execute()
    {
        if (owner.DetectPlayer() || owner.isRienforcement)
        {
            owner.isRienforcement = false;
            owner.stateMachine.changeState(owner.stateMachine.Investigate);
        }

            topNode.evaluate();
    }

    public void exit()
    {
        //throw new System.NotImplementedException();
    }
}

public class InvestigateState : State
{
    //Node topNode;

    GenericAnt owner;
    float detectPlayerTime = 1.0f;

    DetermineShockState shockState;
    public InvestigateState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
        shockState = new DetermineShockState(owner);

       // ExpressShock expressShock = new ExpressShock(owner);
        //moveTowards = new MoveTowards(owner);
       // CallBackup callBackup = new CallBackup(owner);
    }
    public void enter()
    {        
        shockState.enter();

        //stuff do when enter the state
        detectPlayerTime = 3.0f;
    }

    public void execute()
    {
        if (!owner.DetectPlayer())
        {
            detectPlayerTime -= Time.deltaTime;
            if (detectPlayerTime <= 0)
            {
                owner.shockBars[0].SetActive(false);
                owner.shockBars[1].SetActive(false);
                owner.shockBars[2].SetActive(false);
                detectPlayerTime = 3.0f;
                owner.stateMachine.changeState(owner.stateMachine.Movement);
            }
        }
        else if (Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < owner.attachDist)
            owner.stateMachine.changeState(owner.stateMachine.Attack);
        else
            detectPlayerTime = 3.0f;

        Debug.Log(shockState);
        shockState.execute();
    }

    public void exit()
    {
        //deactivate all showing warning symbols
        //throw new System.NotImplementedException();
    }
}

public class AttackState : State
{
    Node topNode;

    GenericAnt owner;
    public AttackState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
        // ExpressShock expressShock = new ExpressShock(owner);
        //MoveTowards moveTowards = new MoveTowards(owner);
        //CallBackup callBackup = new CallBackup(owner);


        //topNode = new Sequence(new List<Node> { expressShock, moveTowards, expressShock, callBackup });
    }
    public void enter()
    {
        //stuff do when enter the state
    }

    public void execute()
    {
        if (!owner.DetectPlayer())
            owner.stateMachine.changeState(owner.stateMachine.Movement);

        topNode.evaluate();
    }

    public void exit()
    {
        //deactivate all showing warning symbols
        //throw new System.NotImplementedException();
    }
}