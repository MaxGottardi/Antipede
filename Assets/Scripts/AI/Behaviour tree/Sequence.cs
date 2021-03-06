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

    public override void loadData(GenericAntData saveableData)
    {
        base.loadData(saveableData);
        currChild = saveableData.aISequenceChildCounts.list[0];//as always adding to the end of the list, when reach this script it should be the first one
        saveableData.aISequenceChildCounts.list.RemoveAt(0);//as used no longer needed so remove it, making the next element in the list the first one
        for (int i = 0; i < children.Count; i++)
        {
            children[i].loadData(saveableData);
        }
    }

    public override void saveData(GenericAntData saveableData)
    {
        base.saveData(saveableData);
        saveableData.aISequenceChildCounts.list.Add(currChild);
        for (int i = 0; i < children.Count; i++)
        {
            children[i].saveData(saveableData);
        }

        ////need some way to tell it which selector gets which curr child value
    }
}
