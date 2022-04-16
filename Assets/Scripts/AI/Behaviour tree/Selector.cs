using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    //run the active node until it no longer returns running
    protected List<Node> children = new List<Node>();
    int currChild = 0;

    public Selector(List<Node> children)
    {
        this.children = children;//when this class initialized set the list to be the one created upon initilization
    }

    public override NodeState evaluate() 
    {
        while (currChild < children.Count)// && nodeState != NodeState.Running)
        {
            switch (children[currChild].execute())
            {
                case NodeState.Running:
                    nodeState = NodeState.Running; //do not move onto the next node as the current one is still running
                    return nodeState;
                case NodeState.Success:
                    nodeState = NodeState.Success;
                    children[currChild].end();
                    currChild = 0;
                    return nodeState;
                case NodeState.Failure:
                    children[currChild].end();
                    currChild++;
                    nodeState = NodeState.Failure;
                    break;
            }
        }
        nodeState = NodeState.Failure;
        end();
        currChild = 0;
        return nodeState; //everything didn't execute as whole lot failed
    }

    public override void interupt() //force any all nodes to stop any execution
    {
        base.interupt();

        currChild = 0;
        foreach (Node childNode in children)
        {
            childNode.interupt();
        }
    }
}
