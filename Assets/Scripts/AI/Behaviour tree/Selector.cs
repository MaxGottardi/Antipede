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
        doInit();

        while (currChild < children.Count)// && nodeState != NodeState.Running)
        {
            switch (children[currChild].evaluate())
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

        ////count thing to store the current child which is running
        //foreach (var node in children) //for each child node
        //{
        //    switch (node.evaluate())
        //    {
        //        case NodeState.Running:
        //            nodeState = NodeState.Running;
        //            return nodeState;
        //        case NodeState.Success:
        //            nodeState = NodeState.Success;
        //            return nodeState;
        //        case NodeState.Failure:
        //            break; //just evaluate the next child node
        //        default: break;
        //    }
        //}
        ////as all children were either running or a success
        //nodeState = NodeState.Failure; //as no child ran everything failed
        //return nodeState;
    }
}
