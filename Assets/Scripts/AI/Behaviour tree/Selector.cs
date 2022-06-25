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

    public override void loadData(ref GenericAntData saveableData)
    {
        currChild = saveableData.aISelectorChildCounts.list[0];//as always adding to the end of the list, when reach this script it should be the first one
        saveableData.aISelectorChildCounts.list.RemoveAt(0);//as used no longer needed so remove it, making the next element in the list the first one
        for (int i = 0; i < children.Count; i++)
        {
            children[i].loadData(ref saveableData);
        }
    }

    public override void saveData(ref GenericAntData saveableData)
    {
        saveableData.aISelectorChildCounts.list.Add(currChild);
        for (int i = 0; i < children.Count; i++)
        {
            children[i].saveData(ref saveableData);
        }

        ////need some way to tell it which selector gets which curr child value
    }
}
