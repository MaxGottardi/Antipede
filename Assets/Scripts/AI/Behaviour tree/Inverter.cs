using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node //invert the value recieved
{
    protected Node child;

    public Inverter(Node child)
    {
        this.child = child;//when this class initialized set the node to be the one created upon initilization
    }

    public override NodeState evaluate()
    {
        switch (child.execute())
        {
            case NodeState.Running:
                nodeState = NodeState.Running;
                break;
            case NodeState.Success:
                nodeState = NodeState.Failure;
                child.end();
                break;
            case NodeState.Failure:
                nodeState = NodeState.Success;
                child.end();
                break;
           // default: break;
        }
        if (nodeState != NodeState.Running)
            end();
        //as all children were either running or a success
        return nodeState;
    }

    public override void loadData(ref GenericAntData saveableData)
    {
        child.loadData(ref saveableData);
    }

    public override void saveData(ref GenericAntData saveableData)
    {
        child.saveData(ref saveableData);
    }
}
