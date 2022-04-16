using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    protected List<Node> children = new List<Node>();
    int currChild = 0;
    public Sequence(List<Node> children)
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
                    nodeState = NodeState.Running;
                    return nodeState;
                case NodeState.Success:
                    nodeState = NodeState.Success;
                    children[currChild].end();
                    currChild++;
                    break;
                case NodeState.Failure:
                    children[currChild].end();
                    currChild = 0;
                    nodeState = NodeState.Failure;
                    end();
                    return nodeState;
            }
        }
        nodeState = NodeState.Success; //as every node has succesfully run the whole thing was a success
        end();
        currChild = 0;
        return nodeState;
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
