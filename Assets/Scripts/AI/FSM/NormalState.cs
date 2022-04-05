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
        //stuff do when enter the state
    }

    public void execute()
    {
        if (owner.DetectPlayer())
            owner.stateMachine.changeState(owner.stateMachine.Investigate);

            topNode.evaluate();
    }

    public void exit()
    {
        //throw new System.NotImplementedException();
    }
}

public class InvestigateState : State
{
    Node topNode;

    GenericAnt owner;
    public InvestigateState(GenericAnt owner) //also initilize any behaviour tree used on the state as well
    {
        this.owner = owner;
        ExpressShock expressShock = new ExpressShock(owner);
        MoveTowards moveTowards = new MoveTowards(owner);
        CallBackup callBackup = new CallBackup(owner);


        topNode = new Sequence(new List<Node> { expressShock, moveTowards, expressShock, callBackup });
    }
    public void enter()
    {
        //stuff do when enter the state
    }

    public void execute()
    {
        if (!owner.DetectPlayer())
            owner.stateMachine.changeState(owner.stateMachine.Movement);
        else if (Vector3.Distance(owner.transform.position, owner.newNode.transform.position) < owner.attachDist)
            owner.stateMachine.changeState(owner.stateMachine.Attack);

        topNode.evaluate();
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