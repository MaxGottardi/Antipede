using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelSequence : Node
{
    //run all sequences each tick
    protected List<Node> children = new List<Node>();
    public ParallelSequence(List<Node> children)
    {
        this.children = children;//when this class initialized set the list to be the one created upon initilization
    }

    public override NodeState evaluate()
    {
        bool isAnyChildRunning = false; //set to true if at any point come across a child thats running
        foreach (var node in children) //for each child node
        {
            switch (node.execute()) //issue as every frame still updating the tree, only want to do when complete the tree and everything fails
                                     //basically here just execture node until it returns a value
            {
                case NodeState.Running:
                    isAnyChildRunning = true; //then go to the next child for evaluation
                    break;
                //return NodeState.Running;
                case NodeState.Success: //do nothing, just evaluate the next child
                    node.end();
                    break;
                case NodeState.Failure: //whole sequence failed so break out of the method
                    nodeState = NodeState.Failure;
                    node.end();
                    return nodeState;
                default: break;
            }
        }
        //as all children were either running or a success
        nodeState = isAnyChildRunning ? NodeState.Running : NodeState.Success;
        if (nodeState != NodeState.Running)
            end();
        return nodeState;
    }

    public override void interupt() //force any all nodes to stop any execution
    {
        base.interupt();

        foreach (Node childNode in children)
        {
            childNode.interupt();
        }
    }

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        for (int i = 0; i < children.Count; i++)
        {
            children[i].loadData(saveableData);
        }
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        for (int i = 0; i < children.Count; i++)
        {
            children[i].saveData(saveableData);
        }
    }
}
