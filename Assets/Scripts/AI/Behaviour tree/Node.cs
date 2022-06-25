using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//behaviour tree used to actually determine what occurs at each state e.g for movement 2 nodes would exists one for determine next position and the other for moving to that pos(if nessesary)
public abstract class Node
{
    public GenericAnt blackboard;

    protected bool isRunning = false; //has this node been running previously or not within the same cycle
    public NodeState nodeState;
    public enum NodeState { Success, Failure, Running };

    public virtual void init() //called on first frame node runs, and only called again once node fully completed runthrough 
    {
        isRunning = true;
    }

    public NodeState execute()
    {
        if (!isRunning)
            init();

        return evaluate();
    }

    public abstract NodeState evaluate(); //determine the state the node is in upon completion, what the node does while it is running

    public virtual void end()     //called whenever the node stops running
    {
        isRunning = false;
    }

    public virtual void interupt() //called when node is interupted and forced to stop execution
    {
        end();
    }

    public abstract void saveData(ref GenericAntData saveableData);
    public abstract void loadData(ref GenericAntData saveableData);
}
