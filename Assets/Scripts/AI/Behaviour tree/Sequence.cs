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
            switch (children[currChild].evaluate())
            {
                case NodeState.Running:
                    nodeState = NodeState.Running;
                    return nodeState;
                case NodeState.Success:
                    nodeState = NodeState.Success;
                    currChild++;
                    break;
                case NodeState.Failure:
                    currChild = 0;
                    nodeState = NodeState.Failure;
                    return nodeState;
            }
        }
        nodeState = NodeState.Success; //as every node has succesfully run the whole thing was a success
        currChild = 0;
        return nodeState;
        //bool isAnyChildRunning = false; //set to true if at any point come across a child thats running
        //foreach (var node in children) //for each child node
        //{
        //    switch (node.evaluate()) //issue as every frame still updating the tree, only want to do when complete the tree and everything fails
        //        //basically here just execture node until it returns a value
        //    {
        //        case NodeState.Running:
        //            isAnyChildRunning = true; //then go to the next child for evaluation
        //            break;
        //            //return NodeState.Running;
        //        case NodeState.Success: //do nothing, just evaluate the next child
        //            break;
        //        case NodeState.Failure: //whole sequence failed so break out of the method
        //            nodeState = NodeState.Failure;
        //            return nodeState;
        //        default: break;
        //    }
        //}
        ////as all children were either running or a success
        //nodeState = isAnyChildRunning ? NodeState.Running : NodeState.Success;
        //return nodeState;
    }
}
