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
        switch (child.evaluate())
        {
            case NodeState.Running:
                nodeState = NodeState.Running;
                break;
            case NodeState.Success:
                nodeState = NodeState.Failure;
                break;
            case NodeState.Failure:
                nodeState = NodeState.Success;
                break;
           // default: break;
        }
        //as all children were either running or a success
        return nodeState;
    }
}
