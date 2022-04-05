using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelSelector : Node
{
    //each tick check the whole tree to see if the state has changed
    protected List<Node> children = new List<Node>();
    public ParallelSelector(List<Node> children)
    {
        this.children = children;//when this class initialized set the list to be the one created upon initilization
    }

    public override NodeState evaluate()
    {
        foreach (var node in children) //for each child node
        {
            switch (node.evaluate()) //issue as every frame still updating the tree, only want to do when complete the tree and everything fails
                                     //basically here just execture node until it returns a value
            {
                case NodeState.Running:
                    nodeState = NodeState.Running; //do not move onto the next node as the current one is still running
                    return nodeState;
                case NodeState.Success: //do nothing, just evaluate the next child
                    nodeState = NodeState.Success;
                    return nodeState;
                case NodeState.Failure: //whole sequence failed so break out of the method
                    nodeState = NodeState.Failure;
                    break;
                default: break;
            }
        }
        nodeState = NodeState.Failure;
        return nodeState; //everything didn't execute as whole lot failed
    }
}

